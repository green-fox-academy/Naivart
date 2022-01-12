using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Interfaces;
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
    }
}
