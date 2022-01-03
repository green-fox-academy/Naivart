namespace Naivart.Models.TroopTypes
{
    abstract public class TroopModel
    {
        public int GoldCost { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
    }
}
