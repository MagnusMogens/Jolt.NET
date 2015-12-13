using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Jolt.NET.Data;
using Jolt.NET.Network;

namespace Jolt.NET.UI.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        #region Properties
        
        private GameClient Client { get; set; }
        private SessionManager SessionManager { get; set; }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        private SessionStatus _status;
        public SessionStatus Status
        {
            get { return _status; }
            set { Set(() => Status, ref _status, value); }
        }

        #endregion

        public UserViewModel(GameClient client, SessionManager sessionManager)
        {
            Client = client;
            Client.AuthenticationCompleted += OnAuthenticationComleted;

            SessionManager = sessionManager;

            SetSessionStatusCommand = new RelayCommand(SetSessionStatus);
        }

        #region Methods

        private void OnAuthenticationComleted(object sender, ResponseEventArgs e)
        {
            if (e.Success)
                RaisePropertyChanged(() => User, default(User), default(User), true);
        }

        #endregion

        #region Commands

        public RelayCommand SetSessionStatusCommand { get; set; }
        private void SetSessionStatus()
        {
            SessionManager.AutoPingUser(Status);
        }

        #endregion
    }
}
