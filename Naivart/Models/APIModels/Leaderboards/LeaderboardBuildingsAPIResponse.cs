using System.Collections.Generic;

namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardBuildingsAPIResponse
    {
        public List<LeaderboardBuildingAPIModel> Results { get; set; }

        public LeaderboardBuildingsAPIResponse(List<LeaderboardBuildingAPIModel> results)
        {
            Results = results;
        }
    }
}
