using AmazonReviewGenerator.Common.Models.Config;
using AmazonReviewGenerator.Common.Models.View;
using AmazonReviewGenerator.Services.Interfaces;
using Azure.Storage.Blobs;
using Markov;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AmazonReviewGenerator.Services
{
    public class ReviewGeneratorService : IReviewGeneratorService
    {
        private readonly MarkovChain<string> _markovChain;
        private readonly BlobContainerClient _blobClient;
        private readonly AppSettings _appSettings;

        public ReviewGeneratorService(
            MarkovChain<string> markovChain,
            BlobContainerClient blobClient,
            AppSettings appSettings)
        {
            _markovChain = markovChain;
            _blobClient = blobClient;
            _appSettings = appSettings;
        }

        public async Task<ReviewLite> GenerateReview()
        {

            return default;
        }

        public async Task TrainModel()
        {
            await _blobClient.CreateIfNotExistsAsync();
            var dataSet = _blobClient.GetBlobClient(_appSettings.AmazonReviewDataDocId);
            var response = await dataSet.DownloadAsync();
            var download = response.Value?.Content;

            using(var sr = new StreamReader(download, Encoding.UTF8))
            {
                // TODO: parse file
                var rawData = sr.ReadToEnd();










            }         

        }
    }
}
