namespace Naivart.Models.BuildingTypes
{
    public class Barracks : BuildingModel
    {
        public Barracks()
        {
            GoldCost = 500;
            RequestTownhallLevel = 5;
            Type = "barracks";
            Level = 1;
            Hp = 250;
        }
    }
}
