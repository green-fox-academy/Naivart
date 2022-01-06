namespace Naivart.Models.BuildingTypes
{
    public class Ramparts:BuildingModel
    {
        public Ramparts()
        {
            GoldCost = 2000;
            RequestTownhallLevel = 5;
            Type = "ramparts";
            Level = 1;
            Hp = 1000;
        }
    }
}
