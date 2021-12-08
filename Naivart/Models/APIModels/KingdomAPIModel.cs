using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    //[JsonObject]
    public class KingdomAPIModel
    {
        [JsonPropertyName("kingdom_id")]
        public long Id { get; set; }
        [JsonPropertyName("kingdomname")]
        public string Name { get; set; }
        [JsonPropertyName("ruler")]
        public string PlayerUsername { get; set; }
        public int Population { get; set; }
        [JsonPropertyName("location")]
        public Dictionary<string, int> LocationCoordinates { get; set; }
    }
}
