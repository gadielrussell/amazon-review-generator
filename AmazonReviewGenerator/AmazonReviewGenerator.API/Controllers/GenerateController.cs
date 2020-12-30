using AmazonReviewGenerator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonReviewGenerator.API.Controllers
{
    [Route("[controller]")]
    public class GenerateController : Controller
    {
        private readonly IReviewGeneratorService _reviewGenerator;

        public GenerateController(IReviewGeneratorService reviewGenerator)
        {
            _reviewGenerator = reviewGenerator;
        }

        /// <summary>
        /// Returns a generated review.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GenerateReview()
        {
            var review = _reviewGenerator.GenerateReview();
            return Ok(review);
        }
    }
}
