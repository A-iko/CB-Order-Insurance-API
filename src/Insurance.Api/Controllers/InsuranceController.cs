using Insurance.Api.BusinessLogic;
using Insurance.Api.Clients;
using Insurance.Api.Models.ProductApi;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public async Task<ActionResult<InsuranceDto>> CalculateInsurance([FromQuery] int productId)
        {
            var insuranceDtoResult = await _insuranceCalculator.CalculateInsurance(productId);

            switch (insuranceDtoResult.insuranceCalculatorResult)
            {
                case InsuranceCalculatorResult.NotFound:
                    return NotFound();                    
                case InsuranceCalculatorResult.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);                    
            }

            return Ok(insuranceDtoResult);
        }
    }
}