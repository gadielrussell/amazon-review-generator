using AmazonReviewGenerator.Common.Models.View;
using System.Threading.Tasks;

namespace AmazonReviewGenerator.Services.Interfaces
{
    public interface IReviewGeneratorService
    {
        Task<ReviewLite> GenerateReview();
        Task TrainModel();
    }
}
