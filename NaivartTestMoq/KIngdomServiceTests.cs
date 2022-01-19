using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Naivart.Controllers;
using Naivart.Interfaces;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Repository;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class KingdomServiceTests : ServiceTest
    {
        private readonly KingdomService testKingdomService;
        private KingdomController kingdomController;
        private Mock<IKingdomService> kingdomServiceMoq = new Mock<IKingdomService>();
        private Mock<IAuthService> authServiceMoq = new Mock<IAuthService>();
        private Mock<IResourceService> resourceServiceMoq = new Mock<IResourceService>();
        private Mock<IBuildingService> buildingServiceMoq = new Mock<IBuildingService>();
        private Mock<ITroopService> troopServiceMoq = new Mock<ITroopService>();
        private Mock<ILoginService> loginServiceMoq = new Mock<ILoginService>();
        private Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly IMapper _mapper;
        private UnitOfWork unitOfWork;

        public KingdomServiceTests()
        {
            // kingdomController = new KingdomController(buildingServiceMoq.Object, kingdomServiceMoq.Object, resourceServiceMoq.Object, troopServiceMoq.Object);
            unitOfWork = GetContextWithoutData();
            testKingdomService = new KingdomService(_mapper, authServiceMoq.Object, loginServiceMoq.Object, timeServiceMoq.Object, unitOfWork);
        }

        [Fact]
        public async Task GetAllKingdoms_ShouldReturnListOfKingdom()
        {
            // Arrange
            unitOfWork.Kingdoms.AddAsync(new Kingdom() { Id = 1, Name = "Igala" });
            await unitOfWork.CompleteAsync();
            string expected = "Igala";

            // Act]
            var result = await testKingdomService.GetAllAsync();
            string actual = result.FirstOrDefault(x => x.Id == 1).Name;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
