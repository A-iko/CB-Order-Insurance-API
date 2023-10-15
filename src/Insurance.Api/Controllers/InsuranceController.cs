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

        [HttpGet]
        [Route("api/insurance/products/{productId}")]
        public async Task<ActionResult<InsuranceDto>> CalculateInsurance([FromRoute] int productId)
        {
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

        [HttpPost]
        [Route("api/insurance/products/")]
        public async Task<ActionResult<CartInsuranceDto>> CalculateInsurance([FromBody] List<int> productIds)
        {
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