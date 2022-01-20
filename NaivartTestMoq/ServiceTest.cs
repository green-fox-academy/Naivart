using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Naivart;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Models.APIModels;
using Naivart.Repository;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Data.Sqlite;
using Xunit;
using Naivart.Services;

namespace NaivartTestMoq
{
    public class ServiceTest
    {
        internal HttpClient HttpClient { get; set; }
        public ServiceTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            HttpClient = appFactory.CreateClient();
        }
        public string GetToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerLogin() { Username = userName, Password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:20625/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            TokenWithStatusResponse token = JsonConvert.DeserializeObject<TokenWithStatusResponse>(contentResponse);
            string tokenResult = token.Token;
            return tokenResult;
        }

        internal UnitOfWork GetContextWithoutData()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
            var context = new ApplicationDbContext(options);

            var unitOfWork = new UnitOfWork(context);
            context.Database.OpenConnectionAsync();
            context.Database.EnsureCreatedAsync();
            return unitOfWork;
        }

    }
}
