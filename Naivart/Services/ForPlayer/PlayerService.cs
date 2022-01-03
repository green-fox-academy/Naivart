using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.BuildingTypes;
using Naivart.Models.Entities;
using System;
using System.Linq;
using System.Web.Helpers;

namespace Naivart.Services
{
    public class PlayerService
    {
        private readonly IMapper mapper;
        private ApplicationDbContext DbContext { get; }
        public PlayerService(IMapper mapper, ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            DbContext = dbContext;
        }

        public Player RegisterPlayer(string username, string password, string kingdomName)
        {
            if (password.Length < 8
                || String.IsNullOrWhiteSpace(username)
                || IsInDbWithThisUsername(username))
            {
                return null;
            }

            //install Microsoft.AspNet.WebPages nuget
            string salt = Crypto.GenerateSalt();
            password = password + salt;
            string hashedPassword = Crypto.HashPassword(password);

            Player player = new Player() { Username = username, Password = hashedPassword, Salt = salt };
            Kingdom kingdom = new Kingdom();

            //check if given kingdom name (and username) is not empty or already exists in database 
            kingdom.Name = !String.IsNullOrWhiteSpace(kingdomName) && FindKingdomByName(kingdomName) == null
                           ? kingdomName : $"{player.Username}'s kingdom";

            var newKingdom = DbContext.Kingdoms.Add(kingdom).Entity;
            DbContext.SaveChanges();

            var DbKingdom = FindKingdomByName(kingdom.Name);
            player.KingdomId = DbKingdom.Id;
            var newPlayer = DbContext.Players.Add(player).Entity;
            DbContext.SaveChanges();
            CreateBasicBuidlings(DbKingdom.Id); //creates basic buildings and save to Db 

            return DbContext.Players.Include(x => x.Kingdom).FirstOrDefault
                (x => x.Username == username && x.Password == hashedPassword);
        }

        public Kingdom FindKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.FirstOrDefault(x => x.Name == kingdomName);
        }

        public Player FindByUsername(string username)
        {
            return DbContext.Players.FirstOrDefault(x => x.Username == username);
        }

        public bool IsInDbWithThisUsername(string username)
        {
            return DbContext.Players.Any(x => x.Username == username);
        }

        public void DeleteByUsername(string username)
        {
            DbContext.Players.Remove(FindByUsername(username));
        }

        public Player GetPlayerById(long id)
        {
            try
            {
                return DbContext.Players.Include(p => p.Kingdom)
                                .FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public void CreateBasicBuidlings(long kingdomId)
        {
            DbContext.Buildings.Add(new Building()
            {
                Type = "townhall",
                Hp = 50,
                Level = 1,
                StartedAt = 12345789,
                FinishedAt = 12399999,
                KingdomId = kingdomId
            });
            DbContext.SaveChanges();

            var farm = new Farm();
            var kingdomFarm = mapper.Map<Building>(farm);
            kingdomFarm.KingdomId = kingdomId;
            DbContext.Buildings.Add(kingdomFarm);
            DbContext.SaveChanges();

            var mine = new Mine();
            var kingdomMine = mapper.Map<Building>(mine);
            kingdomMine.KingdomId = kingdomId;
            DbContext.Buildings.Add(kingdomMine);
            DbContext.SaveChanges();
        }
    }
}
