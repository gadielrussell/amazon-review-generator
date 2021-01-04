using AmazonReviewGenerator.Common.Models.Domain;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace AmazonReviewGenerator.Common.Models.View
{
    public partial class ReviewLite
    {
        public string ReviewText { get; set; }
        public float Overall { get; set; }

        public ReviewLite()
        {  }

        public ReviewLite(string reviewText, float? overallRating = null)
        {
            ReviewText = reviewText;
            Overall = overallRating ?? GenerateOverallRating();
        }

        /// <summary>
        /// Generates an overall rating for the review that is between 1 and 5 stars.
        /// </summary>
        /// <returns></returns>
        public float GenerateOverallRating() => 
            new Random().Next(1, 6);

        /// <summary>
        /// Predicts an overall rating based on review text.
        /// </summary>
        /// <param name="predictionEngine"></param>
        public void PredictOverallRatingBasedOnReviewText(PredictionEngine<ReviewLite, ReviewPrediction> predictionEngine)
        {
            var prediction = predictionEngine.Predict(this);
            this.Overall = prediction.PredictedOverall;
        }
    }
}
