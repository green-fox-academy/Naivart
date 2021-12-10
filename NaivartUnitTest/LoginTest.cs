using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaivartUnitTest
{
    public class LoginTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public LoginTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        [Fact]
        public void PostValidPlayer_ShouldReturnToken()
        {
            string statusExpected = "ok";
            string tokenExpected = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
            var statusCodeExpected = HttpStatusCode.OK;
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "Adam", password = "Santa" });
           
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            var resultTokenSplit = token.token.Split(".");
            var resultToken = resultTokenSplit[0];

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(statusExpected, token.status);
            Assert.Equal(tokenExpected, resultToken);
        }

        [Theory]
        [InlineData("Adam", "qojqodqj")]    //if username is correct, but password incorrect
        [InlineData("qojdoqwf44q8", "Santa")]   //if username is incorrect, but password correct
        [InlineData("qojdoqwf44q8", "qw5d1qw51q5qa")]   //if both incorrect
        public void PostInvalidPlayer_ReturnStatus401AndMessage(string usernameInput, string passwordInput)
        {
            string messageExpected = "Username and/or password was incorrect!";
            var statusCodeExpected = HttpStatusCode.Unauthorized;
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = usernameInput, password = passwordInput });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }

        //[Fact]
        [Theory]
        [InlineData("", "qojqodqj")]
        [InlineData("qf5qf5q1q5", "")]
        [InlineData("", "")]
        public void PostEmptyLogin_ReturnStatus400AndMessage(string usernameInput, string passwordInput)
        {
            string messageExpected = "Field username and/or field password was empty!";
            var statusCodeExpected = HttpStatusCode.BadRequest;
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = usernameInput, password = passwordInput });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }
    }
}