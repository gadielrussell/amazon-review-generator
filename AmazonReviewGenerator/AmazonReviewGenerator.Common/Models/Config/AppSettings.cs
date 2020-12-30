using System;

namespace AmazonReviewGenerator.Common.Models.Config
{
    public class AppSettings
    {
        public string AmazonReviewDataDocId { get; set; }
        public string MarkovBlobContainerName { get; set; }
        public int ReviewMinLength { get; set; }
        public int ReviewMaxLength { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string StorageAccount { get; set; }
    }
}
