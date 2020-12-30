using AmazonReviewGenerator.Common.Models.Config;
using AmazonReviewGenerator.Common.Models.View;
using AmazonReviewGenerator.Services.Interfaces;
using Azure.Storage.Blobs;
using Markov;
using Newtonsoft.Json;
using System;
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
        private readonly Random _randomizer;

        public ReviewGeneratorService(
            MarkovChain<string> markovChain,
            BlobContainerClient blobClient,
            AppSettings appSettings)
        {
            _markovChain = markovChain;
            _blobClient = blobClient;
            _appSettings = appSettings;
            _randomizer = new Random();
        }

        /// <summary>
        /// Generates a random review based on the trained model along with an overall rating.
        /// </summary>
        /// <returns></returns>
        public ReviewLite GenerateReview()
        {
            var reviewWordLenth = _randomizer.Next(_appSettings.ReviewMinLength, _appSettings.ReviewMaxLength + 1);
            var origReviewText = string.Join(' ', _markovChain.Chain(reviewWordLenth));
            var formattedReviewText = CleanUpReviewText(origReviewText);

            var review = new ReviewLite(formattedReviewText);

            return review;
        }

        /// <summary>
        /// Trains the Markov model with a set of review data.
        /// </summary>
        /// <returns></returns>
        public async Task TrainModel()
        {
            await _blobClient.CreateIfNotExistsAsync();
            var dataSet = _blobClient.GetBlobClient(_appSettings.AmazonReviewDataDocId);
            var response = await dataSet.DownloadAsync();
            var download = response.Value?.Content;

            if (download is null)
                throw new Exception("No content found when downloading Training data. Check \"AmazonReviewDataDocId\" and Blob upload.");

            using (var sr = new StreamReader(download, Encoding.UTF8))
            {
                var rawData = sr.ReadToEnd();
                var reviewsJsonCollection = rawData.Split(Environment.NewLine);

                foreach (var rData in reviewsJsonCollection)
                {
                    var review = JsonConvert.DeserializeObject<ReviewLite>(rData);
                    var reviewTextNotAvailable = string.IsNullOrEmpty(review.ReviewText);

                    if (reviewTextNotAvailable)
                        continue;

                    var reviewWords = review.ReviewText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    _markovChain.Add(reviewWords, 1);
                }
            }
        }

        /// <summary>
        /// Attempts to clean up generated review text.
        /// </summary>
        /// <param name="originalReviewText"></param>
        /// <returns></returns>
        private string CleanUpReviewText(string originalReviewText)
        {
            var formattedReviewTextSb = new StringBuilder();

            char[] twoPrevChar = new char[2] { '\0', '\0' };
            Func<bool> capitalizeCurrentChar = () => twoPrevChar[0] == '.' && twoPrevChar[1] == ' ';
            Action<char> updatePrevCharacterArr = (char c) =>
            {
                twoPrevChar[0] = twoPrevChar[1];
                twoPrevChar[1] = c;
            };

            for (int i = 0; i < originalReviewText.Length; i++)
            {
                var c = originalReviewText[i];

                if (i == 0 || capitalizeCurrentChar())
                {
                    c = c.ToString().ToUpperInvariant()[0];
                }

                formattedReviewTextSb.Append(c);
                updatePrevCharacterArr(c);
            }

            return formattedReviewTextSb.ToString();
        }
    }
}
