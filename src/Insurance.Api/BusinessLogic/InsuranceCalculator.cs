using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models;
using Insurance.Api.Models.ProductApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Api.BusinessLogic
{
    public class InsuranceCalculator
    {
        private readonly ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;
        private readonly InsuranceDbContext _dbContext;

        public InsuranceCalculator(
            ILogger<InsuranceCalculator> logger,
            ProductApiClient productApiClient,
            InsuranceDbContext dbContext)
        {
            _logger = logger;
            _productApiClient = productApiClient;
            _dbContext = dbContext;
        }

        public async Task<BusinessLogicResult<CartInsuranceDto>> CalculateInsurance(int productId)
        {
            var cartInsuranceDtoResult = await CalculateInsurance(new List<int> { productId });

            return cartInsuranceDtoResult;
        }

        public async Task<BusinessLogicResult<CartInsuranceDto>> CalculateInsurance(List<int> productIds)
        {
            var insuranceDtos = new List<InsuranceDto>();
            var flatCartSurcharges = new Dictionary<int, decimal>();
            foreach(var productId in productIds)
            {
                var productResult = await _productApiClient.GetProduct(productId);

                switch(productResult.productApiResult)
                {
                    case ProductApiResultEnum.NotFound:
                        return new BusinessLogicResult<CartInsuranceDto>(BusinessLogicResultEnum.NotFound);
                    case ProductApiResultEnum.DeserializationError:
                    case ProductApiResultEnum.Error:
                        return new BusinessLogicResult<CartInsuranceDto>(BusinessLogicResultEnum.Error);
                    case ProductApiResultEnum.Success:
                        break;
                }

                var product = productResult.data;

                var productTypeResult = await _productApiClient.GetProductType(product.ProductTypeId);

                switch(productTypeResult.productApiResult)
                {
                    case ProductApiResultEnum.NotFound:
                        return new BusinessLogicResult<CartInsuranceDto>(BusinessLogicResultEnum.NotFound);
                    case ProductApiResultEnum.DeserializationError:
                    case ProductApiResultEnum.Error:
                        return new BusinessLogicResult<CartInsuranceDto>(BusinessLogicResultEnum.Error);
                    case ProductApiResultEnum.Success:
                        break;
                }

                product.ProductType = productTypeResult.data;

                var insuranceDto = new InsuranceDto { ProductId = product.Id, InsuranceValue = 0 };

                if(product.ProductType.CanBeInsured)
                {
                    //TODO: Move this to the database?
                    switch(product.SalesPrice)
                    {
                        case < 500:
                            insuranceDto.InsuranceValue = 0;
                            break;
                        case >= 500 and < 2000:
                            insuranceDto.InsuranceValue += 1000;
                            break;
                        case >= 2000:
                            insuranceDto.InsuranceValue += 2000;
                            break;
                    }

                    //Apply surcharge rules
                    var surchargeRules = _dbContext.ProductTypeSurchargeRules.Find(product.ProductTypeId);
                    if(surchargeRules != null)
                    {
                        insuranceDto.InsuranceValue += surchargeRules.FlatItemSurcharge;
                        if(surchargeRules.PercentageItemSurcharge > 0 && product.SalesPrice > 0)
                            insuranceDto.InsuranceValue += product.SalesPrice *
                                (surchargeRules.PercentageItemSurcharge / 100);
                        if(!flatCartSurcharges.ContainsKey(product.ProductTypeId))
                            flatCartSurcharges[product.ProductTypeId] = surchargeRules.FlatCartSurcharge;
                    }

                    //I'm assuming we need to round to 2 decimal places here, but I would ask the business for clarification. See https://www.youtube.com/watch?v=yZjCQ3T5yXo
                    insuranceDto.InsuranceValue = Decimal.Round(insuranceDto.InsuranceValue, 2);
                }

                _logger.LogDebug(
                    "Insurance value for product {0} is {1}",
                    insuranceDto.ProductId,
                    insuranceDto.InsuranceValue);
                insuranceDtos.Add(insuranceDto);
            }

            var cartInsuranceDto = new CartInsuranceDto();
            cartInsuranceDto.InsuredProducts = insuranceDtos;
            cartInsuranceDto.CartSurchargeValue = flatCartSurcharges.Values.Sum();
            cartInsuranceDto.TotalInsuranceValue = insuranceDtos.Sum(x => x.InsuranceValue) +
                cartInsuranceDto.CartSurchargeValue;

            return new BusinessLogicResult<CartInsuranceDto>(BusinessLogicResultEnum.Success, cartInsuranceDto);
        }
    }
}
