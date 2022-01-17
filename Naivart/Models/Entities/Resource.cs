namespace Naivart.Models.Entities
{
    public class Resource
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public int Generation { get; set; }
        public long UpdatedAt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }

        public Resource(string type, int amount, int generation, long updatedAt, long kingdomId)
        {
            Type = type;
            Amount = amount;
            Generation = generation;
            UpdatedAt = updatedAt;
            KingdomId = kingdomId;
        }
    }
}
