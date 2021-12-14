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
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }

        public KingdomService(IMapper mapper, ApplicationDbContext dbContext, AuthService authService)
        {
            DbContext = dbContext;
            AuthService = authService;
            _mapper = mapper;
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
                var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
                var locationAPIModel = _mapper.Map<LocationAPIModel>(kingdom.Location);
                kingdomAPIModel.Location = locationAPIModel;
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
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

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
            var locationAPIModel = _mapper.Map<LocationAPIModel>(kingdom.Location);
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

        public int GetGoldAmount(long kingdomId)
        {
            var model = DbContext.Kingdoms.Where(x => x.Id == kingdomId).Include(x => x.Resources).FirstOrDefault();
            return model.Resources.FirstOrDefault(x => x.Type == "gold").Amount;
        }
    }
}
