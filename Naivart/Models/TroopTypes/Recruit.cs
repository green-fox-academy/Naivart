﻿namespace Naivart.Models.TroopTypes
{
    public class Recruit : TroopModel
    {
        public Recruit()
        {
            GoldCost = 10;
            Type = "recruit";
            Level = 1;
            Hp = 10;
            Attack = 1;
            Defense = 2;
        }
    }
}
