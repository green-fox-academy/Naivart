using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class TroopTypeRepository : Repository<TroopType>,ITroopTypeRepository
    {
        public TroopTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
