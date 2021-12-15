using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Leaderboards;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class BuildingService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public KingdomService KingdomService { get; set; }
        public PlayerService PlayerService { get; set; }
        public BuildingService(ApplicationDbContext dbContext, KingdomService kingdomService, PlayerService playerService,
                                IMapper mapper)
        {
            DbContext = dbContext;
            KingdomService = kingdomService;
            PlayerService = playerService;
            this.mapper = mapper;
        }

        public KingdomResponseForBuilding GetKingdom(long id)
        {
            var kingdom = KingdomService.GetByIdWithBuilding(id);
            var player = PlayerService.GetPlayerById(kingdom.Player.Id);
            var location = GetLocations(kingdom);
            if (kingdom != null && player != null)
            {
                return new KingdomResponseForBuilding()
                {
                    KingdomId = kingdom.Id,
                    KingdomName = kingdom.Name,
                    Ruler = player.Username,
                    Population = 0,
                    Location = location
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
                Id = b.Id,
                Type = b.Type,
                Level = b.Level,
                StartedAt = b.StartedAt,
                FinishedAt = b.FinishedAt
            }).ToList();

            return buildings != null ? buildings : new List<BuildingsForResponse>();
        }

        public BuildingResponse GetBuildingResponse(long id, string usernameToken, out int status)
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
        public LocationAPIModel GetLocations(Kingdom kingdom)
        {
            return new LocationAPIModel() { CoordinateX = kingdom.Location.CoordinateX, CoordinateY = kingdom.Location.CoordinateY };
        }

        public List<LeaderboardBuildingAPIModel> GetBuildingLeaderboards(out int status, out string error)
        {
            try
            {
                var AllKingdoms = DbContext.Kingdoms.Include(k => k.Player)
                                                    .Include(k => k.Buildings)
                                                    .OrderByDescending(x => x.Buildings.Sum(a => a.Level))
                                                    .ToList();
                if (AllKingdoms.Count() > 0)
                {
                    var BuildingsLeaderboard = new List<LeaderboardBuildingAPIModel>();
                    foreach (var kingdom in AllKingdoms)
                    {
                        var buildingMapper = mapper.Map<LeaderboardBuildingAPIModel>(kingdom);
                        BuildingsLeaderboard.Add(buildingMapper);
                    }
                    error = "ok";
                    status = 200;
                    return BuildingsLeaderboard;
                }
                else
                {
                    error = "There are no kingdoms in Leaderboard";
                    status = 404;
                    return null;
                }
            }
            catch (Exception)
            {
                error = "Data could not be read";
                status = 500;
                return null;
            }
        }
    }
}
