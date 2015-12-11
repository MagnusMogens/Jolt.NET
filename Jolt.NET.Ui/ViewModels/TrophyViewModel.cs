using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.Data;
using Jolt.NET.Network;
using Jolt.NET.UI.Common;

namespace Jolt.NET.UI.ViewModels
{
    public class TrophyViewModel : ViewModelBase
    {
        #region Properties

        private IDialogService DialogService { get; set; }
        private TrophyClient TrophyClient { get; set; }
        private GameClient Client { get; set; }

        private ObservableCollection<Trophy> _trophies;
        public ObservableCollection<Trophy> Trophies
        {
            get { return _trophies; }
            set { Set(() => Trophies, ref _trophies, value); }
        }

        private bool _fetchAvhieved;
        public bool FetchAchieved
        {
            get { return _fetchAvhieved; }
            set { Set(() => FetchAchieved, ref _fetchAvhieved, value); }
        }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        #endregion

        public TrophyViewModel(IDialogService dialogService, GameClient client, TrophyClient trophyClient)
        {
            Client = client;
            Client.AuthenticationCompleted += OnAuthenticationCompleted;

            DialogService = dialogService;
            TrophyClient = trophyClient;

            LoadTrophiesCommand = new AutoRelayCommand(LoadTrophies, CanLoadTrophies);
            LoadTrophiesCommand.DependsOn(() => User);
            LoadConditionalTrophiesCommand = new AutoRelayCommand(LoadConditionalTrophies, CanLoadConditionalTrophies);
            LoadConditionalTrophiesCommand.DependsOn(() => User);
            AchieveTrophyCommand = new RelayCommand<Trophy>(AchieveTrophy, CanAchieveTrophy);
        }

        #region Methods
        private async void ShowResultOnError(SuccessResponse response)
        {
            if (!response.Success)
            {
                await DialogService.ShowError(response.Message, "Error in web api request", "", null);
            }
        }

        private void OnAuthenticationCompleted(object sender, ResponseEventArgs e)
        {
            RaisePropertyChanged(() => User, default(User), default(User), true);
        }

        #endregion

        #region Commands

        public AutoRelayCommand LoadTrophiesCommand { get; set; }
        private async void LoadTrophies()
        {
            Trophies = new ObservableCollection<Trophy>();
            var data = await TrophyClient.FetchAllTrophies();
            foreach (var item in data)
            {
                Trophies.Add(item);
            }
        }
        private bool CanLoadTrophies()
        {
            return User != null
                && User.IsAuthenticated;
        }

        public AutoRelayCommand LoadConditionalTrophiesCommand { get; set; }
        private async void LoadConditionalTrophies()
        {
            Trophies = new ObservableCollection<Trophy>();
            IEnumerable<Trophy> data;
            if (FetchAchieved)
                data = await TrophyClient.FetchAchievedTrophies();
            else
                data = await TrophyClient.FetchNonAchievedTrophies();
            foreach (var item in data)
            {
                Trophies.Add(item);
            }
        }
        private bool CanLoadConditionalTrophies()
        {
            return User != null
                && User.IsAuthenticated;
        }

        public RelayCommand<Trophy> AchieveTrophyCommand { get; set; }
        private async void AchieveTrophy(Trophy trophy)
        {
            await TrophyClient.AchieveTrophy(trophy.Id);
            var data = await TrophyClient.FetchTrophyById(trophy.Id);
            var element = Trophies.SingleOrDefault(x => x == trophy);
            element = data.SingleOrDefault();
        }
        private bool CanAchieveTrophy(Trophy trophy)
        {
            return trophy != null
                && trophy.Achieved.Equals("false", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
