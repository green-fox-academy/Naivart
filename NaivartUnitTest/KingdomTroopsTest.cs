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
            var TroopAPIModel_1 = new TroopAPIModel() { Id = 1, Level = 1, Hp = 1, Attack = 1, Defense = 1 };
            TroopAPIModels.Add(TroopAPIModel_1);
            var TroopAPIModel_2 = new TroopAPIModel() { Id = 6, Level = 1, Hp = 1, Attack = 2, Defense = 1 };
            TroopAPIModels.Add(TroopAPIModel_2);
            var TroopAPIModel_3 = new TroopAPIModel() { Id = 11, Level = 1, Hp = 1, Attack = 2, Defense = 1 };
            TroopAPIModels.Add(TroopAPIModel_3);
            var TroopAPIModel_4 = new TroopAPIModel() { Id = 12, Level = 2, Hp = 3, Attack = 2, Defense = 2 };
            TroopAPIModels.Add(TroopAPIModel_4);
            var TroopAPIModel_5 = new TroopAPIModel() { Id = 13, Level = 2, Hp = 1, Attack = 4, Defense = 1 };
            TroopAPIModels.Add(TroopAPIModel_5);

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
