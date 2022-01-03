namespace Naivart.Models.BuildingTypes
{
    public class Farm : BuildingModel 
    {
        public Farm()
        {
            GoldCost = 300;
            RequestTownhallLevel = 1;
            Type = "farm";
            Level = 1;
            Hp = 50;
        }
    }
}
