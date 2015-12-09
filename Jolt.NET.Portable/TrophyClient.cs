using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jolt.NET.Core;
using Jolt.NET.Network;
using Jolt.NET.Data;

namespace Jolt.NET
{
	public class TrophyClient
	{
		#region Events
		
		public event EventHandler<ResponseEventArgs> FetchTrophyCompleted;
		public event EventHandler<ResponseEventArgs> AchieveTrophyCompleted;

		#endregion

		private async Task<IEnumerable<Trophy>> FetchTrophy(User user = null, bool? achieved = null, params int[] trophyIds)
		{
			var u = user ?? Settings.Instance.CurrentUser;

			Dictionary<RequestParameter, string> parameters = new Dictionary<RequestParameter, string>
				{
					{ RequestParameter.GameId, Settings.Instance.GameId },
					{ RequestParameter.Username, u.Username },
					{ RequestParameter.UserToken, u.Token }
				};

			if (achieved != null)
				parameters.Add(RequestParameter.Achieved, achieved.ToString().ToLower());

			if (trophyIds != null && trophyIds.Count() > 0)
			{
				string trophyList = "";
				foreach (int trophyId in trophyIds)
				{
					if (trophyIds.Count() == 1 || trophyIds.Last() == trophyId)
						trophyList += trophyId.ToString();
					else
						trophyList += trophyId + ",";
				}
				parameters.Add(RequestParameter.TrophyId, trophyList);
			}

			var request = NetworkClient.CreateWebRequest(RequestType.Trophies, parameters, u);

			using (var response = await request.GetResponseAsync())
			{
				var trophies = NetworkClient.EndWebRequest<TrophyResponse>(response);
				FetchTrophyCompleted?.Invoke(this, new ResponseEventArgs(trophies));
				return trophies.Trophies;
			}
		}
		
		public async Task<IEnumerable<Trophy>> FetchAchievedTrophies(User user = null)
		{
			return await FetchTrophy(user, true);
		}
		
		public async Task<IEnumerable<Trophy>> FetchNonAchievedTrophies(User user = null)
		{
			return await FetchTrophy(user, false);
		}
		
		public async Task<IEnumerable<Trophy>> FetchAllTrophies(User user = null)
		{
			return await FetchTrophy(user);
		}
		
		public async Task<IEnumerable<Trophy>> FetchTrophyById(params int[] trophyIds)
		{
			return await FetchTrophyById(null, trophyIds);
		}
		
		public async Task<IEnumerable<Trophy>> FetchTrophyById(User user, params int[] trophyIds)
		{
			return await FetchTrophy(user, null, trophyIds);
		}
		
		public async Task<SuccessResponse> AchieveTrophy(int trophyId, User user = null)
		{
			if (trophyId < 1) throw new ArgumentException("The parameter must be 1 or higher.", nameof(trophyId));

			var u = user ?? Settings.Instance.CurrentUser;

			var request = NetworkClient.CreateWebRequest(RequestType.Trophies,
														 new Dictionary<RequestParameter, string> 
														 {
															 { RequestParameter.GameId, Settings.Instance.GameId },
															 { RequestParameter.Username, u.Username },
															 { RequestParameter.UserToken, u.Token },
															 { RequestParameter.TrophyId, trophyId.ToString() }
														 }, u, RequestAction.AddAchieved);

			using (var response = await request.GetResponseAsync())
			{
				var trophyAchieved = NetworkClient.EndWebRequest<SuccessResponse>(response);
				AchieveTrophyCompleted?.Invoke(this, new ResponseEventArgs(trophyAchieved));
				return trophyAchieved;
			}
		}
	}
}
