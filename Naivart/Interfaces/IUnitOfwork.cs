using System;

namespace Naivart.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IKingdomRepository Kingdoms { get; }
        IBuildingRepository Buildings { get; }
        ILocationRepository Locations { get; }
        IPlayerRepository Players { get; }
    }
}
