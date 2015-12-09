using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Jolt.NET.Data;
using Jolt.NET.Data.DataStorage;
using Jolt.NET.Network;
using Jolt.NET.UI.Common;

namespace Jolt.NET.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        GameClient client;
        TrophyClient trophies;
        DataStorageClient storage;
        SessionManager session;

        private string _gameId;
        public string GameId
        {
            get { return _gameId; }
            set { Set(() => GameId, ref _gameId, value, true); }
        }

        private string _gameKey;
        public string GameKey
        {
            get { return _gameKey; }
            set { Set(() => GameKey, ref _gameKey, value, true); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { Set(() => UserName, ref _userName, value, true); }
        }

        private string _token;
        public string Token
        {
            get { return _token; }
            set { Set(() => Token, ref _token, value, true); }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { Set(() => IsLoggedIn, ref _isLoggedIn, value); }
        }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        private DateTime _lastPinged;
        public DateTime LastPinged
        {
            get { return _lastPinged; }
            set { Set(() => LastPinged, ref _lastPinged, value); }
        }

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

        private ObservableCollection<KeyValuePair<DataStorageKey, string>> _dataStorage;
        public ObservableCollection<KeyValuePair<DataStorageKey, string>> DataStorage
        {
            get { return _dataStorage; }
            set { Set(() => DataStorage, ref _dataStorage, value); }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            client = new GameClient();
            client.AuthenticationCompleted += OnAuthenticationCompleted;

            session = new SessionManager();
            session.PingSessionCompleted += OnPingSessionCompleted;

            trophies = new TrophyClient();
            storage = new DataStorageClient();

            GameId = Properties.Settings.Default.GameId;
            GameKey = Properties.Settings.Default.GameKey;
            UserName = Properties.Settings.Default.UserName;
            Token = Properties.Settings.Default.Token;

            RegisterGameCommand = new AutoRelayCommand(RegisterGame, CanRegisterGame);
            RegisterGameCommand.DependsOn(() => GameId);
            RegisterGameCommand.DependsOn(() => GameKey);
            AuthenticateUserCommand = new AutoRelayCommand(AuthenticateUser, CanAuthenticateUser);
            AuthenticateUserCommand.DependsOn(() => UserName);
            AuthenticateUserCommand.DependsOn(() => Token);
            LoadTrophiesCommand = new AutoRelayCommand(LoadTrophies, CanLoadTrophies);
            LoadTrophiesCommand.DependsOn(() => User);
            LoadConditionalTrophiesCommand = new AutoRelayCommand(LoadConditionalTrophies, CanLoadConditionalTrophies);
            LoadConditionalTrophiesCommand.DependsOn(() => User);
            AchieveTrophyCommand = new RelayCommand<Trophy>(AchieveTrophy, CanAchieveTrophy);
            FetchDataStorageCommand = new AutoRelayCommand(FetchDataStorage, CanFetchDataStorage);
            FetchDataStorageCommand.DependsOn(() => User);
            FetchDataCommand = new RelayCommand<DataStorageKey>(FetchData);
        }

        private void OnPingSessionCompleted(object sender, ResponseEventArgs e)
        {
            if (e.Success)
                LastPinged = DateTime.Now;
        }

        private async void OnAuthenticationCompleted(object sender, ResponseEventArgs e)
        {
            IsLoggedIn = e.Success;
            if (e.Success)
            {
                RaisePropertyChanged(() => User, default(User), default(User), true);
                await session.OpenSession();
                await session.PingSession();
                session.AutoPing();
            }
        }

        public AutoRelayCommand RegisterGameCommand { get; set; }
        private void RegisterGame()
        {
            client.RegisterGame(GameId, GameKey);
        }
        private bool CanRegisterGame()
        {
            return !string.IsNullOrEmpty(GameId)
                && !string.IsNullOrEmpty(GameKey);
        }

        public AutoRelayCommand AuthenticateUserCommand { get; set; }
        private async void AuthenticateUser()
        {
            await client.AuthenticateUser(UserName, Token);
        }
        private bool CanAuthenticateUser()
        {
                return !string.IsNullOrEmpty(UserName)
                && !string.IsNullOrEmpty(Token);
        }

        public AutoRelayCommand LoadTrophiesCommand { get; set; }
        private async void LoadTrophies()
        {
            Trophies = new ObservableCollection<Trophy>();
            var data = await trophies.FetchAllTrophies();
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
                data = await trophies.FetchAchievedTrophies();
            else
                data = await trophies.FetchNonAchievedTrophies();
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

        public AutoRelayCommand FetchDataStorageCommand { get; set; }
        private async void FetchDataStorage()
        {
            var data = await storage.GetDataStorageKeys();
            DataStorage = new ObservableCollection<KeyValuePair<DataStorageKey, string>>();
            foreach (var item in data)
            {
                DataStorage.Add(new KeyValuePair<DataStorageKey, string>(item, string.Empty));
            }
        }
        private bool CanFetchDataStorage()
        {
            return User != null
                && User.IsAuthenticated;
        }

        public RelayCommand<DataStorageKey> FetchDataCommand { get; set; }
        private async void FetchData(DataStorageKey key)
        {
            var data = new StringDataStoreEntry();
            data.Key = key.Key;
            await data.FetchDataStorageEntry();
            
            for(int i = 0; i < DataStorage.Count -1; i++)
            {
                if (DataStorage[i].Key == key)
                    DataStorage[i] = new KeyValuePair<DataStorageKey, string>(key, data.Data);
            }
        }

        public RelayCommand<Trophy> AchieveTrophyCommand { get; set; }
        private async void AchieveTrophy(Trophy trophy)
        {
            await trophies.AchieveTrophy(trophy.Id);
            var data = await trophies.FetchTrophyById(trophy.Id);
            var element = Trophies.SingleOrDefault(x => x == trophy);
            element = data.SingleOrDefault();
        }
        private bool CanAchieveTrophy(Trophy trophy)
        {
            return trophy != null
                && trophy.Achieved.Equals("false", StringComparison.InvariantCultureIgnoreCase);
        }

        public override void Cleanup()
        {
            session.EndAutoPing();
            base.Cleanup();
        }
    }
}