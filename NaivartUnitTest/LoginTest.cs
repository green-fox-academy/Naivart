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
        public void PostValidPlayer_ShouldReturnToken()
        {
            string statusExpected = "ok";
            string tokenExpected = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
            var statusCodeExpected = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "Adam", password = "Santa" });
           
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5467/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            var response2 = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response2.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            var resultTokenSplit = token.token.Split(".");
            var resultToken = resultTokenSplit[0];

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(statusExpected, token.status);
            Assert.Equal(tokenExpected, resultToken);
        }

        [Fact]
        public void PostInvalidPlayer_ReturnStatus401AndMessage()
        {
            string messageExpected = "Username and/or password was incorrect!";
            var statusCodeExpected = HttpStatusCode.Unauthorized;

            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "qwoifqwj5454qw", password = "qd55q5dq" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5467/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            var response2 = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response2.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }

        [Fact]
        public void PostEmptyLogin_ReturnStatus400AndMessage()
        {
            string messageExpected = "Field username and/or field password was empty!";
            var statusCodeExpected = HttpStatusCode.BadRequest;

            var request = new HttpRequestMessage();
            var request2 = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "", password = "qd55q5dq" });
            var inputObj2 = JsonConvert.SerializeObject(new PlayerLogin() { username = "qwqwfwfqqfwq11", password = "" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5467/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;
            request2.RequestUri = new Uri("http://localhost:5467/login");
            request2.Method = HttpMethod.Post;
            request2.Content = requestContent2;
            var response2 = httpClient.SendAsync(request2).Result;

            var response3 = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response3.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(statusCodeExpected, response2.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }
    }
}
