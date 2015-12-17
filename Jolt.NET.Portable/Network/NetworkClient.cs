using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;
using Jolt.NET.Data;
using Jolt.NET.Helper;
using Jolt.NET.Core;
using PCLCrypto;
using System.IO;
using System;

namespace Jolt.NET.Network
{
    public static class NetworkClient
    {
        #region Consts

        /// <summary>
        /// The base address to the gamejolt api.
        /// </summary>
        public const string BaseAddress = "http://gamejolt.com/api/game/";
        /// <summary>
        /// The actual api version
        /// </summary>
        public const string APIv1 = "v1";
        public const string SignatureKey = "&signature=";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the format.
        /// </summary>
        public static ReturnFormat Format { get { return ReturnFormat.Xml; } }

        #endregion

        #region Events

        public static event EventHandler<NetworkEventArgs> NewUrlCreated;

        #endregion

        #region Methods

        public static string GetRequestSignature(string requestUrl)
        {
            var bytes = Encoding.ASCII.GetBytes(requestUrl + Settings.Instance.GameKey);
            var hash = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1).HashData(bytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// Creates a valid GameJoltAPI web request.
        /// </summary>
        /// <exception cref="Exceptions.UserNotAuthenticatedException">Throws an error if the given user or the main user is not authenticated.</exception>
        public static WebRequest CreateWebRequest(
            RequestType type, 
            IDictionary<RequestParameter, string> parameters,
            User user = null,
            RequestAction action = RequestAction.Nothing,
            ReturnFormat format = ReturnFormat.Unknown)
        {
            var usedFormat = format == ReturnFormat.Unknown ? Format : format;

            // if there is a user whom is not authenticated, the request action must be 'auth'.
            if (user != null && 
                !Settings.Instance.IsAuthenticated(user) && 
                action != RequestAction.Auth)
                throw new Exceptions.UserNotAuthenticatedException(user.Username);
            
            string param = ConcatenateRequestParameters(usedFormat, parameters);
            string url = BuildUrl(type, action, usedFormat, param);
            return BuildRequest(url);
        }

        private static string ConcatenateRequestParameters(ReturnFormat format, IDictionary<RequestParameter, string> parameters)
        {
            string param = "";
            foreach (KeyValuePair<RequestParameter, string> parameter in parameters)
            {
                param += format == ReturnFormat.Unknown && parameters.First().Equals(parameter) ? "?" : "&";
                param += parameter.Key.GetDescription() + "=" + parameter.Value;
            }
            return param;
        }

        private static string BuildUrl(RequestType type, RequestAction action, ReturnFormat format, string parameters)
        {
            string url = BaseAddress.UrlCombine(APIv1, type.GetDescription(), action.GetDescription(),
                                                 (format.GetDescription() + parameters));
            url += SignatureKey + GetRequestSignature(url);
            return url;
        }

        private static WebRequest BuildRequest(string url)
        {
            var request = WebRequest.Create(url);
            NewUrlCreated?.Invoke(null, new NetworkEventArgs(url));

            request.Method = "GET";

            return request;
        }
        
        public static T EndWebRequest<T>(WebResponse response)
        {
            return DeserializeResponseStream<T>(response.GetResponseStream());
        }

        public static T DeserializeResponseStream<T>(Stream data)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(data);
        }

        #endregion
    }
}
