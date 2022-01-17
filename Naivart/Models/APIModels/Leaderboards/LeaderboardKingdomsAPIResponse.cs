using System.Collections.Generic;

namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardKingdomsAPIResponse
    {
        public List<LeaderboardKingdomAPIModel> Results { get; set; }

        public LeaderboardKingdomsAPIResponse(List<LeaderboardKingdomAPIModel> results)
        {
            Results = results;
        }
    }
}
