using Insurance.Api.BusinessLogic;
using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static System.Net.WebRequestMethods;

namespace Insurance.Tests
{
    public class InsuranceCalculatorTests: IClassFixture<ControllerTestFixture>
    {
        private readonly ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;

        public InsuranceCalculatorTests()
        {
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<InsuranceCalculator>();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5002"); //New code

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _productApiClient = new ProductApiClient(loggerFactory.CreateLogger<ProductApiClient>(), new Microsoft.Extensions.Options.OptionsWrapper<ProductApiClientConfiguration>(new ProductApiClientConfiguration() { BaseAddress = "http://localhost:5002" }), mockFactory.Object);
        }

        //Tests for the single product call
        [Fact]
        public async Task CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAdd1000EurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 1000;
            const int productId = 1;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenLaptopUnder500Euros_ShouldReturn500Euros()
        {
            const float expectedInsuranceValue = 500;
            const int productId = 2;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(2);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenSmartphoneOver2000Euros_ShouldReturn2500Euros()
        {
            const float expectedInsuranceValue = 2500;
            const int productId = 3;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUninsurableProduct_ShouldReturn0Euros()
        {
            const float expectedInsuranceValue = 0;
            const int productId = 4;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProductType_ShouldReturnNotFound()
        {
            const int productId = 5;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProduct_ShouldReturnNotFound()
        {
            const int productId = 6;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        //Tests for the cart call
        [Fact]
        public async Task CalculateCartInsurance_GivenSalesPriceBetween500And2000Euros_ShouldSetTotalInsuranceTo1500()
        {
            //Manual calculation of expectedInsuranceValue:
            //Product 1: 750 => 1000
            var product1InsuranceValue = 1000;
            //Product 2: 400 => 0 - Laptop => 500
            var product2InsuranceValue = 500;
            //Product 3: 2000 => 2000 - Smartphone => 500
            var product3InsuranceValue = 2500;
            //Product 4: Uninsurable => 0
            var product4InsuranceValue = 0;

            var productIds = new List<int>{ 1, 2, 3, 4 };

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productIds);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(product1InsuranceValue, result.data.InsuredProducts[0].InsuranceValue);
            Assert.Equal(product2InsuranceValue, result.data.InsuredProducts[1].InsuranceValue);
            Assert.Equal(product3InsuranceValue, result.data.InsuredProducts[2].InsuranceValue);
            Assert.Equal(product4InsuranceValue, result.data.InsuredProducts[3].InsuranceValue);
            Assert.Equal(product1InsuranceValue + product2InsuranceValue + product3InsuranceValue + product4InsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateCartInsurance_GivenUnavailableProduct_ShouldReturnNotFound()
        {
            //Product 6 does not exist
            var productIds = new List<int> { 1, 2, 6 };

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productIds);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

    }
}