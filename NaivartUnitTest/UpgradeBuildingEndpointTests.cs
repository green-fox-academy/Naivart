using FluentAssertions;
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
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = username, Password = password });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
        }

        [Fact]
        public void UpgradeBuildingEndpoint_NegativeCase1ShouldPass() //if kingdom exists, but building does not
        {
            //arrange
            string username = "Adam";
            string password = "Santa123";
            string expectedError = "There is no such building in this kingdom!";
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44311/kingdoms/1/buildings/3");
            request.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            request.Method = HttpMethod.Put;

            //act
            var response = HttpClient.SendAsync(request).Result;
            var responseData = response.Content.ReadAsStringAsync().Result;
            var actualError = JsonConvert.DeserializeObject<ErrorResponse>(responseData).Error;

            //assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(expectedError, actualError);
        }
    }
}
