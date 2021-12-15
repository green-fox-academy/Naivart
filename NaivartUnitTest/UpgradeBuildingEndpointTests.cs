﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NaivartUnitTest
{
    public class UpgradeBuildingEndpointTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;

        public UpgradeBuildingEndpointTests(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }
        public string GetToken(string username, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = username, password = password });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            return token.token;
        }

        [Fact]
        public void UpgradeBuildingEndpoint_PositiveCaseShouldPass()
        {
            //arrange
            string username = "Adam";
            string password = "Santa";

            var request1 = new HttpRequestMessage(); //actual state of all kingdom buildings
            request1.RequestUri = new Uri("https://localhost:44311/kingdoms/1/buildings");
            request1.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            request1.Method = HttpMethod.Get;

            var request2 = new HttpRequestMessage(); //upgrade request for particular building
            request2.RequestUri = new Uri("https://localhost:44311/kingdoms/1/buildings/1");
            request2.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            request2.Method = HttpMethod.Put;

            //act
            var response1 = HttpClient.SendAsync(request1).Result;
            var responseData1 = response1.Content.ReadAsStringAsync().Result;
            var originalBuilding = JsonConvert.DeserializeObject<BuildingResponse>
                (responseData1).Buildings.Find(b => b.Id == 1);
            originalBuilding.Level += 1; //expected building level after upgrade

            var response2 = HttpClient.SendAsync(request2).Result;
            var responseData2 = response2.Content.ReadAsStringAsync().Result;
            var upgradedBuilding = JsonConvert.DeserializeObject<BuildingsForResponse>
                (responseData2);

            //assert
            Assert.True(response1.IsSuccessStatusCode);
            Assert.True(response2.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            originalBuilding.Should().BeEquivalentTo(upgradedBuilding); //install Fluent Assertions NuGet Package ver. 6.2.0
        }

        [Fact]
        public void UpgradeBuildingEndpoint_NegativeCase1ShouldPass() //if kingdom exists, but building does not
        {
            //arrange
            string username = "Adam";
            string password = "Santa";
            string expectedError = "Data could not be read";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms/1/buildings/3");
            request.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            request.Method = HttpMethod.Put;

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var actualError = JsonConvert.DeserializeObject<StatusForError>(responseData).error;

            //assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(expectedError, actualError);
        }

        [Fact]
        public void UpgradeBuildingEndpoint_NegativeCase2ShouldPass() //insufficient gold for upgrade
        {
            //arrange
            string username = "Honza";
            string password = "Doktor";
            string expectedError = "You don't have enough gold to upgrade that!";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms/4/buildings/2");
            request.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            request.Method = HttpMethod.Put;

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var actualError = JsonConvert.DeserializeObject<StatusForError>(responseData).error;

            //assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedError, actualError);
        }
    }
}
