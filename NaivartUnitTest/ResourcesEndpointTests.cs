using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Database;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace NaivartUnitTest
{
    public class ResourcesEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;

        public ResourcesEndpointTests(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        [Fact]
        public void ResourcesEndpoint_PositiveCaseShouldPass()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://localhost:44311/kingdoms/1/resources");
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

            var resourceAPIModels = new List<ResourceAPIModel>();
            for (int i = 0; i < 2; i++)
            {
                var resourceAPIModel = new ResourceAPIModel()
                {
                    Amount = 1,
                    Generation = 1,
                    UpdatedAt = 123456
                };

                if (i == 0)
                {
                    resourceAPIModel.Type = "food";
                }
                else
                {
                    resourceAPIModel.Type = "gold";
                }

                resourceAPIModels.Add(resourceAPIModel);
            }

            var resourceAPIResponse = new ResourceAPIResponse()
            {
                Kingdom = kingdomAPIModel,
                Resources = resourceAPIModels
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<ResourceAPIResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            resourceAPIResponse.Should().BeEquivalentTo(responseDataObj); //install Fluent Assertions NuGet Package ver. 6.2.0
        }

        [Fact]
        public void ResourcesEndpoint_NegativeCaseShouldPass()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://localhost:44311/kingdoms/0/resources");
            request.Method = HttpMethod.Get;

            var locationAPIModel = new LocationAPIModel();
            var kingdomAPIModel = new KingdomAPIModel();
            var resourceAPIModels = new List<ResourceAPIModel>();
            var resourceAPIResponse = new ResourceAPIResponse()
            {
                Kingdom = kingdomAPIModel,
                Resources = resourceAPIModels
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<ResourceAPIResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            resourceAPIResponse.Should().BeEquivalentTo(responseDataObj); //install Fluent Assertions NuGet Package ver. 6.2.0
        }
    }
}
