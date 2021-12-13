using Microsoft.AspNetCore.Mvc.Testing;
using Naivart;
using Naivart.Models.APIModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace NaivartUnitTest
{
    class BuildingTest :IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        public BuildingTest(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
        }
        public void BuildingGetEndpoint_ShouldReturnOk()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new KingdomResponseForBuilding() { KingdomId = 1 }); 
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        public void BuildingGetEndpoint_ShouldReturn401()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new KingdomResponseForBuilding() { KingdomId = 0 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:44385/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            var response = httpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
