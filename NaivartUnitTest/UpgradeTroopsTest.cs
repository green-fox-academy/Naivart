using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Naivart.Models.APIModels.Troops;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NaivartUnitTest
{
    public class UpgradeTroopsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;
        public UpgradeTroopsTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        public string GetToken(string username, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = username, password = password });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:44388/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            return token.token;
        }

        [Fact]
        public void UpgradeTroops_ShouldReturnOk()
        {
            //arange
            string messageExpected = "ok";
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new UpgradeTroopsRequest() { Type = "recruit" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/2/troops");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;

            //act
            request.Headers.Add("Authorization", $"Bearer {GetToken("Pavel","Bigshock")}");
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            UpgradeTroopsResponse upgradeTroopsResponse = JsonConvert.DeserializeObject<UpgradeTroopsResponse>(responseBodyContent);

            //asert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(messageExpected, upgradeTroopsResponse.status);
            //Warning: the test will stop working after the troop type reach maximum(20) Level!
        }

        [Theory]
        [InlineData("https://localhost:44388/kingdoms/4/troops", "Honza","Doktor","recruit", "You have to build Academy first!")]
        [InlineData("https://localhost:44388/kingdoms/1/troops", "Adam", "Santa","knight", "You don't have any troop of this type in your army!")]
        [InlineData("https://localhost:44388/kingdoms/1/troops", "Adam","Santa","recruit", "Upgrade Academy first!")]
        [InlineData("https://localhost:44388/kingdoms/2/troops", "Pavel", "Bigshock","archer", "Maximum level reached!")]
        [InlineData("https://localhost:44388/kingdoms/3/troops", "Míra", "Komín","knight", "You don't have enough gold to upgrade this type of troops!")]
        public void UnsuccessfulOperations_ShouldReturnBadRequest(string uriInput, string username, string password,string troopType,string expectedErrorMessage)
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new UpgradeTroopsRequest() { Type = troopType });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri(uriInput);
            request.Method = HttpMethod.Put;
            request.Content = requestContent;

            //act
            request.Headers.Add("Authorization", $"Bearer {GetToken(username, password)}");
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //asert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedErrorMessage, errorResponse.error);
        }

        [Fact]
        public void UpgradeTroops_ShouldReturnUnauthorized()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            string expectedErrorMessage = "This kingdom doesn't belong to authenticated player";
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new UpgradeTroopsRequest() { Type = "recruit" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/1/troops");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;

            //act
            request.Headers.Add("Authorization", $"Bearer {GetToken("Pavel", "Bigshock")}");
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //asert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedErrorMessage, errorResponse.error);

        }
    }
}
