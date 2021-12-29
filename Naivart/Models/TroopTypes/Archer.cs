﻿namespace Naivart.Models.TroopTypes
{
    public class Archer : TroopModel
    {
        public Archer()
        {
            GoldCost = 20;
            Type = "archer";
            Level = 1;
            Hp = 7;
            Attack = 3;
            Defense = 1;
        }
    }
}
