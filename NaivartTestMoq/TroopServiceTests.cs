using AutoMapper;
using Moq;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.Entities;
using Naivart.Repository;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class TroopServiceTests : ServiceTest
    {
        private readonly TroopService testTroopService;
        private Mock<IAuthService> authServiceMoq = new Mock<IAuthService>();
        private Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private Mock<IKingdomService> kingdomServiceMoq = new Mock<IKingdomService>();
        private readonly IMapper _mapper;
        private UnitOfWork unitOfWork;

        public TroopServiceTests()
        {
            unitOfWork = GetContextWithoutData();
            testTroopService = new TroopService(_mapper, unitOfWork, authServiceMoq.Object, kingdomServiceMoq.Object, timeServiceMoq.Object);
        }

        [Fact]
        public async Task TroopFactoryAsync_ShouldReturnTroopWithCorrectTroopType()
        {
            unitOfWork.TroopTypes.AddAsync(new TroopType() {Id = 1, Level = 1, Type = "recruit", Attack = 3 });
            await unitOfWork.CompleteAsync();
            long expected = 1;
            string troopType = "recruit";
            int level = 1;

            var result = await testTroopService.TroopFactoryAsync(troopType, level);

            Assert.Equal(expected, result.TroopTypeId);
        }

        [Fact]
        public async Task LevelUpAsync_ShouldChangeStatusToUpgrading()
        {
            var troopType = new TroopType() { Id = 1, Level = 1, Type = "recruit", GoldCost = 100 };
            var troopTypeUpgrade = new TroopType() { Id = 2, Level = 2, Type = "recruit", GoldCost = 200 };
            unitOfWork.TroopTypes.AddAsync(troopType);
            await unitOfWork.CompleteAsync();
            unitOfWork.TroopTypes.AddAsync(troopTypeUpgrade);
            await unitOfWork.CompleteAsync();

            var troops = new List<Troop>() { new Troop() { Id = 1, TroopType = troopType, Status = "town", TroopTypeId = 1} };
            var resource = new Resource() { Id = 1, Type = "gold", Amount = 200, KingdomId = 1};
            var resources = new List<Resource>() { resource };
            var kingdom = new Kingdom() { Id = 1, Resources = resources, Troops = troops };
            unitOfWork.Kingdoms.AddAsync(kingdom);
            await unitOfWork.CompleteAsync();
            string type = "recruit";
            string expected = "upgrading";

            await testTroopService.LevelUpAsync(kingdom, type, troopTypeUpgrade);
            var result = unitOfWork.Troops.FirstOrDefault(x => x.Id == 1).Status;
            
            Assert.Equal(expected, result);
        }
    }
}
