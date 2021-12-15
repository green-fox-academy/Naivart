using AutoMapper;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Naivart.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naivart.Models.APIModels;
using Naivart.Models.TroopTypes;

namespace Naivart.Services
{
    public class TroopService
    {
        private readonly IMapper _mapper; //install AutoMapper.Extensions.Microsoft.DependencyInjection NuGet Package (ver. 8.1.1)
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TroopService(ApplicationDbContext dbContext, AuthService authService, KingdomService kingdomService, IMapper mapper)
        {
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
            _mapper = mapper;
        }

        public List<TroopAPIModel> ListOfTroopsMapping(List<Troop> troops)
        {
            var TroopsAPIModels = new List<TroopAPIModel>();

            foreach (var troop in troops)
            {
                var TroopsAPIModel = _mapper.Map<TroopAPIModel>(troop);
                TroopsAPIModels.Add(TroopsAPIModel);
            }
            return TroopsAPIModels;
        }
    }

        public bool IsPossibleToCreate(int goldAmount, string troopType)    //If this pass then will create troop
        {
            var model = TroopFactory(troopType, goldAmount);
            if (model != null)
            {
                //send troop when db will be here
                return true;
            }
            return false;
}
    public string TroopCreateRequest(CreateTroopAPIRequest input, long kingdomId, string username, out int status)
    {
        try
        {
            if (AuthService.IsKingdomOwner(kingdomId, username))
            {
                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                if (IsPossibleToCreate(goldAmount, input.Type))
                {
                    status = 200;
                    return "ok";
                }
                status = 400;
                return "You don't have enough gold to train all these units!";
            }
            status = 401;
            return "This kingdom does not belong to authenticated player";
        }
        catch (Exception)
        {
            status = 500;
            return "Data could not be read";
        }
    }

    public TroopModel TroopFactory(string troopType, int goldAmount)
        {
            switch (troopType)
            {
                case "recruit": TroopModel recruit = new Recruit();
                    return recruit.GoldCost == goldAmount ? recruit : null;
                case "archer": TroopModel archer = new Archer();
                    return archer.GoldCost == goldAmount ? archer : null;
                case "knight": TroopModel knight = new Knight();
                    return knight.GoldCost == goldAmount ? knight : null;
            }
            return null;
        }
    }
}
