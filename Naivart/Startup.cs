using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Naivart.Database;
using Naivart.Interfaces;
using Naivart.Middlewares;
using Naivart.Models;
using Naivart.Repository;
using Naivart.Services;
using System;
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

            ConfigureDb(services);

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

            app.UseMiddleware<TimeMiddleware>();    //If you get errors using id in route then comment this.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void ConfigureDb(IServiceCollection services)
        {
            var connectionString = AppConfig.GetConnectionString("DefaultConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0));

            services.AddDbContext<ApplicationDbContext>(
                options => options
                .UseMySql(connectionString, serverVersion)
                // The following three options help with debugging.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
        }
    }
}
