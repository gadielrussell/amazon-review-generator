using Microsoft.ML.Data;

namespace AmazonReviewGenerator.Common.Models.Domain
{
    public class ReviewPrediction
    {
        [ColumnName("PredictedLabel")]
        public float PredictedOverall { get; set; }
    }
}
