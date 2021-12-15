using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; }
        private readonly IMapper Mapper;
        public KingdomService KingdomService { get; set; }
        public AuthService AuthService { get; set; }
        public BuildingService(IMapper mapper,ApplicationDbContext dbContext, KingdomService kingdomService,AuthService authService)
        {
            DbContext = dbContext;
            Mapper = mapper;
            KingdomService = kingdomService;
            AuthService = authService;
        }

        public List<BuildingsForResponse> ListOfBuildingMapping(List<Building> buildings)
        {
            var buildingForResponses= new List<BuildingsForResponse>();

            if (buildings is null)
            {
                return buildingForResponses;
            }

            foreach (var building in buildings)
            {
                var buildingForResponse = Mapper.Map<BuildingsForResponse>(building);
                buildingForResponses.Add(buildingForResponse);
            }
            return buildingForResponses;
        }
        public bool IsPossibleAddBuilding(int townhallLevel, string buildingType)   
        {
            var model = AddBuilding(buildingType, townhallLevel);
            if (model != null)
            {
                return true;
            }
            return false;
        }
        public string AddBuilding(AddBuildingForResponse input, long kingdomId, string username, out int status)
        {
            if (KingdomService.IsUserKingdomOwner(kingdomId, username))
            {
                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                if (IsPossibleAddBuilding(TownhallLevel, input.BuildingType))
                {
                    status = 200;
                    return "ok";
                }
                status = 400;
                return "You don't have enough gold to build this building!";
            }
            status = 401;
            return "This kingdom does not belong to authenticated player";
        }
        //public BuildingModel BuildingCreate(string buildingType, int townhallLevel)
        //{
        //    switch (buildingType)
        //    {
        //        case "recruit":
        //            Buildin building = new Recruit();
        //            return building.GoldCost == goldAmount ? building : null;
        //        case "archer":
        //            TroopModel archer = new Archer();
        //            return archer.GoldCost == goldAmount ? archer : null;
        //        case "knight":
        //            TroopModel knight = new Knight();
        //            return knight.GoldCost == goldAmount ? knight : null;
        //    }
        //    return null;
        //}

    }
}
