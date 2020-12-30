using System;

namespace AmazonReviewGenerator.Common.Models.Config
{
    public class AppSettings
    {
        public string AmazonReviewDataDocId { get; set; }
        public string MarkovBlobContainerName { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string StorageAccount { get; set; }
    }
}
