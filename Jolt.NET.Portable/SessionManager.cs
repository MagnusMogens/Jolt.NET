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

        #endregion

        #region Events

        public event EventHandler<ResponseEventArgs> OpenSessionCompleted;
        public event EventHandler<ResponseEventArgs> PingSessionCompleted;
        public event EventHandler<ResponseEventArgs> CloseSessionCompleted;

        #endregion

        public SessionManager()
        {
            UserStatus = new Dictionary<User, SessionStatus>();
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

        public async Task AutoPing(int seconds = 30, SessionStatus status = SessionStatus.Active, User user = null)
        {
            var milliSeconds = seconds * 1000;
            var u = user ?? Settings.Instance.CurrentUser;
            UserStatus[u] = status;

            if (timer == null)
                InitializeTimer(milliSeconds);
            else
                await ChangeTimer(milliSeconds);
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
                await PingSessions();
                timer.Change(milliSeconds, milliSeconds);
            }
        }

        public async Task EndAutoPing(User user = null)
        {
            if (UserStatus.ContainsKey(user))
            {
                UserStatus.Remove(user);
            }
            if (!UserStatus.Any())
                await AutoPing(0);
        }

        public async Task EndAutoPings()
        {
            foreach (var item in UserStatus)
            {
                await EndAutoPing(item.Key);
            }
        }

        private async void time(object stateInfo)
        {
            await PingSessions();
        }
    }
}
