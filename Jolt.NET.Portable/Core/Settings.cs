using System;
using System.Collections.Generic;
using System.Linq;
using Jolt.NET.Base;
using Jolt.NET.Exceptions;
using Jolt.NET.Data;

namespace Jolt.NET.Core
{
    public class Settings : Notifieable
    {
        #region Singleton implementation

        private static readonly Lazy<Settings> _instance
            = new Lazy<Settings>(() => new Settings());

        public static Settings Instance { get { return _instance.Value; } }

        private Settings()
        {
            _users = new List<User>();
        }

        #endregion

        #region Properties

        private List<User> _users;

        private string _gameId;
        private string _gameKey;
        private User _currentUser;

        /// <summary>
        /// The Game ID is a unique identifier that is used to identify the game which
        /// is using the API. The ID is fund in the dashboard under "Manage Achievements".
        /// </summary>
        public string GameId
        {
            get { return _gameId; }
            set { Set(ref _gameId, value); }
        }

        /// <summary>
        /// The Game Key is a private key found by your Game ID. It is used to generate the signature
        /// which validates the API requests. The Game Key should be placed in an save environment, because
        /// it could be used to hack the game.
        /// </summary>
        public string GameKey
        {
            get { return _gameKey; }
            set { Set(ref _gameKey, value); }
        }

        public User CurrentUser
        {
            get { return _currentUser; }
            set { Set(ref _currentUser, value); }
        }

        public List<User> AuthenticatedUsers
        {
            get { return _users; }
            set { Set(ref _users, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the current user. The username must be found in the list of authenticated
        /// users.
        /// </summary>
        public void SetCurrentUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new UserNotAuthenticatedException(username, "The username cannot be empty.");

            var user = AuthenticatedUsers.SingleOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new UserNotAuthenticatedException(username, "The user cannot be found.");

            if (string.IsNullOrEmpty(user.Token))
                throw new UserNotAuthenticatedException(username);

            CurrentUser = user;
        }

        public User GetAuthenticatedUser(string username = "")
        {
            if (string.IsNullOrEmpty(username))
                return CurrentUser;
            return AuthenticatedUsers.Single(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <param name="user">The user (if empty the main user will be used).</param>
        public bool IsAuthenticated(User user = null)
        {
            User cUser = user ?? CurrentUser;

            return AuthenticatedUsers != null &&
                   AuthenticatedUsers.Any() &&
                   AuthenticatedUsers.Find(x => x == cUser) != null &&
                   AuthenticatedUsers.Find(x => x == cUser).IsAuthenticated;
        }

        #endregion
    }
}
