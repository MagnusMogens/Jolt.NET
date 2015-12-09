using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;
using Jolt.NET.Data.DataStorage;

namespace Jolt.NET
{
    public class DataStorageClient
    {
        #region Events
        
        public event EventHandler<ResponseEventArgs> FetchDataStorageKeysCompleted;

        #endregion
        
        public async Task<IEnumerable<DataStorageKey>> GetDataStorageKeys(User user = null)
        {
            var param = new Dictionary<RequestParameter, string>
                { 
                    { RequestParameter.GameId, Settings.Instance.GameId }
                };

            if (user != null)
            {
                param.Add(RequestParameter.Username, user.Username);
                param.Add(RequestParameter.UserToken, user.Token);
            }

            var request = NetworkClient.CreateWebRequest(RequestType.DataStore, param, user, RequestAction.GetKeys);

            using (var response = await request.GetResponseAsync())
            {
                var keys = NetworkClient.EndWebRequest<GetKeysResponse>(response);
                FetchDataStorageKeysCompleted?.Invoke(this, new ResponseEventArgs(keys));
                return keys.Keys;
            }
        }
    }
}
