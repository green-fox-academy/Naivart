using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Naivart;
using Naivart.Database;
using Naivart.Models.APIModels;
using Naivart.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace NaivartUnitTest
{
    public class RegistrationEndpointTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public PlayerService PlayerService { get; set; }
        public RegistrationEndpointTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
            PlayerService = factory.Services.GetService<PlayerService>();
        }

        [Fact]
        public void PostValidRegister_ReturnOk()
        {
            var statusCodeExpected = HttpStatusCode.OK;
            long kingdomIdExpected = 10;
            string UsernameExpected = "adam0096";

            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RegisterRequest()
            {
                username = $"usertest",
                password = "password123",
                kingdomName = "kingdom Name Test"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");

            var response = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            RegisterResponse RegisterResponse = JsonConvert.DeserializeObject<RegisterResponse>(responseBodyContent);


            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(kingdomIdExpected, RegisterResponse.kingdomId);
            Assert.Equal(UsernameExpected, RegisterResponse.username);

            PlayerService.DeleteByUsername(RegisterResponse.username);
        }

        [Fact]
        public void PostInvalidRegister_ReturnBadRequest()
        {
            var statusCodeExpected = HttpStatusCode.BadRequest;
            string ErrorExpected = "Username was empty, already exists or password was shorter than 8 characters!";
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RegisterRequest()
            {
                username = "",
                password = "psw",
                kingdomName = "Discovery Channel"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse ErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(ErrorExpected, ErrorResponse.error);

        }
    }
}
