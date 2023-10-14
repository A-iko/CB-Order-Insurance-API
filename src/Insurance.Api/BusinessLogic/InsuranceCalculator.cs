using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Models;
using Insurance.Api.Models.ProductApi;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Insurance.Api.BusinessLogic
{
    public class InsuranceCalculator
    {
        private readonly ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;
        public InsuranceCalculator(ILogger<InsuranceCalculator> logger, ProductApiClient productApiClient)
        {
            _logger = logger;
            _productApiClient = productApiClient;
        }

        public async Task<InsuranceDtoResult> CalculateInsurance(int productId)
        {
            var productResult = await _productApiClient.GetProduct(productId);

            switch (productResult.productApiResult)
            {
                case ProductApiResultEnum.NotFound:
                    return new InsuranceDtoResult(InsuranceCalculatorResult.NotFound);
                case ProductApiResultEnum.DeserializationError:
                case ProductApiResultEnum.Error:
                    return new InsuranceDtoResult(InsuranceCalculatorResult.Error);
                case ProductApiResultEnum.Success:
                    break;
            }

            var product = productResult.data;

            var productTypeResult = await _productApiClient.GetProductType(product.ProductTypeId);

            switch (productTypeResult.productApiResult)
            {
                case ProductApiResultEnum.NotFound:
                    return new InsuranceDtoResult(InsuranceCalculatorResult.NotFound);
                case ProductApiResultEnum.DeserializationError:
                case ProductApiResultEnum.Error:
                    return new InsuranceDtoResult(InsuranceCalculatorResult.Error);
                case ProductApiResultEnum.Success:
                    break;
            }

            product.ProductType = productTypeResult.data;

            var insuranceDto = new InsuranceDto
            {
                ProductId = product.Id,
                InsuranceValue = 0f
            };

            if (!product.ProductType.CanBeInsured)
                return new InsuranceDtoResult(InsuranceCalculatorResult.Success, insuranceDto);

            switch (product.SalesPrice)
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

            if (product.ProductType.Name == "Laptops" || product.ProductType.Name == "Smartphones") //TODO: Don't hardcode this, also we probably shouldn't do this using the name instead of the ID
                insuranceDto.InsuranceValue += 500;

            _logger.LogDebug("Insurance value for product {0} is {1}", insuranceDto.ProductId, insuranceDto.InsuranceValue);

            return new InsuranceDtoResult(InsuranceCalculatorResult.Success, insuranceDto);
        }
    }
}
