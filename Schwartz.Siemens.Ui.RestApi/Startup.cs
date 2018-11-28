using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Schwartz.Siemens.Core.ApplicationServices;
using Schwartz.Siemens.Core.ApplicationServices.Services;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.HostedServices;
using Schwartz.Siemens.Infrastructure.Data;
using Schwartz.Siemens.Infrastructure.Data.Repositories;
using Schwartz.Siemens.Infrastructure.Static.Data;
using Schwartz.Siemens.Ui.RestApi.Auth;
using System;

namespace Schwartz.Siemens.Ui.RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    scope.ServiceProvider.GetRequiredService<IDBInitialization>().SeedDb();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseCors(opt => opt
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                );
                app.UseAuthentication();
                app.UseMvc();
                app.UseHangfireDashboard();
                app.UseHangfireServer();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var secretBytes = new byte[40];
            var random = new Random();
            random.NextBytes(secretBytes);

            #region Database

            // Establish the database
            if (Environment.IsDevelopment())
                // In development, use in Memory. Remember to seed in Configure()
                services.AddDbContext<MaritimeContext>(opt => opt.UseInMemoryDatabase("Siemens"));
            else if (Environment.IsProduction())
                // While in production, use MsSql
                services.AddDbContext<MaritimeContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            else
                // In any other case, use SQLite
                services.AddDbContext<MaritimeContext>(opt => opt.UseSqlite("Data Source=Siemens.db"));

            #endregion Database

            #region Injections

            // Singleton
            services.AddSingleton<EstablishHostedServices>();
            services.AddSingleton<IWebSpider>(new MarineTrafficSpider("https://www.marinetraffic.com/en/ais/details/ships/shipid:"));
            services.AddSingleton<IAuthenticationHelper>(new AuthenticationHelper(secretBytes));

            // Rigs
            services.AddScoped<IRigService, RigService>();
            services.AddScoped<IRigRepository, RigRepository>();

            // Location
            services.AddScoped<ILocationRepository, LocationRepository>();

            // Database
            services.AddScoped<IDBInitialization, DatabaseInitializer>();

            #endregion Injections

            #region Declarations

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                };
            });
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddJsonOptions(options =>
            {
                // Prevents the JSON parser from going into an endless loop if two objects refer to each other
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MaxDepth = 2;
            });
            services.AddHangfire(conf =>
            {
                conf.UseSqlServerStorage("Server=(localDB)\\MSSQLLocalDB;Integrated Security=true;");
            });

            #endregion Declarations
        }
    }
}