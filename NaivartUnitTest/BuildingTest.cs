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
    }
}
