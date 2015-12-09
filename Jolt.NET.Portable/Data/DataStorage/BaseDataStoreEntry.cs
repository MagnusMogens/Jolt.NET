using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jolt.NET.Base;
using Jolt.NET.Core;
using Jolt.NET.Interfaces;
using Jolt.NET.Network;

namespace Jolt.NET.Data.DataStorage
{
    /// <summary>
    /// The base data store entry which implements the non-generic interface.
    /// </summary>
    public class BaseDataStoreEntry : Notifieable, IDataStoreEntry
    {
        #region Fields
        
        protected string _key;
        protected User _user;

        #endregion

        #region Properties

        public object Data { get; protected set; }

        public string Key 
        { 
            get { return _key; }
            set { Set(ref _key, value); } 
        }

        /// <summary>
        /// The optional user of the data.
        /// </summary>
        public User User 
        {
            get { return _user; }
            protected set { Set(ref _user, value); }
        }

        #endregion

        #region Events

        public event EventHandler<ResponseEventArgs> RemoveDataStorageCompleted;
        public event EventHandler<ResponseEventArgs> FetchDataStorageCompleted;

        #endregion

        #region Methods

        public async Task<SuccessResponse> RemoveDataStorageEntry(User user = null)
        {
            var param = new Dictionary<RequestParameter, string>
                { 
                    { RequestParameter.GameId, Settings.Instance.GameId },
                    { RequestParameter.Key, Key },
                };

            if (user != null)
            {
                param.Add(RequestParameter.Username, user.Username);
                param.Add(RequestParameter.UserToken, user.Token);
            }

            var request = NetworkClient.CreateWebRequest(RequestType.DataStore, param, user, RequestAction.Remove);

            using (var response = await request.GetResponseAsync())
            {
                var res = NetworkClient.EndWebRequest<SuccessResponse>(response);
                RemoveDataStorageCompleted?.Invoke(this, new ResponseEventArgs(res));
                return res;
            }
        }

        public async Task<object> FetchDataStorageEntry(User user = null)
        {
            var param = new Dictionary<RequestParameter, string>
                { 
                    { RequestParameter.GameId, Settings.Instance.GameId },
                    { RequestParameter.Key, Key },
                };

            if (user != null)
            {
                param.Add(RequestParameter.Username, user.Username);
                param.Add(RequestParameter.UserToken, user.Token);
            }

            var request = NetworkClient.CreateWebRequest(RequestType.DataStore, param, user, RequestAction.Nothing, ReturnFormat.Dump);

            using (var response = await request.GetResponseAsync())
            {
                string success;
                object result;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    success = reader.ReadLine();
                    result = reader.ReadToEnd();
                }                
                FetchDataStorageCompleted?.Invoke(this, new ResponseEventArgs(success.Equals("success", StringComparison.OrdinalIgnoreCase) ? true : false));
                return result;
            }
        }

        #endregion
    }
}
