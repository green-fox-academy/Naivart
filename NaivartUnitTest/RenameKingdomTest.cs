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
    public class RenameKingdomTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient HttpClient;
        public RenameKingdomTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        public string GetToken(bool returnValidToken)
        {
            PlayerLogin playerLogin = new PlayerLogin();
            if (returnValidToken)
            {
                playerLogin.Username = "Pavel";
                playerLogin.Password = "Bigshock123";
            }
            else
            {
                playerLogin.Username = "Adam";
                playerLogin.Password = "Santa123";
            }
            var inputObj = JsonConvert.SerializeObject(playerLogin);

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:44388/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(responseBodyContent);
            return token.Token;
        }

        [Fact]
        public void Endpoint_RenameKingdom_ShouldReturnOk()
        {
            //arrange
            Random r = new Random();
            int rInt = r.Next(0, 1000);

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            string kingdomNameExpected = $"kingdomNameTest{rInt}";
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RenameKingdomRequest()
            {
                KingdomName = $"kingdomNameTest{rInt}"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/2");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("Authorization", $"Bearer {GetToken(true)}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            RenameKingdomResponse renameKingdomResponse = JsonConvert.DeserializeObject<RenameKingdomResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(kingdomNameExpected, renameKingdomResponse.KingdomName);
        }

        [Theory]
        [InlineData("Field kingdomName was empty!"," ")]
        [InlineData("Given kingdom name already exists!", "Nabatean")]
        public void Endpoint_RenameKingdom_ShouldReturnBadRequest(string errorExpectedInput, string kingdomNameInput)
        {
            //arange
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            var request = new HttpRequestMessage();
            string ErrorExpected = errorExpectedInput;
            var inputObj = JsonConvert.SerializeObject(new RenameKingdomRequest()
            {
                KingdomName = kingdomNameInput
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/2");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("Authorization", $"Bearer {GetToken(true)}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(ErrorExpected, errorResponse.Error);
        }


        [Fact]
        public void Endpoint_RenameKingdom_ShouldReturnUnauthorized()
        {
            //arange
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;
            var request = new HttpRequestMessage();
            string ErrorExpected = "This kingdom does not belong to authenticated player";
            var inputObj = JsonConvert.SerializeObject(new RenameKingdomRequest()
            {
                KingdomName = "KnigdomNameTest"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44388/kingdoms/2");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("Authorization", $"Bearer {GetToken(false)}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(ErrorExpected, errorResponse.Error);
        }
    }
}
