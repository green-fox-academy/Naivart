using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Naivart.Services
{
    public class KingdomService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public KingdomService(IMapper mapper, ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _mapper = mapper;
        }

        public List<Kingdom> GetAll()
        {
            var kingdoms = new List<Kingdom>();
            try
            {
                return kingdoms = DbContext.Kingdoms
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .ToList();
            }
            catch
            {
                return kingdoms;
            }
        }

        public List<KingdomAPIModel> ListOfKingdomsMapping(List<Kingdom> kingdoms)
        {
            var kingdomAPIModels = new List<KingdomAPIModel>();

            foreach (var kingdom in kingdoms)
            {
                var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
                var locationAPIModel = _mapper.Map<LocationAPIModel>(kingdom.Location);
                kingdomAPIModel.Location = locationAPIModel;
                kingdomAPIModels.Add(kingdomAPIModel);
            }
            return kingdomAPIModels;
        }

        public Kingdom GetById(long id)
        {
            var kingdom = new Kingdom();
            try
            {
               kingdom = DbContext.Kingdoms
                    .Where(k => k.Id == id)
                    .Include(k => k.Player)
                    .Include(k => k.Location)
                    .Include(k => k.Resources)
                    .Include(k=>k.Troops)
                    .FirstOrDefault();
                return kingdom;
            }
            catch
            {
                return kingdom;
            }
        }

        public KingdomAPIModel KingdomMapping(Kingdom kingdom)
        {
            var kingdomAPIModel = _mapper.Map<KingdomAPIModel>(kingdom);
            var locationAPIModel = _mapper.Map<LocationAPIModel>(kingdom.Location);
            kingdomAPIModel.Location = locationAPIModel;
            return kingdomAPIModel;
        }
    }
}
