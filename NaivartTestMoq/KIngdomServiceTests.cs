using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Naivart.Controllers;
using Naivart.Interfaces;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class KingdomServiceTests 
    {
        private readonly KingdomService testKingdomService;
        private KingdomController kingdomController;
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();
        private Mock<IKingdomService> kingdomServiceMoq = new Mock<IKingdomService>();
        private Mock<IAuthService> authServiceMoq = new Mock<IAuthService>();
        private Mock<IResourceService> resourceServiceMoq = new Mock<IResourceService>();
        private Mock<IBuildingService> buildingServiceMoq = new Mock<IBuildingService>();
        private Mock<ITroopService> troopServiceMoq = new Mock<ITroopService>();

        public KingdomServiceTests()
        {
            kingdomController = new KingdomController(buildingServiceMoq.Object, kingdomServiceMoq.Object, resourceServiceMoq.Object, troopServiceMoq.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShoudReturnEmptyList()
        {
            // Arrange
            kingdomServiceMoq.Setup(x => x.GetAllAsync()).ReturnsAsync(() => new List<Kingdom> ());
            kingdomController = new KingdomController(buildingServiceMoq.Object, kingdomServiceMoq.Object, resourceServiceMoq.Object, troopServiceMoq.Object);

            // Act
            var actual = await kingdomController.KingdomsInformationAsync();

            // Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        }
      
    }
}
