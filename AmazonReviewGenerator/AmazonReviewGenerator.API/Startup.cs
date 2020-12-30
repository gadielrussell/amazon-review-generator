using AmazonReviewGenerator.API.Helpers;
using AmazonReviewGenerator.Common.Models.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AmazonReviewGenerator.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region APP SETTINGS
            services.ConfigureAppSettings(Configuration, out AppSettings appSettings);
            #endregion

            #region AZURE TABLE STORAGE
            services.AddAzureTableStorage(appSettings);
            #endregion

            #region MARKOV CHAIN SERVICES
            services.AddMarkov(appSettings).Wait();
            #endregion

            services.AddControllers();
            services.AddCors();
            services.AddMvc(o => o.EnableEndpointRouting = false)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UsePathBase(new PathString("/api/"));
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod()

                //WithOrigins("http://localhost:4200", "http://localhost" )
            );

            app.UseMvc();
        }
    }
}
