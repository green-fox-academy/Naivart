using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Building
    {
        [Key]
        public long Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
        public long BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; }
    }
}
