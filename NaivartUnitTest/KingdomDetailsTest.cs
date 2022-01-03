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
    public class KingdomDetailsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public KingdomDetailsTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        [Fact]
        public void AuthorizedPlayerIdInRoute_ShouldReturnPlayerDetails()
        {
            var statusCodeExpected = HttpStatusCode.OK;
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri("https://localhost:44385/kingdoms/1");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(statusCodeExpected, response.StatusCode);
        }

        [Fact]
        public void UnauthorizedPlayerIdInRoute_ShouldReturnUnauthorized()
        {
            var statusCodeExpected = HttpStatusCode.Unauthorized;
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri("https://localhost:44385/kingdoms/2");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(statusCodeExpected, response.StatusCode);
        }

        public string GetToken()
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = "Adam", Password = "Santa" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
        }
    }
}
