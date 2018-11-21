using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Schwartz.Siemens.Infrastructure.Data;
using Schwartz.Siemens.Infrastructure.Static.Data;

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
                    var context = scope.ServiceProvider.GetService<MaritimeContext>();
                    DatabaseInitializer.SeedDb(context);
                }

                app.UseCors(opt => opt
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                );
                app.UseAuthentication();
                app.UseMvc();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            // Tells the App to use CORS. Configured in Configure()
            services.AddCors();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    // Prevents the JSON parser from going into an endless loop if two objects refer to each other
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.MaxDepth = 2;
                });
        }
    }
}