using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Naivart.Models.Entities;
using Naivart.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaivartUnitTest
{
    public class KingdomsIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;

        public KingdomsIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        [Fact]
        public void KingdomsEndpoint_ShouldReturnListOfKingdomAPIModels()
        {
            //arrange
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms");
            request.Method = HttpMethod.Get;

            //act
            var response = HttpClient.SendAsync(request).Result;

            //assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        //[Fact]
        //public void Get_EdgeCaseWithZeroKingdomsShouldReturnEmptyList()
        //{
        //    //arrange
        //    var request = new HttpRequestMessage();
        //    var inputObj = JsonConvert.SerializeObject(new List<KingdomAPIModel>());
        //    StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
        //    request.RequestUri = new Uri("https://localhost:44311/kingdoms");
        //    request.Method = HttpMethod.Get;
        //    request.Content = requestContent;

        //    //act
        //    var response = HttpClient.SendAsync(request).Result;

        //    //assert
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        [Fact]
        public void KingdomsEndpoint_ShouldReturnListOfKingdomAPIModels2()
        {
            //arrange4
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            //string expectedMessage = "No data was present!";
            //var inputDataObj = JsonConvert.SerializeObject(new List<KingdomAPIModel>());
            //StringContent requestContent = new(inputDataObj, Encoding.UTF8, "application/json");
            var kingdomAPIModel = new KingdomAPIModel()
            {
                Id = 1,
                Name = "Igala",
                PlayerUsername = "Adam",
                Population = 1,
                LocationCoordinates = new Dictionary<string, int>() { { "coordinateX", 15 }, { "coordinateY", 30 } }
            };
            
            //act
            var response = HttpClient.GetAsync("https://localhost:44311/kingdoms").Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<List<KingdomAPIModel>>(responseData);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(kingdomAPIModel, responseDataObj[0]);
        }
    }
}
