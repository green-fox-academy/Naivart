using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Naivart;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Services;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NaivartUnitTest
{
    public class RegistrationEndpointTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public RegistrationEndpointTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        [Fact]
        public void PostValidRegister_ReturnOk()
        {
            Random r = new Random();
            int rInt = r.Next(0, 1000);

            var statusCodeExpected = HttpStatusCode.OK;
            //long kingdomIdExpected = 18;
            string UsernameExpected = $"usertest{rInt}";

            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RegisterRequest()
            {
                Username = $"usertest{rInt}",
                Password = "password123",
                KingdomName = $"kingdom Name Test{rInt}"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            RegisterResponse RegisterResponse = JsonConvert.DeserializeObject<RegisterResponse>(responseBodyContent);


            Assert.Equal(statusCodeExpected, response.StatusCode);
            //Assert.Equal(kingdomIdExpected, RegisterResponse.kingdomId);
            Assert.Equal(UsernameExpected, RegisterResponse.Username);
        }

        [Fact]
        public void PostInvalidRegister_ReturnBadRequest()
        {
            var statusCodeExpected = HttpStatusCode.BadRequest;
            string ErrorExpected = "Username was empty, already exists or password was shorter than 8 characters!";
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RegisterRequest()
            {
                Username = "",
                Password = "psw",
                KingdomName = "Discovery Channel"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse ErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(ErrorExpected, ErrorResponse.Error);

        }
    }
}
