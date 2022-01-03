using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaivartUnitTest
{
    public class IdentityTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;

        public IdentityTest(WebApplicationFactory<Startup> factory)
        { 

            httpClient = factory.CreateClient();
        }
        public string GetToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = userName, Password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(contentResponse);
            string tokenResult = token.Token;
            return tokenResult;
        }
        [Fact]
        public void AuthPostEndpoint_ShouldReturnOkFromToken()
        {
            var request = new HttpRequestMessage();
            var tokenResult = GetToken("Adam", "Santa");

            var inputObj2 = JsonConvert.SerializeObject(new PlayerIdentity() { Token = tokenResult });
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent2;
            var response2 = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }
        [Fact]
        public void AuthPostEndpoint_ReturnStatusCode401()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerIdentity() { Token = "" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            request.RequestUri = new Uri("https://localhost:44385/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public void AuthPostEndpoint_ShoudReturnInfoAboutPLayer()
        {

            string rulerExpected = "Adam";
            long kingdomIdExpected = 1;
            string kingdomNameExpected = "Igala";
            var statusCodeExpected = HttpStatusCode.OK;
            var tokenResult = GetToken("Adam", "Santa");


            var inputObj2 = JsonConvert.SerializeObject(new PlayerIdentity() { Token = tokenResult });
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            var response2 = httpClient.PostAsync("https://localhost:44385/auth", requestContent2).Result;
            string contentResponse2 = response2.Content.ReadAsStringAsync().Result;
            PlayerWithKingdom player = JsonConvert.DeserializeObject<PlayerWithKingdom>(contentResponse2);

            Assert.Equal(rulerExpected, player.Ruler);
            Assert.Equal(kingdomIdExpected, player.KingdomId);
            Assert.Equal(kingdomNameExpected, player.KingdomName);
            Assert.Equal(statusCodeExpected, response2.StatusCode);
        }
    }
}