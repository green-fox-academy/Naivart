using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class TroopsLostRepository : Repository<TroopsLost>, ITroopsLostRepository
    {
        public TroopsLostRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
