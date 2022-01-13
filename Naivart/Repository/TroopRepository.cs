using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class TroopRepository : Repository<Troop>, ITroopRepository
    {
        public TroopRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
