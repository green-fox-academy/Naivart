namespace Naivart.Models.APIModels
{
    public class PlayerWithKingdom
    {
        public long KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }

        public PlayerWithKingdom(long kingdomId, string kingdomName, string ruler)
        {
            KingdomId = kingdomId;
            KingdomName = kingdomName;
            Ruler = ruler;
        }
    }
}
