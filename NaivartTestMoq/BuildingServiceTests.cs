using AutoMapper;
using Moq;
using Naivart.Controllers;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.Entities;
using Naivart.Repository;
using Naivart.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class BuildingServiceTests : ServiceTest
    {
        private readonly BuildingService testBuildingService;
        private Mock<IKingdomService> kingdomServiceMoq = new Mock<IKingdomService>();
        private Mock<IAuthService> authServiceMoq = new Mock<IAuthService>();
        private Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();
        private UnitOfWork unitOfWork;

        public BuildingServiceTests()
        {
            unitOfWork = GetContextWithoutData();
            testBuildingService = new BuildingService(_mapper.Object, unitOfWork,authServiceMoq.Object, kingdomServiceMoq.Object,timeServiceMoq.Object) ;
        }
        [Fact]
        public void ListOfBuildingsMapping()
        {
            var buildingtype = new BuildingType() { Id = 1, Type = "townhall", Level = 5 };
            var buildingtype2 = new BuildingType() { Id = 2, Type = "farm", Level = 1 };
            var kingdom = new Kingdom() { Id = 1, Name = "Igala" };
            var building = new Building() { Id = 1, Type = "townhall", Level = 5, BuildingType = buildingtype, Kingdom = kingdom, KingdomId = 1 };
            var building2 = new Building() { Id = 2, Type = "farm", Level = 1, BuildingType = buildingtype2, Kingdom = kingdom, KingdomId = 1 };
            var buildings = new List<Building>();
            var buildings1 = new List<Building>();
            var buildings2 = new List<Building>();
            buildings1.Add(building);
            buildings2.Add(building2);
            buildings.Add(building);
            buildings.Add(building2);

            buildingtype.Buildings = buildings1;
            buildingtype2.Buildings = buildings2;
            string expectedOnId1= "townhall";
            string expectedOnId2 = "farm";

            var result = testBuildingService.ListOfBuildingsMapping(buildings);
            string actual = result.FirstOrDefault(x => x.Id == 1).Type;
            string actual2 = result.FirstOrDefault(x => x.Id == 2).Type;

            Assert.Equal(expectedOnId1, actual);
            Assert.Equal(expectedOnId2, actual2);
        }
    }
}
