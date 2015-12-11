using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.Data;
using Jolt.NET.Network;
using Jolt.NET.UI.Common;

namespace Jolt.NET.UI.ViewModels
{
    public class ScoreViewModel : ViewModelBase
    {
        #region Properties

        private IDialogService DialogService { get; set; }
        private GameClient Client { get; set; }
        private ScoreClient ScoreClient { get; set; }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        private ObservableCollection<ScoreTable> _scoreTables;
        public ObservableCollection<ScoreTable> ScoreTables
        {
            get { return _scoreTables; }
            set { Set(() => ScoreTables, ref _scoreTables, value); }
        }

        private ScoreTable _selectedScoreTable;
        public ScoreTable SelectedScoreTable
        {
            get { return _selectedScoreTable; }
            set
            {
                Set(() => SelectedScoreTable, ref _selectedScoreTable, value, true);
                FetchScoresCommand.Execute(null);
            }
        }

        private ObservableCollection<Score> _scoreData;
        public ObservableCollection<Score> ScoreData
        {
            get { return _scoreData; }
            set { Set(() => ScoreData, ref _scoreData, value); }
        }

        private string _newScore;
        public string NewScore
        {
            get { return _newScore; }
            set { Set(() => NewScore, ref _newScore, value, true); }
        }

        private int _newScoreSort;
        public int NewScoreSort
        {
            get { return _newScoreSort; }
            set { Set(() => NewScoreSort, ref _newScoreSort, value, true); }
        }

        private string _newScoreExtraData;
        public string NewScoreExtraData
        {
            get { return _newScoreExtraData; }
            set { Set(() => NewScoreExtraData, ref _newScoreExtraData, value, true); }
        }

        private ScoreTable _newScoreSelectedTable;
        public ScoreTable NewScoreSelectedTable
        {
            get { return _newScoreSelectedTable; }
            set { Set(() => NewScoreSelectedTable, ref _newScoreSelectedTable, value, true); }
        }

        #endregion

        public ScoreViewModel(IDialogService dialogService, GameClient client, ScoreClient scoreClient)
        {
            Client = client;
            Client.AuthenticationCompleted += OnAuthenticationCompleted;

            DialogService = dialogService;
            ScoreClient = scoreClient;

            FetchScoreTablesCommand = new AutoRelayCommand(FetchScoreTables, CanFetchScoreTables);
            FetchScoreTablesCommand.DependsOn(() => User);
            FetchScoresCommand = new AutoRelayCommand(FetchScores, CanFetchScores);
            FetchScoresCommand.DependsOn(() => SelectedScoreTable);
            AddNewScoreCommand = new AutoRelayCommand(AddNewScore, CanAddNewScore);
            AddNewScoreCommand.DependsOn(() => NewScore);
            AddNewScoreCommand.DependsOn(() => NewScoreSelectedTable);
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

        public AutoRelayCommand FetchScoreTablesCommand { get; set; }
        private async void FetchScoreTables()
        {
            ScoreTables = new ObservableCollection<ScoreTable>();
            var tables = await ScoreClient.FetchScoreTables();
            foreach (var item in tables)
            {
                ScoreTables.Add(item);
            }
        }
        private bool CanFetchScoreTables()
        {
            return User != null
                && User.IsAuthenticated;
        }

        public AutoRelayCommand FetchScoresCommand { get; set; }
        private async void FetchScores()
        {
            ScoreData = new ObservableCollection<Score>();
            var scoreList = await ScoreClient.FetchScores(new List<int> { SelectedScoreTable.Id }, 100);
            foreach (var item in scoreList.OrderBy(x => x.Sort))
            {
                ScoreData.Add(item);
            }
        }
        private bool CanFetchScores()
        {
            return SelectedScoreTable != null;
        }

        public AutoRelayCommand AddNewScoreCommand { get; set; }
        private async void AddNewScore()
        {
            var internTable = NewScoreSelectedTable;
            ShowResultOnError(await ScoreClient.AddScore(NewScore, NewScoreSort, NewScoreSelectedTable.Id, null, NewScoreExtraData));
            FetchScoreTablesCommand.Execute(null);
            SelectedScoreTable = internTable;
        }
        private bool CanAddNewScore()
        {
            return NewScoreSelectedTable != null
                && !string.IsNullOrEmpty(NewScore);
        }

        #endregion
    }
}
