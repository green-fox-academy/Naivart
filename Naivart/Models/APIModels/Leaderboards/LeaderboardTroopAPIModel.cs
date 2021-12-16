using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardTroopAPIModel
    {
        public string ruler { get; set; }
        public string kingdom { get; set; }
        public int total_defense { get; set; }
        public int total_attack { get; set; }
        public int points { get; set; }
    }
}
