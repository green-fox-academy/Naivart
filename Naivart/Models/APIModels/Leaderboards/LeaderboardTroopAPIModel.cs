namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardTroopAPIModel
    {
        public string Ruler { get; set; }
        public string Kingdom { get; set; }
        public int Total_defense { get; set; }
        public int Total_attack { get; set; }
        public int Points { get; set; }
    }
}
