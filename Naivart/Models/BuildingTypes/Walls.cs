namespace Naivart.Models.BuildingTypes
{
    public class Walls:BuildingModel
    {
        public Walls()
        {
            GoldCost = 400;
            RequestTownhallLevel = 2;
            Type = "walls";
            Level = 1;
            Hp = 1000;
        }
    }
}
