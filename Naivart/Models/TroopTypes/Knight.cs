namespace Naivart.Models.TroopTypes
{
    public class Knight : TroopModel
    {
        public Knight()
        {
            GoldCost = 80;
            Type = "knight";
            Level = 1;
            Hp = 20;
            Attack = 6;
            Defense = 10;
        }
    }
}
