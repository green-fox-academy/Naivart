using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface ITroopsLostRepository : IRepository<TroopsLost>
    {
        Task<List<TroopsLost>> LostTroopsAttackerAsync(long battleId);
        Task<List<TroopsLost>> LostTroopsDefenderAsync(long battleId);
    }
}
