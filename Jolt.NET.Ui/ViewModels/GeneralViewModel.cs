using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.Data;
using Jolt.NET.UI.Common;

namespace Jolt.NET.UI.ViewModels
{
    public class GeneralViewModel : ViewModelBase 
    {
        #region Properties

        private IDialogService DialogService { get; set; }
        private GameClient Client { get; set; }

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

        #endregion

        public GeneralViewModel(IDialogService dialogSerivce, GameClient client)
        {
            DialogService = dialogSerivce;
            Client = client;

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
        }

        #region Methods

        private async Task ShowResultOnError(SuccessResponse response)
        {
            if (!response.Success)
            {
                await DialogService.ShowError(response.Message, "Error in web api request", "", null);
            }
        }

        #endregion

        #region Commands

        public AutoRelayCommand RegisterGameCommand { get; set; }
        private void RegisterGame()
        {
            Client.RegisterGame(GameId, GameKey);
        }
        private bool CanRegisterGame()
        {
            return !string.IsNullOrEmpty(GameId)
                && !string.IsNullOrEmpty(GameKey);
        }

        public AutoRelayCommand AuthenticateUserCommand { get; set; }
        private async void AuthenticateUser()
        {
            await ShowResultOnError(await Client.AuthenticateUser(UserName, Token));
        }
        private bool CanAuthenticateUser()
        {
            return !string.IsNullOrEmpty(UserName)
                && !string.IsNullOrEmpty(Token);
        }

        #endregion

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
