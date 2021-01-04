using AmazonReviewGenerator.Common.Models.Config;
using AmazonReviewGenerator.Common.Models.Domain;
using AmazonReviewGenerator.Common.Models.View;
using AmazonReviewGenerator.Services.Interfaces;
using Azure.Storage.Blobs;
using Markov;
using Microsoft.ML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private PredictionEngine<ReviewLite, ReviewPrediction> _predictionEngine;

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

            if (_appSettings.UseSentimentAnalysis)
                review.PredictOverallRatingBasedOnReviewText(_predictionEngine);

            return review;
        }

        /// <summary>
        /// Trains the Markov and Sentiment Prediction model with a set of review data.
        /// </summary>
        /// <returns></returns>
        public async Task TrainModels()
        {
            var reviews = new List<ReviewLite>();
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
                    reviews.Add(review);
                    var reviewWords = review.ReviewText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    _markovChain.Add(reviewWords, 1);
                }
            }

            BuildSentimentPredictionEngine(reviews);
        }

        /// <summary>
        /// Builds and trains a review rating prediction engine based on the provided set of reviews.
        /// </summary>
        /// <param name="reviews"></param>
        public void BuildSentimentPredictionEngine(List<ReviewLite> reviews)
        {
            var context = new MLContext(seed: 0);

            #region DATA PIPELINE: TRANSFORMATION & ALGORITHM
            var dataProcessPipeline = context.Transforms.Text.FeaturizeText(outputColumnName: "ReviewText", inputColumnName: nameof(ReviewLite.ReviewText))
                                             .Append(context.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(ReviewLite.Overall)));

            var trainer = context.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "ReviewText");
            var trainingPipeline = dataProcessPipeline.Append(trainer).Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            #endregion

            #region LOAD AND TRAIN MODEL
            var dataView = context.Data.LoadFromEnumerable(reviews);
            var trainedModel = trainingPipeline.Fit(dataView);
            #endregion

            #region LOCAL TESTING
            //var predictions = trainedModel.Transform(dataView);

            //var schema = dataView.Schema;
            //var preview = predictions.Preview();
            //var data = context.Data.CreateEnumerable<ReviewPrediction>(predictions, true);
            //var metrics = context.MulticlassClassification.Evaluate(predictions, "Label", "Score");
            //var dir = $"{AppDomain.CurrentDomain.BaseDirectory}/AmazonReviewGenerator.Services/ML/ARGReviewModel.zip";

            //if (!Directory.Exists(dir))
            //    Directory.CreateDirectory(dir);
            //context.Model.Save(trainedModel, dataView.Schema, dir);
            #endregion

            _predictionEngine = context.Model.CreatePredictionEngine<ReviewLite, ReviewPrediction>(trainedModel);
        }

        /// <summary>
        /// Predicts an overall rating based on the text of a review.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public void UpdateOverallRatingWithPrediction(ReviewLite review)
        {
            var prediction = _predictionEngine.Predict(review);
            review.Overall = prediction.PredictedOverall;
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