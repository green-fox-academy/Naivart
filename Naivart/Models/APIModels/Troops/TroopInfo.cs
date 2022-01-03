namespace Naivart.Models.APIModels
{
    public class TroopInfo
    {
        public long Id { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
    }
}
