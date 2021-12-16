
namespace Naivart.Models.APIModels
{
    public class TroopsInfo
    {
        public long Id { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public long Started_at { get; set; }
        public long Finished_at { get; set; }
    }
}
