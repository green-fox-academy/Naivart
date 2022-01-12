using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Task<List<Kingdom>> GetAllKingdomsAsync();
        Task<Kingdom> RenameKingdomAsync(long kingdomId, string newKingdomName);
        Task<Kingdom> FindPlayerInfoByKingdomIdAsync(long kingdomId);
    }
}
