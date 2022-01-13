using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;

namespace Naivart.Repository
{
    public class AttackerTroopsRepository : Repository<AttackerTroops>, IAttackerTroopsRepository
    {
        public AttackerTroopsRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
