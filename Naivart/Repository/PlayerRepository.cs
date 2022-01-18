using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<bool> IsKingdomOwnerAsync(long kingdomId, string username)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.Any(x => x.KingdomId == kingdomId && x.Username == username));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public async Task<Player> PlayerIncludeKingdomFindByUsernameAndPasswordAsync(string username,string hashedPassword)
        {
            return await Task.FromResult(DbContext.Players.Include(x => x.Kingdom).FirstOrDefault
                (x => x.Username == username && x.Password == hashedPassword));
        }
        public async Task<Player> GetPlayerByIdAsync(long id)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.Include(p => p.Kingdom)
                                .FirstOrDefault(p => p.Id == id));
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsInDbWithThisUsernameAsync(string username)
        {
            return await Task.FromResult(DbContext.Players.Any(x => x.Username == username));
        }
        public async Task<Player> FindByUsernameAsync(string username)
        {
            return await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.Username == username));
        }
        public async Task<Player> FindPlayerIncludeKingdomsByUsernameAsync(string name)
        {
            return await Task.FromResult(DbContext.Players.Include(x => x.Kingdom).Where(x => x.Username == name).FirstOrDefault());
        }
        public async Task<PlayerInfo> FindPlayerByNameReturnPlayerInfoAsync(string name)
        {
            var model = await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.Username == name));
            return new PlayerInfo() { Id = model.Id, Username = model.Username, KingdomId = model.KingdomId };
        }
        public async Task<bool> IsUserKingdomOwnerAsync(long kingdomId, string username)
        {
            try
            {
                return await Task.FromResult(DbContext.Players.FirstOrDefault(x => x.KingdomId == kingdomId)?
                                        .Username == username);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
