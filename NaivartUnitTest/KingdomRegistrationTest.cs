﻿using Microsoft.AspNetCore.Mvc.Testing;
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
    public class KingdomRegistrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public KingdomRegistrationTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        [Theory]
        [InlineData(10, 100)]
        [InlineData(100, 15)]
        [InlineData(100, 150)]
        public void PutCoordinatesOutOfRange_ReturnErrorMessageAndStatusCode400(int coordinateX, int coordinateY)
        {
            string messageExpected = "One or both coordinates are out of valid range (0-99).";
            var statusCodeExpected = HttpStatusCode.BadRequest;
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new KingdomLocationInput() { coordinateX = coordinateX, coordinateY = coordinateY, kingdomId = 1 });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            var response = httpClient.SendAsync(request).Result;

            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }

        [Fact]
        public void PutTakenCoordinates_ReturnErrorMessageAndStatusCode400()
        {
            string messageExpected = "Given coordinates are already taken!";
            var statusCodeExpected = HttpStatusCode.BadRequest;
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new KingdomLocationInput() { coordinateX = 15, coordinateY = 30, kingdomId = 1 });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("Authorization", $"Bearer {GetToken()}");
            var response = httpClient.SendAsync(request).Result;

            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
            Assert.Equal(messageExpected, statusError.error);
        }

        [Fact]
        public void PutObjectWithoutToken_ShouldReturnStatusCode401()
        {
            var statusCodeExpected = HttpStatusCode.Unauthorized;
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new KingdomLocationInput() { coordinateX = 15, coordinateY = 30, kingdomId = 1 });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusForError statusError = JsonConvert.DeserializeObject<StatusForError>(responseBodyContent);

            Assert.Equal(statusCodeExpected, response.StatusCode);
        }

        public string GetToken()
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { username = "Adam", password = "Santa" });

            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://localhost:5467/login", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            TokenWithStatus token = JsonConvert.DeserializeObject<TokenWithStatus>(responseBodyContent);
            return token.token;
        }
    }
}