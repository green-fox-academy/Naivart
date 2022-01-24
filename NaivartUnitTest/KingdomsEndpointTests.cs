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
    public class KingdomsEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;

        public KingdomsEndpointTests(WebApplicationFactory<Startup> factory)
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

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<KingdomsResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            kingdomAPIModel.Should().BeEquivalentTo(responseDataObj.Kingdoms[0]); //install Fluent Assertions NuGet Package ver. 6.2.0
        }
    }
}
