using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Troop
    {
        [Key]
        public long Id { get; set; }
        public long Started_at { get; set; }
        public long Finished_at { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
        public long TroopTypeId { get; set; }
        public TroopType TroopType { get; set; }

    }
}
