using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public string GetToken()
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = "Adam", Password = "Santa123" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
        }

        [Fact]
        public void ResourcesEndpoint_PositiveCaseShouldPass()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms/1/resources");
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
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
                    Amount = 10,
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

            var resourceAPIResponse = new ResourcesResponse(kingdomAPIModel, resourceAPIModels);

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<ResourcesResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            resourceAPIResponse.Should().BeEquivalentTo(responseDataObj); //install Fluent Assertions NuGet Package ver. 6.2.0
        }

        [Fact]
        public void ResourcesEndpoint_NegativeCaseShouldPass()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms/0/resources");
            request.Method = HttpMethod.Get;

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<ResourcesResponse>(responseData);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(responseDataObj);
        }
    }
}
