﻿using Microsoft.EntityFrameworkCore;
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
                    && !isInDbWithThisUsername(username))
                {
                    Player player = new Player() { Username = username, Password = password };
                    Kingdom kingdom = new Kingdom();

                //check if given kingdom name(and username) is not empty or already exists in database 
                kingdom.Name = !String.IsNullOrWhiteSpace(kingdomName) && FindKingdomByName(kingdomName) == null ? kingdomName : $"{player.Username}'s kingdom";

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
        public Player FindByUsername(string username)
        {
            return DbContext.Players.FirstOrDefault(x => x.Username == username);
        }
        public bool isInDbWithThisUsername(string username)
        {
            return DbContext.Players.Any(x => x.Username == username) ? true : false;
        }
        public void DeleteByUsername(string username)
        {
            DbContext.Players.Remove(FindByUsername(username));
        }
        public Player GetPlayerById(long id)
        {
            try
            {
                return DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.Id == id);
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
