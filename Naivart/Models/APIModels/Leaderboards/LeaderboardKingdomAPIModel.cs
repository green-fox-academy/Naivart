namespace Naivart.Models.APIModels.Leaderboards
{
    public class LeaderboardKingdomAPIModel
    {
        public string ruler { get; set; }
        public string kingdom { get; set; }
        public int buildings_points { get; set; }
        public int troops_points { get; set; }
        public int total_points { get; set; }
    }
}
