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
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "Adam", password = "Santa" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:44388/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            return token.token;
        }

        [Fact]
        public void Endpoint_Troops_ShouldReturnOk()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/5/troops");
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            request.Method = HttpMethod.Get;

            var locationAPIModel = new LocationAPIModel()
            {
                CoordinateX = 90,
                CoordinateY = 80
            };

            var kingdomAPIModel = new KingdomAPIModel()
            {
                Kingdom_Id = 5,
                KingdomName = "Deira",
                Ruler = "Fro",
                Population = 1,
                Location = locationAPIModel
            };

            var TroopAPIModels = new List<TroopAPIModel>();
            var TroopAPIModel_1 = new TroopAPIModel() { Id = 5, Level = 1, Hp = 1, Attack = 1, Defense = 1 };
            TroopAPIModels.Add(TroopAPIModel_1);
            var TroopAPIModel_2 = new TroopAPIModel() { Id = 10, Level = 5, Hp = 6, Attack = 2, Defense = 2 };
            TroopAPIModels.Add(TroopAPIModel_2);
            var TroopAPIModel_3 = new TroopAPIModel() { Id = 20, Level = 5, Hp = 6, Attack = 2, Defense = 2 };
            TroopAPIModels.Add(TroopAPIModel_3);

            var TroopAPIResponse = new TroopAPIResponse()
            {
                Kingdom = kingdomAPIModel,
                Troops = TroopAPIModels
            };

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var responseDataObj = JsonConvert.DeserializeObject<TroopAPIResponse>(responseData);

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
            var responseDataObj = JsonConvert.DeserializeObject<TroopAPIResponse>(responseData);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Null(responseDataObj);
        }
    }
}
