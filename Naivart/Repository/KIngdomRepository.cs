using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class KingdomRepository : Repository<Kingdom>, IKingdomRepository
    {
        public KingdomRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Kingdom>> GetAllKingdomsAsync()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return await Task.FromResult(DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .Include(k => k.Buildings)
                    .Include(k => k.Resources)
                    .Include(k => k.Troops)
                    .ThenInclude(k => k.TroopType)
                    .ToList());
            }
            catch
            {
                return kingdoms;
            }
        }
        public async Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName)
        {
            Kingdom kingdom = GetById(kingdomId);
            kingdom.Name = newKingdomName;
            DbContext.Update(kingdom);
            await DbContext.SaveChangesAsync();
            return kingdom;
        }
        public async Task<Kingdom> FindPlayerInfoByKingdomIdAsync(long kingdomId)
        {
            try
            {
                return await Task.FromResult(DbContext.Kingdoms.Where(x => x.Id == kingdomId)
                                        .Include(x => x.Troops).ThenInclude(x => x.TroopType).FirstOrDefault());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public async Task<Kingdom> KingdomIncludeResourceByIdAsync(long kingdomId)
        {
            return await Task.FromResult(DbContext.Kingdoms.Include(x => x.Resources).Where(x => x.Id == kingdomId).FirstOrDefault());
        }
        public async Task<Kingdom> FindKingdomByNameAsync(string kingdomName)
        {
            return await Task.FromResult(DbContext.Kingdoms.FirstOrDefault(x => x.Name == kingdomName));
        }
        public async Task<bool> HasAlreadyLocationAsync(KingdomLocationInput input)
        {
            try
            {
                return await Task.FromResult(DbContext.Kingdoms.Any(x => x.Id == input.KingdomId && x.LocationId == null));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
        public async Task ChangeLocationIdForKingdomAsync(KingdomLocationInput input, long locationId)
        {
            await Task.FromResult(DbContext.Kingdoms.FirstOrDefault(x => x.Id == input.KingdomId).LocationId = locationId);
        }
        public async Task<bool> DoesKingdomExistAsync(long kingdomId)
        {
            try
            {
                return await Task.FromResult(DbContext.Kingdoms.Any(x => x.Id == kingdomId));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Data could not be read", e);
            }
        }
    }
}
