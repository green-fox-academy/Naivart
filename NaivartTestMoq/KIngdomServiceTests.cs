using Moq;
using Naivart.Controllers;
using Naivart.Interfaces;
using Naivart.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NaivartTestMoq
{
    public class KingdomServiceTests
    {
        private readonly KingdomService testKingdomService;
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();


        public KingdomServiceTests()
        {
            //testKingdomService = new KingdomService();
        }
        
        [Fact]
        public async Task GetAllAsync_ShoudReturnEmptyList()
        {
            // Arrange

            // Act


            // Assert
        }
    }
}
