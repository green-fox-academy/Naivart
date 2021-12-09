﻿using System.ComponentModel.DataAnnotations;

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

    }
}
