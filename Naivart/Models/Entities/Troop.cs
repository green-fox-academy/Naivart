using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Troop
    {
        [Key]
        public long Id { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public string Status { get; set; }
        public long KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
        public long TroopTypeId { get; set; }
        public TroopType TroopType { get; set; }

        public Troop(long troopTypeId, TroopType troopType)
        {
            TroopTypeId = troopTypeId;
            TroopType = troopType;
        }

        public Troop()
        {

        }
    }
}
