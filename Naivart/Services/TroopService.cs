using Naivart.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naivart.Models.APIModels;
using Naivart.Models.TroopTypes;

namespace Naivart.Services
{
    public class TroopService
    {
        private ApplicationDbContext DbContext { get; }
        public AuthService AuthService { get; set; }
        public KingdomService KingdomService { get; set; }
        public TroopService(ApplicationDbContext dbContext, AuthService authService, KingdomService kingdomService)
        {
            DbContext = dbContext;
            AuthService = authService;
            KingdomService = kingdomService;
        }

        public void TroopCreateRequest(CreateTroopAPIRequest input, long kingdomId, string username)
        {
            if (AuthService.IsKingdomOwner(kingdomId, username))
            {
                int goldAmount = KingdomService.GetGoldAmount(kingdomId);
                if (IsPossibleToCreate(goldAmount, input.Type))
                {

                }
            }
        }

        public bool IsPossibleToCreate(int goldAmount, string troopType)    //If this pass then will create troop
        {
            if (true)
            {

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
