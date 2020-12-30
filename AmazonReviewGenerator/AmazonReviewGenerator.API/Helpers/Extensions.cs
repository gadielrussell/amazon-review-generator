using AmazonReviewGenerator.Common.Models.Config;
using AmazonReviewGenerator.Services;
using AmazonReviewGenerator.Services.Interfaces;
using Azure.Storage.Blobs;
using Markov;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AmazonReviewGenerator.API.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Configures the AppSettings POCO for use in Startup.cs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, IConfiguration config, out AppSettings appSettings)
        {
            services.AddOptions();
            services.Configure<AppSettings>(config);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<AppSettings>>().Value);

            appSettings = new AppSettings();
            config.Bind(appSettings);

            return services;
        }

        /// <summary>
        /// Adds Markov services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static async Task<IServiceCollection> AddMarkov(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient((sp) => new MarkovChain<string>(1));

            var sp = services.BuildServiceProvider();
            var markovChain = sp.GetService<MarkovChain<string>>();
            var blobClient = sp.GetService<BlobContainerClient>();
            var rgs = new ReviewGeneratorService(markovChain, blobClient, appSettings);

            await rgs.TrainModel();

            services.AddSingleton<IReviewGeneratorService>(rgs);
            return services;
        }

        /// <summary>
        /// Adds Azure Table Storage APIs.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public static IServiceCollection AddAzureTableStorage(this IServiceCollection services, AppSettings appSettings)
        {
            var storageAcctConnStr = appSettings.ConnectionStrings.StorageAccount;
            var markovBlobContainerName = appSettings.MarkovBlobContainerName;

            if (string.IsNullOrEmpty(storageAcctConnStr))
                throw new Exception("Azure Storage Account connection string configuration value not set properly.");

            if (string.IsNullOrEmpty(markovBlobContainerName))
                throw new Exception("\"MarkovBlobContainerName\" configuration value not set properly.");

            var blobClient = new BlobContainerClient(storageAcctConnStr, markovBlobContainerName);
            services.AddSingleton(blobClient);

            return services;
        }
    }
}
