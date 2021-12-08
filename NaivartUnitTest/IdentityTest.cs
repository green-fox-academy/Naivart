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
            //factory.ConfigureTestServices(services =>
            //{
            //    // You can mock services here if you have to
            //    // e.g.
            //    //var mockService = Substitute.For<IHelloService>();
            //    //string expectedResult = "test";
            //    //mockService.SayHello().Returns(expectedResult);

            //    //services.AddTransient<IHelloService>(_ => mockService);
            //});

            httpClient = factory.CreateClient();
        }
        [Fact]
        public void AuthPostEndpoint_ShouldReturnOkFromToken()
        {
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new PlayerIdentity() { token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkYW0iLCJuYmYiOjE2Mzg4ODgwMjgsImV4cCI6MTYzODk3NDQyOCwiaWF0IjoxNjM4ODg4MDI4fQ.A5ngmHslgtIbKFuIStQtdYK_fw-sRriReWsYtx_f7zY" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            request.RequestUri = new Uri("https://localhost:44385/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public void AuthPostEndpoint_ReturnStatusCode401()
        {
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new PlayerIdentity() { token = "" });
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
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerLogin { username = "Adam", password = "Santa" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://localhost:44385/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(contentResponse);
            string tokenResult = token.token;

            var inputObj2 = JsonConvert.SerializeObject(new PlayerIdentity() { token = tokenResult });
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            var response2 = httpClient.PostAsync("https://localhost:44385/auth", requestContent2).Result;
            string contentResponse2 = response2.Content.ReadAsStringAsync().Result;
            PlayerWithKingdom player = JsonConvert.DeserializeObject<PlayerWithKingdom>(contentResponse2);

            Assert.Equal(rulerExpected, player.ruler);
            Assert.Equal(kingdomIdExpected, player.kingdomId);
            Assert.Equal(kingdomNameExpected, player.kingdomName);
            Assert.Equal(statusCodeExpected, response2.StatusCode);
        }
    }
}