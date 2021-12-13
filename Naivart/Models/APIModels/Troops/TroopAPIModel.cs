﻿using System;

namespace Naivart.Models.APIModels
{
    public class TroopAPIModel
    {
        public long Id { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public DateTime? Started_at { get; set; }
        public DateTime? Finished_at { get; set; }
    }
}
