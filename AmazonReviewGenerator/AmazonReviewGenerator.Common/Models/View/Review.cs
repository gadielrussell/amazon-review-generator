using Microsoft.ML.Data;

namespace AmazonReviewGenerator.Common.Models.View
{
    public class Review : ReviewLite
    {
        public string ReviewerID { get; set; }
        public string Asin { get; set; }
        public string ReviewerName { get; set; }
        public bool Verified { get; set; }
        public string ReviewTime { get; set; }
        public string Summary { get; set; }
        public long UnixReviewTime { get; set; }
    }
}
