using System.Collections.Generic;

namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardTroopsAPIResponse
    {
        public List<LeaderboardTroopAPIModel> Results { get; set; }

        public LeaderboardTroopsAPIResponse(List<LeaderboardTroopAPIModel> results)
        {
            Results = results;
        }
    }
}
