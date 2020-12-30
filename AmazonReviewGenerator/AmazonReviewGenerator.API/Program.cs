using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace AmazonReviewGenerator.API
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
           return  Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseStartup<Startup>()
                                     .ConfigureAppConfiguration((context, builder) =>
                                     {
                                         builder.AddAzureAppConfiguration(options =>
                                         {
                                             var appConfigConnStr = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");
                                             if (string.IsNullOrEmpty(appConfigConnStr))
                                                 throw new Exception("\"APP_CONFIG_CONNECTION\" environment variable not set.");

                                             var creds = new DefaultAzureCredential();
                                             options.Connect(appConfigConnStr)
                                                    .Select(KeyFilter.Any, "ARG")
                                                    .ConfigureKeyVault(kv => kv.SetCredential(creds))
                                                    .ConfigureRefresh(refresh =>
                                                    {
                                                        refresh.Register("RefreshTrigger", "ARG", refreshAll: true)
                                                               .SetCacheExpiration(TimeSpan.FromSeconds(30));
                                                    });
                                         });
                                     });
                       });
        }
    }
}
