using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        public void KingdomsEndpoint_CaseWithOneKingdomShouldPass()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms");
            request.Method = HttpMethod.Get;

            var locationAPIModel = new LocationAPIModel()
            {
                CoordinateX = 15,
                CoordinateY = 30
            };

            var kingdomAPIModel = new KingdomAPIModel()
            {
                Kingdom_Id = 1,
                KingdomName = "Igala",
                Ruler = "Adam",
                Population = 1,
                Location = locationAPIModel
            };

            var kingdomAPIResponse = new KingdomAPIResponse()
            {
                Kingdoms = new List<KingdomAPIModel>() { kingdomAPIModel }
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<KingdomAPIResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            kingdomAPIModel.Should().BeEquivalentTo(responseDataObj.Kingdoms[0]); //install Fluent Assertions NuGet Package ver. 6.2.0
        }

        [Fact]
        public void KingdomsEndpoint_EdgeCaseWithZeroKingdomsShouldFailForNow() //need to set up functional database and service mocking
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.NotFound;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms");
            request.Method = HttpMethod.Get;

            var kingdomAPIResponse = new KingdomAPIResponse()
            {
                Kingdoms = new List<KingdomAPIModel>()
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<KingdomAPIResponse>(responseData);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            kingdomAPIResponse.Should().BeEquivalentTo(responseDataObj);
        }
    }
}
