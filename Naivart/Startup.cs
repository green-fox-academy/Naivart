using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Middlewares;
using Naivart.Models;
using Naivart.Repository;
using Naivart.Services;
using System;
using System.Data.SqlClient;
using System.Text;

namespace Naivart
{
    public class Startup
    {
        private IConfiguration AppConfig { get; }

        public Startup(IConfiguration config)
        {
            AppConfig = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews();

            

            services.AddTransient<AuthService>();
            services.AddTransient<BuildingService>();
            services.AddTransient<KingdomService>();
            services.AddTransient<LoginService>();
            services.AddTransient<PlayerService>();
            services.AddTransient<ResourceService>();
            services.AddTransient<TimeService>();
            services.AddTransient<TroopService>();

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IBuildingRepository, BuildingRepository>();
            services.AddTransient<IBuildingTypeRepository, BuildingTypeRepository>();
            services.AddTransient<IKingdomRepository, KingdomRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            services.AddTransient<IResourceRepository, ResourceRepository>();
            services.AddTransient<ITroopRepository, TroopRepository>();
            services.AddTransient<IBattleRepository, BattleRepository>();
            services.AddTransient<ITroopsLostRepository, TroopsLostRepository>();
            services.AddTransient<IAttackerTroopsRepository, AttackerTroopsRepository>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            services.AddTransient<ITroopTypeRepository, TroopTypeRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            ConfigureDb(services);

            var appSettingSection = AppConfig.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Key);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers().AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Naivart",
                    Description = ".NET 5 API App"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<TimeMiddleware>();    //If you get unexpected errors, try to comment this.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        private void ConfigureDb(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            if (environment == "Production")
            {
                var connectionString = AppConfig.GetConnectionString("naivartserver");
                var sb = new SqlConnectionStringBuilder(connectionString);
                services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(sb.ConnectionString));
            }
        }
    }
}
