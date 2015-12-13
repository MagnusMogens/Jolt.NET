using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;
using Jolt.NET.Helper;
using System.Threading;
using System.Linq;

namespace Jolt.NET
{
    public class SessionManager
    {
        #region Properties

        private Timer timer { get; set; }
        public Dictionary<User, SessionStatus> UserStatus { get; set; }
        private List<User> AutoPingedUser { get; set; }

        #endregion

        #region Events

        public event EventHandler<ResponseEventArgs> OpenSessionCompleted;
        public event EventHandler<ResponseEventArgs> PingSessionCompleted;
        public event EventHandler<ResponseEventArgs> CloseSessionCompleted;

        #endregion

        public SessionManager()
        {
            UserStatus = new Dictionary<User, SessionStatus>();
            AutoPingedUser = new List<User>();
        }
        
        public async Task<SuccessResponse> OpenSession(User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;
            var request = NetworkClient.CreateWebRequest(RequestType.Sessions,
                                                      new Dictionary<RequestParameter, string>
                                                      {
                                                          { RequestParameter.GameId, Settings.Instance.GameId },
                                                          { RequestParameter.Username, u.Username },
                                                          { RequestParameter.UserToken, u.Token }
                                                      }, u, RequestAction.Open);

            using (var response = await request.GetResponseAsync())
            {
                var sessionResponse = NetworkClient
                                        .EndWebRequest<SuccessResponse>(response);
                OpenSessionCompleted?.Invoke(this, new ResponseEventArgs(sessionResponse));
                return sessionResponse;
            }
        }
        
        public async Task<SuccessResponse> PingSession(SessionStatus status = SessionStatus.Active, User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;
            UserStatus[u] = status;

            var request = NetworkClient.CreateWebRequest(RequestType.Sessions,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId },
                                                             { RequestParameter.Username, u.Username },
                                                             { RequestParameter.UserToken, u.Token },
                                                             { RequestParameter.Status, status.GetDescription() }
                                                         }, u , RequestAction.Ping);

            using (var response = await request.GetResponseAsync())
            {
                var sessionResponse = NetworkClient.EndWebRequest<SuccessResponse>(response);
                PingSessionCompleted?.Invoke(this, new ResponseEventArgs(sessionResponse));
                return sessionResponse;
            }
        }

        public async Task PingSessions()
        {
            foreach (var item in UserStatus)
            {
                await PingSession(item.Value, item.Key);
            }
        }

        private async Task AutoPingSessions()
        {
            foreach (var item in AutoPingedUser)
            {
                await PingSession(UserStatus[item], item);
            }
        }
        
        public SessionStatus GetSessionStatus(User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;
            if (u == null) return SessionStatus.Idle;

            var status = UserStatus.ContainsKey(u) ? UserStatus[u] : SessionStatus.Idle;
            return status;
        }

        public async Task<SuccessResponse> CloseSession(User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;
            if (UserStatus.ContainsKey(u)) UserStatus.Remove(u);
            await StopAutoPingUser(u);

            var request = NetworkClient.CreateWebRequest(RequestType.Sessions,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId },
                                                             { RequestParameter.Username, u.Username },
                                                             { RequestParameter.UserToken, u.Token }
                                                         }, u, RequestAction.Close);

            using (var response = await request.GetResponseAsync())
            {
                var sessionResponse = NetworkClient.EndWebRequest<SuccessResponse>(response);
                CloseSessionCompleted?.Invoke(this, new ResponseEventArgs(sessionResponse));
                return sessionResponse;
            }
        }

        public void AutoPingUser(SessionStatus status = SessionStatus.Active, User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;
            if (!AutoPingedUser.Contains(u)) AutoPingedUser.Add(u);
            UserStatus[u] = status;

            if (timer == null)
                // Ping the user status every 30 seconds.
                InitializeTimer(30 * 1000);
        }

        public async Task SetPingPeriod(int period)
        {
            if (timer == null)
                InitializeTimer(30 * 1000);
            else
                await ChangeTimer(period * 1000);
        }

        public async Task StopAutoPingUser(User user = null)
        {
            if (AutoPingedUser.Contains(user))
            {
                AutoPingedUser.Remove(user);
            }
            if (!AutoPingedUser.Any())
                await StopAutoPing();
        }

        public void StartAutoPing(int period)
        {
            if (timer == null)
                InitializeTimer(30 * 1000);
        }

        public async Task StopAutoPing()
        {
            await ChangeTimer(0);
        }

        private void InitializeTimer(int milliSeconds)
        {
            timer = new Timer(time, new AutoResetEvent(false), milliSeconds, milliSeconds);
        }

        private async Task ChangeTimer(int milliSeconds)
        {
            if (milliSeconds == 0)
                timer.Dispose();
            else
            {
                await AutoPingSessions();
                timer.Change(milliSeconds, milliSeconds);
            }
        }               

        private async void time(object stateInfo)
        {
            await AutoPingSessions();
        }
    }
}
