using Naivart.Models.Entities;
using System;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IKingdomRepository Kingdoms { get; }
        IBuildingRepository Buildings { get; }
        IBuildingTypeRepository BuildingTypes { get; }
        ILocationRepository Locations { get; }
        IPlayerRepository Players { get; }
        ITroopRepository Troops { get; }
        IResourceRepository Resources { get; }
        ITroopTypeRepository TroopTypes{ get; }
        IBattleRepository Battles { get; }
        IAttackerTroopsRepository AttackerTroops { get; }
        ITroopsLostRepository TroopsLost { get; }
        
        Task CompleteAsync();
    }
}
