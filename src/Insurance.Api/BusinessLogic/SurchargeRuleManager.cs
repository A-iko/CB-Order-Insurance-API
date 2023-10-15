using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models.ProductApi;
using Insurance.Api.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Api.Data.Models;
using System.Collections.Generic;
using System;

namespace Insurance.Api.BusinessLogic
{
    public class SurchargeRuleManager
    {
        private readonly ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;
        private readonly InsuranceDbContext _dbContext;

        public SurchargeRuleManager(ILogger<InsuranceCalculator> logger, ProductApiClient productApiClient, InsuranceDbContext dbContext)
        {
            _logger = logger;
            _productApiClient = productApiClient;
            _dbContext = dbContext;
        }

        public async Task<BusinessLogicResult<List<ProductTypeSurchargeRule>>> GetSurchargeRulesForProductTypes()
        {
            _logger.LogInformation("Getting surcharge rules for all product types");

            var UpdateProductTypeResult = await UpdateProductTypes();

            switch (UpdateProductTypeResult)
            {
                case BusinessLogicResultEnum.NotFound:
                case BusinessLogicResultEnum.Error:
                    return new BusinessLogicResult<List<ProductTypeSurchargeRule>>(UpdateProductTypeResult);
                case BusinessLogicResultEnum.Success:
                    break;
            }
         
            var surchargeRules = _dbContext.ProductTypeSurchargeRules.ToList();
            if (!surchargeRules.Any())
            {
                return new BusinessLogicResult<List<ProductTypeSurchargeRule>>(BusinessLogicResultEnum.NotFound);
            }

            return new BusinessLogicResult<List<ProductTypeSurchargeRule>>(BusinessLogicResultEnum.Success, surchargeRules);
        }

        public async Task<BusinessLogicResult<ProductTypeSurchargeRule>> GetSurchargeRulesForProductType(int ProductTypeId)
        {
            _logger.LogInformation("Getting surcharge rules for product type {0}", ProductTypeId);

            var UpdateProductTypeResult = await UpdateProductTypes();

            switch (UpdateProductTypeResult)
            {
                case BusinessLogicResultEnum.NotFound:
                case BusinessLogicResultEnum.Error:
                    return new BusinessLogicResult<ProductTypeSurchargeRule>(UpdateProductTypeResult);
                case BusinessLogicResultEnum.Success:
                    break;
            }

            var surchargeRules = _dbContext.ProductTypeSurchargeRules.FirstOrDefault();
            if(surchargeRules == null)
            {
                return new BusinessLogicResult<ProductTypeSurchargeRule>(BusinessLogicResultEnum.NotFound);
            }

            return new BusinessLogicResult<ProductTypeSurchargeRule>(BusinessLogicResultEnum.Success, surchargeRules);
        }

        public async Task<BusinessLogicResult<ProductTypeSurchargeRule>> SetSurchargeRulesForProductType(ProductTypeSurchargeRule productTypeSurchargeRule)
        {
            _logger.LogInformation("Setting surcharge rules for product type {0}", productTypeSurchargeRule.ProductTypeId);

            var UpdateProductTypeResult = await UpdateProductTypes();

            switch (UpdateProductTypeResult)
            {
                case BusinessLogicResultEnum.NotFound:
                case BusinessLogicResultEnum.Error:
                    return new BusinessLogicResult<ProductTypeSurchargeRule>(UpdateProductTypeResult);
                case BusinessLogicResultEnum.Success:
                    break;
            }

            var surchargeRule = _dbContext.ProductTypeSurchargeRules.FirstOrDefault(x => x.ProductTypeId == productTypeSurchargeRule.ProductTypeId);
            if (surchargeRule == null)
            {
                _logger.LogWarning("Unable to find product type {0}. Adding product types is only possible in the product API", productTypeSurchargeRule.ProductTypeId);
                return new BusinessLogicResult<ProductTypeSurchargeRule>(BusinessLogicResultEnum.NotFound);
            }

            surchargeRule.FlatCartSurcharge = productTypeSurchargeRule.FlatCartSurcharge;
            surchargeRule.FlatItemSurcharge = productTypeSurchargeRule.FlatItemSurcharge;
            surchargeRule.PercentageItemSurcharge = productTypeSurchargeRule.PercentageItemSurcharge;

            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving surcharge rule");
                return new BusinessLogicResult<ProductTypeSurchargeRule>(BusinessLogicResultEnum.Error);
            }

            return new BusinessLogicResult<ProductTypeSurchargeRule>(BusinessLogicResultEnum.Success, productTypeSurchargeRule);
        }

        private async Task<BusinessLogicResultEnum> UpdateProductTypes()
        {
            //Product types being up to date only matters when we fetch or update them.
            //I don't think this is very performant, but it's the easiest way to make sure we have the latest product types.
            //Also, this class currently assumes that product type ID's won't be reused.

            _logger.LogInformation("Updating Product Types");
            var productTypesResult = await _productApiClient.GetProductTypes();

            switch (productTypesResult.productApiResult)
            {
                case ProductApiResultEnum.NotFound:
                case ProductApiResultEnum.DeserializationError:
                case ProductApiResultEnum.Error:
                    return BusinessLogicResultEnum.Error;
                case ProductApiResultEnum.Success:
                    break;
            }

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                //Check if this is efficient
                var productTypesToAdd = productTypesResult.data.Where(x => !_dbContext.ProductTypeSurchargeRules.Any(y => y.ProductTypeId == x.Id)).ToList();
                if (productTypesToAdd.Count == 0)
                {
                    _logger.LogDebug("Product Types are up to date");
                    return BusinessLogicResultEnum.Success;
                }

                _logger.LogInformation("Adding {0} Product Types", productTypesToAdd.Count);

                _dbContext.ProductTypeSurchargeRules.AddRange(productTypesToAdd.Select(x => new ProductTypeSurchargeRule(x.Id)));
                _dbContext.SaveChanges();
                transaction.Commit();

                _logger.LogDebug("Added {0} Product Types successfully", productTypesToAdd.Count);
            }
            return BusinessLogicResultEnum.Success;
        }
    }
}
