using AutoMapper;
using Moq;
using Naivart.Controllers;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Models.Entities;
using Naivart.Repository;
using Naivart.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class KingdomServiceTests : ServiceTest
    {
        private readonly KingdomService testKingdomService;
        private Mock<IAuthService> authServiceMoq = new Mock<IAuthService>();
        private Mock<ILoginService> loginServiceMoq = new Mock<ILoginService>();
        private Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly IMapper _mapper;
        private UnitOfWork unitOfWork;

        public KingdomServiceTests()
        {      
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
        [Fact]
        public async Task GetByIdAsync_ShouldReturnKingdomById()
        {
            // Arrange
            unitOfWork.Kingdoms.AddAsync(new Kingdom() { Id = 1, Name = "Igala" });
            await unitOfWork.CompleteAsync();
            long expected = 1;
            string expected2 = "Igala";

            // Act
            var result = await testKingdomService.GetByIdAsync(1);
            string actual = result.Name;

            // Assert
            Assert.Equal(expected, result.Id);
            Assert.Equal(expected2, actual);
        }
    }
}
