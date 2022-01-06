namespace Naivart.Models.BuildingTypes
{
    public class Academy : BuildingModel
    {
        public Academy()
        {
            GoldCost = 700;
            RequestTownhallLevel = 5;
            Type = "academy";
            Level = 1;
            Hp = 200;
        }
    }
}
