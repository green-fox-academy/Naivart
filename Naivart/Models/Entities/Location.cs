using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Location
    {
        [Key]
        public long Id { get; set; }
        public Kingdom Kingdom { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
    }
}
