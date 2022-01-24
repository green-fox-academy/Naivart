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
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = username, Password = password });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:44388/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
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
            request.Headers.Add("Authorization", $"Bearer {GetToken("Pavel", "Bigshock123")}");
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //asert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedErrorMessage, errorResponse.Error);

        }
    }
}
