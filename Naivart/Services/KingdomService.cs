﻿using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naivart.Models.APIModels;
using Microsoft.EntityFrameworkCore;
using Naivart.Models.Entities;
using Naivart.Models;
using Microsoft.Extensions.Options;

namespace Naivart.Services
{
    public class KingdomService
    {
        private ApplicationDbContext DbContext { get; }
        private readonly AppSettings appSettings;
        public LoginService LoginService { get; set; }

        public KingdomService(ApplicationDbContext dbContext, IOptions<AppSettings> appSettings, LoginService loginService)
        {
            this.appSettings = appSettings.Value;
            DbContext = dbContext;
            LoginService = loginService;
        }

        public List<Kingdom> GetAll()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                kingdoms = DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .ToList();

                return kingdoms;
            }
            catch
            {
                return kingdoms;
            }
        }

        public string RegisterKingdom(KingdomLocationInput input, string authorization, out int status)
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
                    if (LoginService.IsTokenOwner(FindKingdomOwnerPlayer(input.kingdomId), authorization))
                    {
                        ConnectLocation(input);
                        status = 200;
                        return "ok";
                    }
                    status = 500;
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
    }
}
