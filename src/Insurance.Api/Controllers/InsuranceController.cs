using Insurance.Api.BusinessLogic;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Insurance.Api.Controllers
{

    public class InsuranceController : Controller
    {
        private readonly ILogger<InsuranceController> _logger;
        private readonly InsuranceCalculator _insuranceCalculator;
        public InsuranceController(ILogger<InsuranceController> logger, InsuranceCalculator insuranceCalculator)
        {
            _logger = logger;
            _insuranceCalculator = insuranceCalculator;
        }

        /// <summary>
        /// Calculates the insurance over the provided product identifier.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A CartInsuranceDto that includes the total insurance for the cart and the insurance for each product.
        /// </returns>
        /// <response code = "200">Returns the CartInsuranceDto</response>
        /// <response code = "404">If the product or it's product type is not found</response>
        /// <respones code = "500">If an error occurs</respones>
        [HttpGet]
        [Route("api/insurance/products/{productId}")]
        public async Task<ActionResult<CartInsuranceDto>> CalculateInsurance([FromRoute] int productId)
        {
            _logger.LogInformation("Received request to calculate insurance for product {0}", productId);
            var insuranceDtoResult = await _insuranceCalculator.CalculateInsurance(productId);

            switch (insuranceDtoResult.businessLogicResult)
            {
                case BusinessLogicResultEnum.NotFound:
                    return NotFound();                    
                case BusinessLogicResultEnum.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);                    
            }

            return Ok(insuranceDtoResult.data);
        }

        /// <summary>
        /// Calculates the insurance over provided product Ids.
        /// </summary>
        /// <param name="productIds">The product identifiers.</param>
        /// <returns>
        /// A CartInsuranceDto that includes the total insurance for the cart and the insurance for each product.
        /// </returns>
        /// <response code = "200">Returns the CartInsuranceDto</response>
        /// <response code = "404">If any product or it's product type is not found</response>
        /// <respones code = "500">If an error occurs</respones>
        [HttpPost]
        [Route("api/insurance/products/")]
        public async Task<ActionResult<CartInsuranceDto>> CalculateInsurance([FromBody] List<int> productIds)
        {
            _logger.LogInformation("Received request to calculate insurance for products {0}", productIds);
            var insuranceDtoResult = await _insuranceCalculator.CalculateInsurance(productIds);

            switch (insuranceDtoResult.businessLogicResult)
            {
                case BusinessLogicResultEnum.NotFound:
                    return NotFound();
                case BusinessLogicResultEnum.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(insuranceDtoResult.data);
        }
    }
}