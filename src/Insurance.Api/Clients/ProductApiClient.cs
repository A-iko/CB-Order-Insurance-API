using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Insurance.Api.Controllers;
using System.Text.Json;
using Insurance.Api.Models.ProductApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Insurance.Api.Clients
{
    public class ProductApiClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        public ProductApiClient(ILogger<ProductApiClient> logger, IOptions<ProductApiClientConfiguration> configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(configuration.Value.BaseAddress);
        }

        public async Task<ProductApiResult<Product>> GetProduct(int productId)
        {
            var productResponse = await _httpClient.GetAsync(string.Format("/products/{0:G}", productId));
            switch (productResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _logger.LogWarning("Product productId could not found");
                    return new ProductApiResult<Product>(ProductApiResultEnum.NotFound);
                case HttpStatusCode.OK:
                    break;
                default:
                    _logger.LogError("ProductApi returned an unexpected statuscode while getting productId {0}: {1}", productId, productResponse.ReasonPhrase);
                    return new ProductApiResult<Product>(ProductApiResultEnum.Error);
            }

            try
            {
                var product = JsonSerializer.Deserialize<Product>(productResponse.Content.ReadAsStringAsync().Result);
                return new ProductApiResult<Product>(ProductApiResultEnum.Success, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProductApiResult<Product>(ProductApiResultEnum.DeserializationError);
            }
        }

        public async Task<ProductApiResult<ProductType>> GetProductType(int productTypeId)
        {
            var productTypeResponse = await _httpClient.GetAsync(string.Format("/product_types/{0:G}", productTypeId));
            switch (productTypeResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _logger.LogWarning("ProductType {0} could not found", productTypeId);
                    return new ProductApiResult<ProductType>(ProductApiResultEnum.NotFound);
                case HttpStatusCode.OK:
                    break;
                default:
                    _logger.LogError("ProductApi returned an unexpected statuscode while getting productId {0}: {1}", productTypeId, productTypeResponse.ReasonPhrase);
                    return new ProductApiResult<ProductType>(ProductApiResultEnum.Error);
            }

            try
            {
                var productType = JsonSerializer.Deserialize<ProductType>(productTypeResponse.Content.ReadAsStringAsync().Result);
                return new ProductApiResult<ProductType>(ProductApiResultEnum.Success, productType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProductApiResult<ProductType>(ProductApiResultEnum.DeserializationError);
            }
        }
    }
}
