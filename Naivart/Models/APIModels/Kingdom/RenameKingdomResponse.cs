namespace Naivart.Models.APIModels
{
    public class RenameKingdomResponse
    {
        public long KingdomId { get; set; }
        public string KingdomName { get; set; }

        public RenameKingdomResponse(long kingdomId, string kingdomName)
        {
            KingdomId = kingdomId;
            KingdomName = kingdomName;
        }
    }
}
