using System.Text.Json.Serialization;

namespace Naivart.Models.APIModels
{
    public class KingdomAPIModel
    {
        [JsonPropertyName("kingdom_id")]
        public long Kingdom_Id { get; set; }
        [JsonPropertyName("kingdomname")]
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public LocationAPIModel Location { get; set; }
    }
}
