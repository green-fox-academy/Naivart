namespace Naivart.Models.BuildingTypes
{
    public class Mine : BuildingModel
    {
        public Mine()
        {
            GoldCost = 100;
            RequestTownhallLevel = 1;
            Type = "mine";
            Level = 1;
            Hp = 50;
        }
    }
}
