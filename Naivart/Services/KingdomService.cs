using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class KingdomService
    {
        private readonly IMapper mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public LoginService LoginService { get; set; }
        public KingdomService(ApplicationDbContext dbContext, LoginService loginService, IMapper mapper, AuthService authService)
        {
            DbContext = dbContext;
            this.mapper = mapper;
            LoginService = loginService;
            AuthService = authService;
        }

        public List<Kingdom> GetAll()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return kingdoms = DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .ToList();
            }
            catch
            {
                return kingdoms;
            }
        }

        public List<KingdomAPIModel> ListOfKingdomsMapping(List<Kingdom> kingdoms)
        {
            var kingdomAPIModels = new List<KingdomAPIModel>();

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = mapper.Map<KingdomAPIModel>(kingdom);
                var locationAPIModel = mapper.Map<LocationAPIModel>(kingdom.Location);
                kingdomAPIModel.Location = locationAPIModel;
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
        }

        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                return kingdom = DbContext.Kingdoms.Where(k => k.Id == id).FirstOrDefault();
            }
            catch
            {
                return kingdom;
            }
        }

        public Kingdom GetByIdWithResources(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                return kingdom = DbContext.Kingdoms
                     .Where(k => k.Id == id)
                     .Include(k => k.Player)
                     .Include(k => k.Location)
                     .Include(k => k.Resources)
                     .First();
            }
            catch
            {
                return kingdom;
            }
        }

        public Kingdom GetByIdWithTroops(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                kingdom = DbContext.Kingdoms
                     .Where(k => k.Id == id)
                     .Include(k => k.Player)
                     .Include(k => k.Location)
                     .Include(k => k.Troops)
                     .FirstOrDefault();
                return kingdom;
            }
            catch
            {
                return kingdom;
            }
        }

        public void RenameKingdom(long KingdomId, string NewKingdomName)
        {
            Kingdom kingdom = GetById(KingdomId);
            kingdom.Name = NewKingdomName;
            DbContext.Update(kingdom);
            DbContext.SaveChanges();
        }

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = mapper.Map<KingdomAPIModel>(kingdom);
            var locationAPIModel = mapper.Map<LocationAPIModel>(kingdom.Location);
            kingdomAPIModel.Location = locationAPIModel;
            return kingdomAPIModel;
        }

        public string RegisterKingdom(KingdomLocationInput input, string usernameToken, out int status)
        {
            try
            {
                status = 400;
                if (!IsCorrectLocationRange(input))
                {
                    return "One or both coordinates are out of valid range (0-99).";
                }
                else if (IsLocationTaken(input))
                {
                    return "Given coordinates are already taken!";
                }
                else if (HasAlreadyLocation(input))
                {
                    if (IsUserKingdomOwner(input.kingdomId, usernameToken))
                    {
                        ConnectLocation(input);
                        status = 200;
                        return "ok";
                    }
                    status = 401;
                    return "Wrong user authentication!";
                }
                status = 409;
                return "Your kingdom already have location!";
            }
            catch (Exception)
            {
                status = 500;
                return "Data could not be read";
            }
        }

        public bool IsCorrectLocationRange(KingdomLocationInput input)
        {
            return input.coordinateX >= 0 && input.coordinateX <= 99 && input.coordinateY >= 0 && input.coordinateY <= 99;
        }

        public bool IsLocationTaken(KingdomLocationInput input)
        {
            try
            {
                return DbContext.Locations.Any(x => x.CoordinateX == input.coordinateX && x.CoordinateY == input.coordinateY);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public bool HasAlreadyLocation(KingdomLocationInput input)
        {
            try
            {
                return DbContext.Kingdoms.Any(x => x.Id == input.kingdomId && x.LocationId == null);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public void ConnectLocation(KingdomLocationInput input)
        {
            try
            {
                var model = new Location() { CoordinateX = input.coordinateX, CoordinateY = input.coordinateY };
                DbContext.Locations.Add(model);
                DbContext.SaveChanges();
                long locationId = DbContext.Locations.FirstOrDefault(x => x.CoordinateX == model.CoordinateX && x.CoordinateY == model.CoordinateY).Id;
                DbContext.Kingdoms.FirstOrDefault(x => x.Id == input.kingdomId).LocationId = locationId;
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public string FindKingdomOwnerPlayer(long kingdomId)
        {
            try
            {
                return DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId).Username;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }

        public AllPlayerDetails GetKingdomInfo(long kingdomId, string tokenUsername, out int status, out string error)
        {
            try
            {
                if (IsUserKingdomOwner(kingdomId, tokenUsername))  
                {
                    error = "ok";
                    status = 200;
                    return GetAllInfoAboutKingdom(kingdomId);      //this will use automapper to create object
                }
                else
                {
                    error = "This kingdom does not belong to authenticated player";
                    status = 401;
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

        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                kingdom = DbContext.Kingdoms.Include(k => k.Player).Include(k => k.Location).Include(k => k.Buildings).FirstOrDefault(k => k.Id == id);
                return kingdom;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public bool IsUserKingdomOwner(long kingdomId, string username)
        {
            try
            {
                return DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId).Username == username;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public Kingdom GetByIdWithBuilding(long id)
        {
            var kingdom = new Kingdom();
            try
            {
                kingdom = DbContext.Kingdoms.Include(k => k.Player).Include(k => k.Location).Include(k => k.Buildings).FirstOrDefault(k => k.Id == id);
                return kingdom;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public AllPlayerDetails GetAllInfoAboutKingdom(long kingdomId)
        {
            try
            {
                var model = DbContext.Kingdoms.Where(x => x.Id == kingdomId)
                                  .Include(x => x.Location)
                                  .Include(x => x.Resources)
                                  .Include(x => x.Buildings)
                                  .Include(x => x.Troops).FirstOrDefault();

                var buildings = new List<BuildingsInfo>();
                var resources = new List<ResourceAPIModel>();
                var troops = new List<TroopsInfo>();

                foreach (var item in model.Buildings)   //Creating list of buildingsInfo
                {
                    var buildingMapper = mapper.Map<BuildingsInfo>(item);
                    buildings.Add(buildingMapper);
                }
                foreach (var item in model.Resources)   //list of resources
                {
                    var resourcesMapper = mapper.Map<ResourceAPIModel>(item);
                    resources.Add(resourcesMapper);
                }
                foreach (var item in model.Troops)   //list of troops
                {
                    var troopsMapper = mapper.Map<TroopsInfo>(item);
                    troops.Add(troopsMapper);
                }

                var result = new AllPlayerDetails()
                {
                    Kingdom = mapper.Map<KingdomAPIModel>(model),
                    Buildings = buildings,
                    Resources = resources,
                    Troops = troops
                };
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e); ;
            }
        }
    }
}
