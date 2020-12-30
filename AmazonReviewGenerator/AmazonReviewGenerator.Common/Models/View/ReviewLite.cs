using System;

namespace AmazonReviewGenerator.Common.Models.View
{
    public class ReviewLite
    {
        public string ReviewText { get; set; }
        public decimal Overall { get; set; }

        public ReviewLite()
        {  }

        public ReviewLite(string reviewText, decimal? overallRating = null)
        {
            ReviewText = reviewText;
            Overall = overallRating ?? GenerateOverallRating();
        }

        /// <summary>
        /// Generates an overall rating for the review that is between 1 and 5 stars.
        /// </summary>
        /// <returns></returns>
        public decimal GenerateOverallRating() => 
            new Random().Next(1, 6);
    }
}
