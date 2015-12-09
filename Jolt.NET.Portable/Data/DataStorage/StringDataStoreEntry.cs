using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Helper;
using Jolt.NET.Interfaces;
using Jolt.NET.Network;

namespace Jolt.NET.Data.DataStorage
{
    public class StringDataStoreEntry : BaseDataStoreEntry, IGenericDataStoreEntry<string>
    {
        private string _data;
        
        public new string Data
        {
            get { return _data; }
            protected set { Set(ref _data, value); }
        }

        object IDataStoreEntry.Data
        {
            get { return Data; }
        }

        #region Events
        
        public event EventHandler<ResponseEventArgs> SetDataStorageCompleted;
        public event EventHandler<ResponseEventArgs> UpdateDataStorageCompleted;

        #endregion

        #region Methods

        public async Task<SuccessResponse> SetDataStorageEntry(string data, User user = null)
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

            var request = NetworkClient.CreateWebRequest(RequestType.DataStore, param, user, RequestAction.Set);

            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("data=" + data);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Content-Length"] = byteData.Length.ToString();
            using (var dataStream = await request.GetRequestStreamAsync())
            {
                dataStream.Write(byteData, 0, byteData.Length);
            }

            using (var response = await request.GetResponseAsync())
            {
                var res = NetworkClient.EndWebRequest<SuccessResponse>(response);
                SetDataStorageCompleted?.Invoke(this, new ResponseEventArgs(res));
                return res;
            }
        }

        public async Task<UpdateDataStorageResponse> UpdateDataStorageEntry(string value, DataStorageUpdateOperation updateOperation, User user = null)
        {
            if (updateOperation != DataStorageUpdateOperation.Append &&
                updateOperation != DataStorageUpdateOperation.Prepend)
            {
                throw new ArgumentException("The update operation for string can only be 'Prepend' or 'Append'.");
            }

            var param = new Dictionary<RequestParameter, string>
                { 
                    { RequestParameter.GameId, Settings.Instance.GameId },
                    { RequestParameter.Key, Key },
                    { RequestParameter.Operation, updateOperation.GetDescription() }
                };

            if (user != null)
            {
                param.Add(RequestParameter.Username, user.Username);
                param.Add(RequestParameter.UserToken, user.Token);
            }

            var request = NetworkClient.CreateWebRequest(RequestType.DataStore, param, user, RequestAction.Set);

            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("value=" + value);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Content-Length"] = byteData.Length.ToString();
            using (var dataStream = await request.GetRequestStreamAsync())
            {
                dataStream.Write(byteData, 0, byteData.Length);
            }

            using (var response = await request.GetResponseAsync())
            {
                var res = NetworkClient.EndWebRequest<UpdateDataStorageResponse>(response);
                UpdateDataStorageCompleted?.Invoke(this, new ResponseEventArgs(res));
                Data = res.Data as string;
                return res;
            }
        }

        public new async Task<string> FetchDataStorageEntry(User user = null)
        {
            var result = await base.FetchDataStorageEntry(user);
            _data = result.ToString();
            return _data;
        }

        #endregion
    }
}
