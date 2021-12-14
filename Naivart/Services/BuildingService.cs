using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; }
        private readonly IMapper Mapper;
        public BuildingService(IMapper mapper,ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        public List<BuildingsForResponse> ListOfBuildingMapping(List<Building> buildings)
        {
            var buildingForResponses= new List<BuildingsForResponse>();

            if (buildings is null)
            {
                return buildingForResponses;
            }

            foreach (var building in buildings)
            {
                var buildingForResponse = Mapper.Map<BuildingsForResponse>(building);
                buildingForResponses.Add(buildingForResponse);
            }
            return buildingForResponses;
        }   
    }
}
