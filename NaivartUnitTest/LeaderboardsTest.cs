using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels.Leaderboards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace NaivartUnitTest
{
    public class LeaderboardsTest : IClassFixture<WebApplicationFactory<Startup>> 
    {
        private readonly HttpClient httpClient;
        public LeaderboardsTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }

        [Fact]
        public void Endpoint_BuildingLeaderboards_ShouldReturnOk()
        {
            //arrange
            var statusCodeExpected = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:44385/leaderboards/buildings");
            request.Method = HttpMethod.Get;

            //act
            var response = httpClient.SendAsync(request).Result;

            //assert
            Assert.Equal(statusCodeExpected, response.StatusCode);
        }

        //add Endpoint_BuildingLeaderboard_ShouldReturnNotFound() with mocking
        //add Endpoint_BuildingLeaderboard_ShouldReturnInternalServerError() with mocking
    }
}
