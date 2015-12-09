using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;
using Jolt.NET.Helper;
using System.Threading;

namespace Jolt.NET
{
    public class SessionManager
    {
        Timer timer;
        SessionStatus autoStatus;

        #region Events
        
        public event EventHandler<ResponseEventArgs> OpenSessionCompleted;
        public event EventHandler<ResponseEventArgs> PingSessionCompleted;
        public event EventHandler<ResponseEventArgs> CloseSessionCompleted;

        #endregion
        
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
        
        public async Task<SuccessResponse> CloseSession(User user = null)
        {
            var u = user ?? Settings.Instance.CurrentUser;

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

        public void AutoPing(int seconds = 30, SessionStatus status = SessionStatus.Active)
        {
            var milliSeconds = seconds * 1000;
            autoStatus = status;

            if (timer == null)
                InitializeTimer(milliSeconds);
            else
                ChangeTimer(milliSeconds);
        }

        private void InitializeTimer(int milliSeconds)
        {
            var autoEvent = new AutoResetEvent(false);
            timer = new Timer(time, autoEvent, milliSeconds, milliSeconds);
        }

        private async void ChangeTimer(int milliSeconds)
        {
            if (milliSeconds == 0)
                timer.Dispose();
            else
            {
                await PingSession(autoStatus);
                timer.Change(milliSeconds, milliSeconds);
            }
        }

        public void EndAutoPing()
        {
            AutoPing(0);
        }

        private async void time(object stateInfo)
        {
            await PingSession(autoStatus);
        }
    }
}
