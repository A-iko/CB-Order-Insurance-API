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

        /// <summary>
        /// Gets all the surcharge rules.
        /// </summary>
        /// <returns>All product types and their surcharge rules</returns>
        /// <remarks>
        /// Here's an explanation for all the available surcharge rules:
        /// - FlatItemSurcharge: A flat surcharge that will be added to the insurance value of each item.
        /// - FlatCartSurcharge: A flat surcharge that will be added to the total insurance value of the cart. If there are multiple items with the same product type, the surcharge will only be added once.
        /// - PercentageItemSurcharge: A percentage surcharge (calculated on the item's price) that will be added to the insurance value of each item.
        /// </remarks>
        /// <response code = "200">Returns all surcharge rules</response>
        /// <response code = "404">If there are no product types in the database</response>
        /// <respones code = "500">If an error occurs</respones>
        [HttpGet]
        [Route("api/surchargerules")]
        public async Task<ActionResult<List<ProductTypeSurchargeRule>>> GetSurchargeRules()
        {
            _logger.LogInformation("Received request for surcharge rules");
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

        /// <summary>
        /// Gets a surcharge rule.
        /// </summary>
        /// <param name="productTypeId">The product type identifier.</param>
        /// <returns>A product type and it's surcharge rules</returns>
        /// <remarks>
        /// Here's an explanation for all the available surcharge rules:
        /// - FlatItemSurcharge: A flat surcharge that will be added to the insurance value of each item.
        /// - FlatCartSurcharge: A flat surcharge that will be added to the total insurance value of the cart. If there are multiple items with the same product type, the surcharge will only be added once.
        /// - PercentageItemSurcharge: A percentage surcharge (calculated on the item's price) that will be added to the insurance value of each item.
        /// </remarks>
        /// <response code = "200">Returns a surcharge rule</response>
        /// <response code = "404">If the requested product type is not in the database</response>
        /// <respones code = "500">If an error occurs</respones>
        [HttpGet]
        [Route("api/surchargerules/{productTypeId}")]
        public async Task<ActionResult<ProductTypeSurchargeRule>> GetSurchargeRule([FromRoute] int productTypeId)
        {
            _logger.LogInformation("Received request for surcharge rule for product type {0}", productTypeId);
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

        /// <summary>
        /// Sets the surcharge rules.
        /// </summary>
        /// <param name="productTypeSurchargeRule">The product type surcharge rule.</param>
        /// <remarks>
        /// All surcharge rules for the product type will be overwritten.
        ///  Here's an explanation for all the available surcharge rules:
        /// - FlatItemSurcharge: A flat surcharge that will be added to the insurance value of each item.
        /// - FlatCartSurcharge: A flat surcharge that will be added to the total insurance value of the cart. If there are multiple items with the same product type, the surcharge will only be added once.
        /// - PercentageItemSurcharge: A percentage surcharge (calculated on the item's price) that will be added to the insurance value of each item.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("api/surchargerules")]
        public async Task<ActionResult<ProductTypeSurchargeRule>> SetSurchargeRules([FromBody] ProductTypeSurchargeRule productTypeSurchargeRule)
        {
            _logger.LogInformation("Received request to set surcharge rules for product type {0}", productTypeSurchargeRule.ProductTypeId);
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
