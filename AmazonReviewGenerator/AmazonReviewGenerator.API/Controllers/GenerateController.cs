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
        public GenerateController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> GenerateReview()
        {



            return Ok();
        }
    }
}
