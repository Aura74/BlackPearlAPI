using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

using NecklaceCRUD;
using NecklaceDB;

namespace DbAppWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the DbContext to the services
            var connectionString = AppConfig.ConfigurationRoot.GetConnectionString("SQLServer_necklace");
            AppLog.Instance.LogDBConnection(connectionString);

            services.AddDbContext<NecklaceDbContext>(options => options.UseSqlServer(connectionString));
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DbWebApi", Version = "v1" });
            });

            //Dependency Injection for the controller class constructors
            services.AddScoped<INecklaceRepository, NecklaceRepository>();

            // Handle recursion loop?
            services.AddMvc()
             .AddJsonOptions(opt =>
             {
                 //opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                 opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                 opt.JsonSerializerOptions.WriteIndented = true;
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DbWebApi v1");
                c.SupportedSubmitMethods(new[] {
                        SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Delete, SubmitMethod.Post});
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers() );
        }
    }
}
