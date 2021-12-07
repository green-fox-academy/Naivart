using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class PlayerService
    {
        private ApplicationDbContext DbContext { get; }
        public PlayerService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Player RegisterPlayer(string username, string password, string kingdomName)
        {
                if (password.Length >= 8 
                    && !String.IsNullOrWhiteSpace(username) 
                    && FindPlayerByUsername(username) == null)
                {
                    Player player = new Player() { Username = username, Password = password };
                    Kingdom kingdom = new Kingdom();

                    //check if given kingdom name(and username) is not empty or already exists in database 
                    if (!String.IsNullOrWhiteSpace(kingdomName) && FindKingdomByName(kingdomName) == null)
                    {
                        kingdom.Name = kingdomName;
                    }
                    else
                    {
                        kingdom.Name = $"{player.Username}'s kingdom";
                    }
                    var newKingdom = DbContext.Kingdoms.Add(kingdom).Entity;
                    DbContext.SaveChanges();

                    var DbKingdom = FindKingdomByName(kingdom.Name);
                    player.KingdomId = DbKingdom.Id;
                    var newPlayer = DbContext.Players.Add(player).Entity;
                    DbContext.SaveChanges();

                    return DbContext.Players.Include(x=>x.Kingdom).FirstOrDefault(x => x.Username == username && x.Password == password);
                }
                else
                {
                    return null;
                }
        }

        public Kingdom FindKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.FirstOrDefault(x => x.Name == kingdomName);
        }
        public Player FindPlayerByUsername(string username)
        {
            return DbContext.Players.FirstOrDefault(x => x.Username == username);
        }
    }
}
