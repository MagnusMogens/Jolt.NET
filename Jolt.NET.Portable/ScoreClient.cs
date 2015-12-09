using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;

namespace Jolt.NET
{
    public class ScoreClient
    {
        #region Events
        
        public event EventHandler<ResponseEventArgs> FetchScoresCompleted;
        public event EventHandler<ResponseEventArgs> FetchScoreTablesCompleted;
        public event EventHandler<ResponseEventArgs> AddScoreCompleted;

        #endregion

        private string GetIdParameterString(List<int> tableIds)
        {
            if (tableIds == null && tableIds.Count == 0) throw new ArgumentException("The list cannot be empty or null!", nameof(tableIds));

            string ids = "";
            foreach (int tableId in tableIds)
            {
                if (tableIds.Count == 1 || tableIds.Last() == tableId)
                    ids += tableId;
                else
                    ids += tableId + ",";
            }

            return ids;
        }

        public async Task<IEnumerable<Score>> FetchScoresByUser(User user = null, List<int> tableIds = null, int limit = 10)
        {
            var u = user ?? Settings.Instance.CurrentUser;

            var dict = new Dictionary<RequestParameter, string>
                        {
                            { RequestParameter.GameId, Settings.Instance.GameId },
                            { RequestParameter.Username, u.Username },
                            { RequestParameter.UserToken, u.Token },
                            { RequestParameter.Limit, limit.ToString() },
                        };

            if (tableIds != null && tableIds.Count > 0)
                dict.Add(RequestParameter.TableId, GetIdParameterString(tableIds));

            var request = NetworkClient.CreateWebRequest(RequestType.Scores, dict, u);

            using (var response = await request.GetResponseAsync())
            {
                var scoreResponse = NetworkClient.EndWebRequest<ScoreResponse>(response);
                FetchScoresCompleted?.Invoke(this, new ResponseEventArgs(scoreResponse));
                return scoreResponse.Scores;
            }
        }
        
        public async Task<IEnumerable<Score>> FetchScores(List<int> tableIds = null, int limit = 10)
        {
            var dict = new Dictionary<RequestParameter, string>
                        {
                            { RequestParameter.GameId, Settings.Instance.GameId },
                            { RequestParameter.Limit, limit.ToString() },
                        };

            if (tableIds != null && tableIds.Count > 0)
                dict.Add(RequestParameter.TableId, GetIdParameterString(tableIds));

            var request = NetworkClient.CreateWebRequest(RequestType.Scores, dict);

            using (var response = await request.GetResponseAsync())
            {
                var scoreResponse = NetworkClient.EndWebRequest<ScoreResponse>(response);
                FetchScoresCompleted?.Invoke(this, new ResponseEventArgs(scoreResponse));
                return scoreResponse.Scores;
            }
        }
        
        public async Task<SuccessResponse> AddScore(string score, int sort, int tableId = -1, User user = null, string extraData = "")
        {
            if (string.IsNullOrWhiteSpace(score)) throw new ArgumentException("The parameter cannot be empty", nameof(score));

            var u = user ?? Settings.Instance.CurrentUser;

            var dict = new Dictionary<RequestParameter, string>
                                                   {
                                                       { RequestParameter.GameId, Settings.Instance.GameId },
                                                       { RequestParameter.Score, score},
                                                       { RequestParameter.Sort, sort.ToString() },
                                                       { RequestParameter.Username, u.Username },
                                                       { RequestParameter.UserToken, u.Token }
                                                   };

            if (tableId >= 0)
                dict.Add(RequestParameter.TableId, tableId.ToString());

            if (!string.IsNullOrEmpty(extraData))
                dict.Add(RequestParameter.ExtraData, extraData);

            var request = NetworkClient.CreateWebRequest(RequestType.Scores, dict, u, RequestAction.Add);

            using (var response = await request.GetResponseAsync())
            {
                var scoreResponse = NetworkClient.EndWebRequest<SuccessResponse>(response);
                AddScoreCompleted?.Invoke(this, new ResponseEventArgs(scoreResponse));
                return scoreResponse;
            }
        }
        
        public async Task<SuccessResponse> AddGuestScore(string score, int sort, string name, int tableId = -1, string extraData = "")
        {
            if (string.IsNullOrWhiteSpace(score)) throw new ArgumentException("The parameter cannot be empty", nameof(score));

            var dict = new Dictionary<RequestParameter, string>
                                                   {
                                                       { RequestParameter.GameId, Settings.Instance.GameId },
                                                       { RequestParameter.Score, score},
                                                       { RequestParameter.Sort, sort.ToString() },
                                                       {RequestParameter.Guest, name }
                                                   };

            if (tableId >= 0)
                dict.Add(RequestParameter.TableId, tableId.ToString());

            if (!string.IsNullOrEmpty(extraData))
                dict.Add(RequestParameter.ExtraData, extraData);

            var request = NetworkClient.CreateWebRequest(RequestType.Scores, dict, null, RequestAction.Add);

            using (var response = await request.GetResponseAsync())
            {
                var scoreResponse = NetworkClient.EndWebRequest<SuccessResponse>(response);
                AddScoreCompleted?.Invoke(this, new ResponseEventArgs(scoreResponse));
                return scoreResponse;
            }
        }
        
        public async Task<IEnumerable<ScoreTable>> FetchScoreTables()
        {
            var request = NetworkClient.CreateWebRequest(RequestType.Scores,
                                                         new Dictionary<RequestParameter, string>
                                                         {
                                                             { RequestParameter.GameId, Settings.Instance.GameId }
                                                         }, null, RequestAction.Tables);

            using (var response = await request.GetResponseAsync())
            {
                var scoreResponse = NetworkClient.EndWebRequest<ScoreTableResponse>(response);
                FetchScoreTablesCompleted?.Invoke(this, new ResponseEventArgs(scoreResponse));
                return scoreResponse.Scores;
            }
        }
    }
}
