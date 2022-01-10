﻿using System;

namespace Naivart.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IKingdomRepository Kingdoms { get; }
        IBuildingRepository Buildings { get; }
        ILocationRepository Locations { get; }
        IPlayerRepository Players { get; }
        ITroopRepository Troops { get; }
        IResourceRepository Resources { get; }
        ITroopTypeRepository TroopTypes{ get; }

    }
}
