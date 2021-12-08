using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naivart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaivartUnitTest
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        //protected override void ConfigureDb(IServiceCollection services)
        //{
        //    // Configure in-memory database for testing purposes
        //}
    }
}
