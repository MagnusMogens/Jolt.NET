using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;

namespace Jolt.NET
{
    public class GameClient
    {
        #region Constructor
        
        public GameClient()
        { }
        
        public GameClient(string gameId, string gameKey)
            : this()
        {
            RegisterGame(gameId, gameKey);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is called when the authentication process is completed.
        /// </summary>
        public event EventHandler<ResponseEventArgs> AuthenticationCompleted;

        /// <summary>
        /// Occurs when [on fetch user complete].
        /// </summary>
        public event EventHandler<ResponseEventArgs> FetchUserCompleted;

        #endregion

        #region Methods
        
        public void RegisterGame(string gameId, string gameKey)
        {
            if (string.IsNullOrWhiteSpace(gameId)) throw new ArgumentException("The parameter must be filled!", nameof(gameId));
            if (string.IsNullOrWhiteSpace(gameKey)) throw new ArgumentException("The parameter must be filled!", nameof(gameKey));

            Settings.Instance.GameId = gameId;
            Settings.Instance.GameKey = gameKey;
        }
        
        public async Task<SuccessResponse> AuthenticateUser(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("The parameter must be filled!", nameof(username));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("The parameter must be filled!", nameof(token));

            var request = NetworkClient.CreateWebRequest(RequestType.Users,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId },
                                                             { RequestParameter.Username, username },
                                                             { RequestParameter.UserToken, token }
                                                         }, null, RequestAction.Auth);

            using (var response = await request.GetResponseAsync())
            {
                var userResponse = NetworkClient.EndWebRequest<SuccessResponse>(response);

                if (userResponse.Success)
                {
                    await AddAuthenticatedUser(username, token);

                    if (Settings.Instance.CurrentUser == null)
                    {
                        Settings.Instance.SetCurrentUser(username);
                    }
                }

                AuthenticationCompleted?.Invoke(this, new ResponseEventArgs(userResponse));
                return userResponse;
            }            
        }

        private async Task AddAuthenticatedUser(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("The parameter must be filled!", nameof(username));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("The parameter must be filled!", nameof(token));

            var user = new User { IsAuthenticated = true, Username = username, Token = token };

            // Only add user if not already in list.
            if (!Settings.Instance.AuthenticatedUsers.Any(x => x == user))
                Settings.Instance.AuthenticatedUsers.Add(user);

            // Fetch authenticated user data.
            var userRes = await FetchUserByName(username);
            var finalUser = userRes;
            finalUser.Token = user.Token;
            finalUser.IsAuthenticated = user.IsAuthenticated;

            // Complete user data.
            var index = Settings.Instance.AuthenticatedUsers.IndexOf(Settings.Instance.AuthenticatedUsers.Single(x => x == user));
            Settings.Instance.AuthenticatedUsers[index] = finalUser;
        }
        
        public async Task<IEnumerable<User>> FetchUser(params string[] userIds)
        {
            if (userIds == null || userIds.Count() == 0) throw new ArgumentException("At least one user id must be given.", nameof(userIds));

            string idList = "";
            foreach (string userId in userIds)
            {
                if (userIds.Count() == 1 || userIds.Last() == userId)
                    idList += userId;
                else
                    idList += userId + ",";
            }

            var request = NetworkClient.CreateWebRequest(RequestType.Users,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId },
                                                             { RequestParameter.UserId, idList }
                                                         });

            using (var response = await request.GetResponseAsync())
            {
                var userResponse = NetworkClient.EndWebRequest<UserResponse>(response);
                FetchUserCompleted?.Invoke(this, new ResponseEventArgs(userResponse));
                return userResponse.Users;
            }
        }
        
        public async Task<User> FetchUserByName(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("The username cannot be empty!", nameof(username));

            var request = NetworkClient.CreateWebRequest(RequestType.Users,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId },
                                                             { RequestParameter.Username, username }
                                                         });

            using (var response = await request.GetResponseAsync())
            {
                var userResponse = NetworkClient.EndWebRequest<UserResponse>(response);
                FetchUserCompleted?.Invoke(this, new ResponseEventArgs(userResponse));
                return userResponse.Users.FirstOrDefault();
            }
        }

        #endregion
    }    
}
