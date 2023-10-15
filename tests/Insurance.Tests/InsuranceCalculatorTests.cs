using Insurance.Api.BusinessLogic;
using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Data.Models;
using Insurance.Api.Models;
using Insurance.Api.Models.ProductApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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
    [Collection("RunSequentially")]
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
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            const decimal expectedInsuranceValue = 1000;
            const int productId = 1;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenLaptopUnder500Euros_ShouldReturn500Euros()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 2,
                FlatItemSurcharge = 500,
                FlatCartSurcharge = 0,
                PercentageItemSurcharge = 0
            });

            const decimal expectedInsuranceValue = 500;
            const int productId = 2;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenSmartphoneOver2000Euros_ShouldReturn2500Euros()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 3,
                FlatItemSurcharge = 500,
                FlatCartSurcharge = 0,
                PercentageItemSurcharge = 0
            });
            const decimal expectedInsuranceValue = 2500;
            const int productId = 3;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUninsurableProduct_ShouldReturn0Euros()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            const decimal expectedInsuranceValue = 0;
            const int productId = 4;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProductType_ShouldReturnNotFound()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            const int productId = 5;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProduct_ShouldReturnNotFound()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            const int productId = 6;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        //Tests for the cart call
        [Fact]
        public async Task CalculateCartInsurance_GivenSalesPriceBetween500And2000Euros_ShouldSetTotalInsuranceTo1500()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 2,
                FlatItemSurcharge = 500,
                FlatCartSurcharge = 0,
                PercentageItemSurcharge = 0
            });
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 3,
                FlatItemSurcharge = 500,
                FlatCartSurcharge = 0,
                PercentageItemSurcharge = 0
            });
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

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productIds);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(product1InsuranceValue, result.data.InsuredProducts[0].InsuranceValue);
            Assert.Equal(product2InsuranceValue, result.data.InsuredProducts[1].InsuranceValue);
            Assert.Equal(product3InsuranceValue, result.data.InsuredProducts[2].InsuranceValue);
            Assert.Equal(product4InsuranceValue, result.data.InsuredProducts[3].InsuranceValue);
            Assert.Equal(product1InsuranceValue + product2InsuranceValue + product3InsuranceValue + product4InsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_Given10PercentageSurcharge_Returns2200()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 3,
                FlatItemSurcharge = 0,
                FlatCartSurcharge = 0,
                PercentageItemSurcharge = 10
            });

            const decimal expectedInsuranceValue = 2200;
            const int productId = 3;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_Given1000FlatCartSurcharge_Returns5000()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            dbContext.Add(new ProductTypeSurchargeRule()
            {
                ProductTypeId = 3,
                FlatItemSurcharge = 0,
                FlatCartSurcharge = 1000,
                PercentageItemSurcharge = 0
            });

            //This product has a value of 2000, making the surcharge 2 * 2000 + 1000 = 5000
            const decimal expectedInsuranceValue = 5000;
            var productIds = new List<int> { 3, 3 };

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productIds);

            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(1000, result.data.CartSurchargeValue);
            Assert.Equal(expectedInsuranceValue, result.data.TotalInsuranceValue);
        }

        [Fact]
        public async Task CalculateCartInsurance_GivenUnavailableProduct_ShouldReturnNotFound()
        {
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            //Product 6 does not exist
            var productIds = new List<int> { 1, 2, 6 };

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient, dbContext);

            var result = await insuranceCalculator.CalculateInsurance(productIds);

            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

    }
}