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
    public class BuildingTest :IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public BuildingTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }
        public string GetToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = userName, password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(contentResponse);
            string tokenResult = token.token;
            return tokenResult;
        }
        [Fact]
        public void BuildingGetEndpoint_ShouldReturnOk()
        {
            var request = new HttpRequestMessage();
            var tokenResult = GetToken("Adam", "Santa");

            var inputObj = JsonConvert.SerializeObject(new KingdomAPIModel() { Kingdom_Id = 1 }); 
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public void BuildingGetEndpoint_ShouldReturn401()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new KingdomAPIModel() { Kingdom_Id = 0});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public void CreateBuilding_ShouldReturnUnauthorized()
        {
            var expectedStatusCode = HttpStatusCode.Unauthorized;
            var tokenResult = GetToken("Adam", "Santa");
            var inputObj = JsonConvert.SerializeObject(new AddBuildingResponse() { Type = "farm" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://localhost:44385/kingdoms/2/buildings");
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Bearer {tokenResult}");
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
