namespace Naivart.Models.APIModels
{
    public class Location
    {
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }

        public Location(int x, int y)
        {
            CoordinateX = x;
            CoordinateY = y; 
        }
    }
}
