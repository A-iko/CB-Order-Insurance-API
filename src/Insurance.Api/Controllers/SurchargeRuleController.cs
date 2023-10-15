using Insurance.Api.BusinessLogic;
using Insurance.Api.Data.Models;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Insurance.Api.Controllers
{
    public class SurchargeRuleController : Controller
    {
        private readonly ILogger<InsuranceController> _logger;
        private readonly SurchargeRuleManager _surchargeRuleManager;
        public SurchargeRuleController(ILogger<InsuranceController> logger, SurchargeRuleManager surchargeRuleManager)
        {
            _logger = logger;
            _surchargeRuleManager = surchargeRuleManager;
        }

        [HttpGet]
        [Route("api/surchargerules")]
        public async Task<ActionResult<List<ProductTypeSurchargeRule>>> GetSurchargeRules()
        {
            var surchargeRulesResult = await _surchargeRuleManager.GetSurchargeRulesForProductTypes();

            switch (surchargeRulesResult.businessLogicResult)
            {
                case BusinessLogicResultEnum.NotFound:
                    return NotFound();
                case BusinessLogicResultEnum.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(surchargeRulesResult.data);
        }

        [HttpGet]
        [Route("api/surchargerules/{productTypeId}")]
        public async Task<ActionResult<ProductTypeSurchargeRule>> GetSurchargeRule([FromRoute] int productTypeId)
        {
            var surchargeRulesResult = await _surchargeRuleManager.GetSurchargeRulesForProductTypes();

            switch (surchargeRulesResult.businessLogicResult)
            {
                case BusinessLogicResultEnum.NotFound:
                    return NotFound();
                case BusinessLogicResultEnum.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var surchargeRule = surchargeRulesResult.data.Find(x => x.ProductTypeId == productTypeId);

            if (surchargeRule == null)
            {
                return NotFound();
            }

            return Ok(surchargeRule);
        }

        [HttpPost]
        [Route("api/surchargerules")]
        public async Task<ActionResult<ProductTypeSurchargeRule>> SetSurchargeRules([FromBody] ProductTypeSurchargeRule productTypeSurchargeRule)
        {
            var surchargeRuleResult = await _surchargeRuleManager.SetSurchargeRulesForProductType(productTypeSurchargeRule);

            switch (surchargeRuleResult.businessLogicResult)
            {
                case BusinessLogicResultEnum.NotFound:
                    return NotFound();
                case BusinessLogicResultEnum.Error:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(surchargeRuleResult.data);
        }
    }
}
