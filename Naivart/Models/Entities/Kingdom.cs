using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naivart.Models.Entities
{
    public class Kingdom
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public Player Player { get; set; }
        public int Population { get; set; }
        public long? LocationId { get; set; }
        public Location Location { get; set; }
        public List<Resource> Resources { get; set; }
        public List<Troop> Troops { get; set; }
        public List<Building> Buildings { get; set; }

        public Kingdom(string name)
        {
            Name = name;
        }

        public Kingdom()
        {

        }

    }
}
