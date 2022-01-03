namespace Naivart.Models.APIModels
{
    public class AddBuildingResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
    }
}
