namespace Naivart.Models.BuildingTypes
{
    public class Barracks : BuildingModel
    {
        public Barracks()
        {
            GoldCost = 250;
            RequestTownhallLevel = 2;
            Type = "barracks";
            Level = 1;
            Hp = 250;
        }
    }
}
