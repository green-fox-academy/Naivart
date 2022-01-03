using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NaivartUnitTest
{
    public class KingdomTroopsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;
        public KingdomTroopsTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        public string GetToken()
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = "Adam", Password = "Santa" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:44388/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
        }

        [Fact]
        public void Endpoint_Troops_ShouldReturnOk()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/1/troops");
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

            var TroopAPIModels = new List<TroopAPIModel>();
            var TroopAPIModel = new TroopAPIModel() { Id = 1, Level = 3, Hp = 32, Attack = 5, Defense = 6, StartedAt = 12345789, FinishedAt = 12399999 };
            TroopAPIModels.Add(TroopAPIModel);

            var TroopAPIResponse = new TroopsResponse()
            {
                Kingdom = kingdomAPIModel,
                Troops = TroopAPIModels
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<TroopsResponse>(responseData);

            //assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            TroopAPIResponse.Should().BeEquivalentTo(responseDataObj);
        }

        [Fact]
        public void Endpoint_Troops_ShouldReturn_Unauthorized()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://localhost:44388/kingdoms/0/troops");
            request.Method = HttpMethod.Get;

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<TroopsResponse>(responseData);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(responseDataObj);
        }

        [Fact]
        public void CreateTroopsEndpoint_DifferentKingdomIdInRouteThanLoggedUser_ShouldReturnUnauthorized()
        {
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            var inputObj = JsonConvert.SerializeObject(new CreateTroopRequest() { Quantity = 2, Type = "knight"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://localhost:44388/kingdoms/2/troops");
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
