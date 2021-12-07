using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.Entities
{
    public class Location
    {
        [Key]
        public long Id { get; set; }
        public Kingdom Kingdom { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }

        public Dictionary<string, int> GetCoordinates()
        {
            Dictionary<string, int> coordinates = new Dictionary<string, int>()
            {
                { "coordinateX", CoordinateX },
                { "coordinateY", CoordinateY }
            };

            return coordinates;
        }
    }
}
