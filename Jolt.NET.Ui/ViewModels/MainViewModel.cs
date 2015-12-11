using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.Data;
using Jolt.NET.Network;

namespace Jolt.NET.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private GameClient Client { get; set; }
        private SessionManager SessionManager { get; set; }
        private IDialogService DialogService { get; set; }

        private ObservableCollection<string> _requestUrls;
        public ObservableCollection<string> RequestUrls
        {
            get { return _requestUrls; }
            set { Set(() => RequestUrls, ref _requestUrls, value); }
        }
        private int _requestUrlsLastIndex;
        public int RequestUrlsLastIndex
        {
            get { return _requestUrlsLastIndex; }
            set { Set(() => RequestUrlsLastIndex, ref _requestUrlsLastIndex, value); }
        }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        public string GameId
        {
            get { return Core.Settings.Instance.GameId; }
        }

        public SessionStatus Status
        {
            get { return SessionManager.GetSessionStatus(); }
        }

        private DateTime _lastPinged;
        public DateTime LastPinged
        {
            get { return _lastPinged; }
            set { Set(() => LastPinged, ref _lastPinged, value); }
        }
        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogService dialogService, GameClient client, SessionManager sessionManager)
        {
            DialogService = dialogService;
            RequestUrls = new ObservableCollection<string>();
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(RequestUrls, new object());
            NetworkClient.NewUrlCreated += OnNewUrlCreated;

            Client = client;
            Client.AuthenticationCompleted += OnAuthenticationCompleted;

            SessionManager = sessionManager;
            SessionManager.PingSessionCompleted += OnPingSessionCompleted;
        }

        #region Methods

        private async void OnAuthenticationCompleted(object sender, ResponseEventArgs e)
        {
            if (e.Success)
            {
                RaisePropertyChanged(() => User, default(User), default(User), true);
                RaisePropertyChanged(nameof(GameId));
                ShowResultOnError(await SessionManager.OpenSession());
                ShowResultOnError(await SessionManager.PingSession());
                await SessionManager.AutoPingUser();
            }
        }

        private async void ShowResultOnError(SuccessResponse response)
        {
            if (!response.Success)
            {
                await DialogService.ShowError(response.Message, "Error in web api request", "", null);
            }
        }       

        private void OnNewUrlCreated(object sender, NetworkEventArgs e)
        {
            RequestUrls.Add(string.Format("[{0}] {1}", e.CreatedAt.ToString("HH:mm:ss"), e.Url));
            RequestUrlsLastIndex++;
        }

        private void OnPingSessionCompleted(object sender, ResponseEventArgs e)
        {
            if (e.Success)
            {
                LastPinged = DateTime.Now;
                RaisePropertyChanged(nameof(Status));
            }
        }

        #endregion

        public override void Cleanup()
        {
            SessionManager.StopAutoPing();
            base.Cleanup();
        }
    }
}