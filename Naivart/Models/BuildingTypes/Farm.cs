namespace Naivart.Models.BuildingTypes
{
    public class Farm : BuildingModel 
    {
        public Farm()
        {
            GoldCost = 100;
            RequestTownhallLevel = 1;
            Type = "farm";
            Level = 1;
            Hp = 100;
        }
    }
}
