namespace Naivart.Models.APIModels
{
    public class BuildingRequest
    {
        public string Type { get; set; }

        public BuildingRequest(string type)
        {
            Type = type;
        }
    }
}
