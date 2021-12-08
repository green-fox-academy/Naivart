using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NaivartUnitTest
{
    public class RegistrationEndpointTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public RegistrationEndpointTest(WebApplicationFactory<Startup> factory)
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
        public void PostValidRegister_ReturnOk()
        {
            var statusCodeExpected = HttpStatusCode.OK;
            long kingdomIdExpected = 10;
            string UsernameExpected = "adam0096";

            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new RegisterRequest()
            {
                username = "adam0096",
                password = "password123",
                kingdomName = "Discovery Channel"
            });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            //request.RequestUri = new Uri("https://localhost:44388/registration");
            //request.Method = HttpMethod.Post;
            //request.Content = requestContent;
            //var response = httpClient.SendAsync(request).Result;

            var response2 = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response2.Content.ReadAsStringAsync().Result;
            RegisterResponse RegisterResponse = JsonConvert.DeserializeObject<RegisterResponse>(responseBodyContent);


            Assert.Equal(statusCodeExpected, response2.StatusCode);
            Assert.Equal(kingdomIdExpected, RegisterResponse.kingdomId);
            Assert.Equal(UsernameExpected, RegisterResponse.username);

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
            var response2 = httpClient.PostAsync("https://localhost:44388/registration", requestContent).Result;
            string responseBodyContent = response2.Content.ReadAsStringAsync().Result;
            ErrorResponse ErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response2.StatusCode);
            Assert.Equal(ErrorExpected, ErrorResponse.error);

        }
    }
}
