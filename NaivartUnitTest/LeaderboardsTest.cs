using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using System;
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

        [Theory]
        [InlineData("https://localhost:44385/leaderboards/buildings")]
        [InlineData("https://localhost:44385/leaderboards/troops")]
        [InlineData("https://localhost:44385/leaderboards/kingdoms")]
        public void Endpoint_BuildingLeaderboards_ShouldReturnOk(string UriInput)
        {
            //arrange
            var statusCodeExpected = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(UriInput);
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
