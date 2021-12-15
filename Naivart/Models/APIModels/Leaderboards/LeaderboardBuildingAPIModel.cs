using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardBuildingAPIModel
    {
        public string ruler { get; set; }
        public string kingdom { get; set; }
        public int buildings { get; set; }
        public int points { get; set; }
    }
}
