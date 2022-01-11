using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.Entities;
using System;

namespace Naivart.Repository
{
    public class BuildingTypeRepository: Repository<BuildingType>, IBuildingTypeRepository
    {
        public BuildingTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
