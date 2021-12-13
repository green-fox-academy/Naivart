using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public BuildingService(ApplicationDbContext dbContext, KingdomService kingdomService,PlayerService playerService)
        {
            DbContext = dbContext;
            KingdomService = kingdomService;
            PlayerService = playerService;
        }
          public KingdomResponseForBuilding GetKingdom(long id)
        {
            var kingdom = KingdomService.GetById(id);
            var player = PlayerService.GetPlayerById(kingdom.Player.Id);
            if (kingdom != null && player != null)
            {
                return new KingdomResponseForBuilding()
                {
                    KingdomId = kingdom.Id,
                    KingdomName = kingdom.Name,
                    Ruler = player.Username,
                    Population = 0,
                    Location = new Location() { CoordinateX = kingdom.Location.CoordinateX, CoordinateY = kingdom.Location.CoordinateY }
                };
            }
            else
            {
                return null;
            }
         
        }
        public List<BuildingsForResponse> GetBuildingsById(long id)
        {
            List<BuildingsForResponse> buildings = DbContext.Buildings.Where(b => b.KingdomId == id).Select(b => new BuildingsForResponse()
            {
                id = b.Id,
                type = b.Type,
                level = b.Level,
                startedAt = b.StartedAt,
                finishedAt = b.FinishedAt
            }).ToList();

            if (buildings != null)
            {
                return buildings;
            }
            else
            {
                return new List<BuildingsForResponse>();
            }
        }
      
        public BuildingResponse GetBuildingResponse(long id,string usernameToken, out int status)
        {
            BuildingResponse response = new BuildingResponse();
            if (KingdomService.IsUserKingdomOwner(id, usernameToken))
            {
                response.Kingdom = GetKingdom(id);
                response.Buildings = GetBuildingsById(id);
                status = 200;
                return response;
            }
            else
            {
                status = 401;
                return response;
            }
        }
    }
}
