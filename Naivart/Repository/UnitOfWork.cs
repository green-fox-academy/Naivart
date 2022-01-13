using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System.Threading.Tasks;

namespace Naivart.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext DbContext;
        public IBuildingRepository Buildings { get; private set; }
        public IKingdomRepository Kingdoms { get; private set; }
        public IPlayerRepository Players { get; private set; }
        public IResourceRepository Resources { get; private set; }
        public ITroopRepository Troops { get; private set; }
        public IBuildingTypeRepository BuildingTypes { get; private set; }
        public ILocationRepository Locations { get; private set; }
        public ITroopTypeRepository TroopTypes { get; private set; }
        public IBattleRepository Battles { get; private set; }
        public IAttackerTroopsRepository AttackerTroops { get; private set; }
        public ITroopsLostRepository TroopsLost { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            DbContext = context;
            Buildings = new BuildingRepository(DbContext);
            BuildingTypes = new BuildingTypeRepository(DbContext);
            Kingdoms = new KingdomRepository(DbContext);
            Players = new PlayerRepository(DbContext);
            Resources = new ResourceRepository(DbContext);
            Troops = new TroopRepository(DbContext);
            Locations = new LocationRepository(DbContext);
            TroopTypes = new TroopTypeRepository(DbContext);
            Battles = new BattleRepository(DbContext);
            AttackerTroops = new AttackerTroopsRepository(DbContext);
            TroopsLost = new TroopsLostRepository(DbContext);
        }

        public async Task CompleteAsync()
        {
           await DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
           DbContext.Dispose();
        }
    }
}
