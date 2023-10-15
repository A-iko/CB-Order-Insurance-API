using Insurance.Api.BusinessLogic;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Data.Models;
using Insurance.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests
{
    public class SurchargeRuleManagerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;

        public SurchargeRuleManagerTests()
        {
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<InsuranceCalculator>();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5002"); //New code

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _productApiClient = new ProductApiClient(loggerFactory.CreateLogger<ProductApiClient>(), new Microsoft.Extensions.Options.OptionsWrapper<ProductApiClientConfiguration>(new ProductApiClientConfiguration() { BaseAddress = "http://localhost:5002" }), mockFactory.Object);
        }

        [Fact]
        public void SetProductTypeSurchargeRule_ShouldSaveAndReturnProductTypeSurchargeRule()
        {
            //Arrange
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            var surchargeRuleManager = CreateSurchargeRuleManager(dbContext);

            var productTypeSurchargeRule = new ProductTypeSurchargeRule()
            {
                ProductTypeId = 1,
                FlatItemSurcharge = 10,
                FlatCartSurcharge = 11,
                PercentageItemSurcharge = 11
            };

            //Act
            var result = surchargeRuleManager.SetSurchargeRulesForProductType(productTypeSurchargeRule).Result;

            //Assert
            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(productTypeSurchargeRule.ProductTypeId, result.data.ProductTypeId);
            Assert.Equal(productTypeSurchargeRule.FlatItemSurcharge, result.data.FlatItemSurcharge);
            Assert.Equal(productTypeSurchargeRule.FlatCartSurcharge, result.data.FlatCartSurcharge);
            Assert.Equal(productTypeSurchargeRule.PercentageItemSurcharge, result.data.PercentageItemSurcharge);

            //Check if the rule was saved to the database
            var savedRule = dbContext.ProductTypeSurchargeRules.FirstOrDefault(x => x.ProductTypeId == productTypeSurchargeRule.ProductTypeId);
            Assert.Equal(productTypeSurchargeRule.ProductTypeId, savedRule.ProductTypeId);
            Assert.Equal(productTypeSurchargeRule.FlatItemSurcharge, savedRule.FlatItemSurcharge);
            Assert.Equal(productTypeSurchargeRule.FlatCartSurcharge, savedRule.FlatCartSurcharge);
            Assert.Equal(productTypeSurchargeRule.PercentageItemSurcharge, savedRule.PercentageItemSurcharge);
        }

        [Fact]
        public void SetProductTypeSurchargeRule_WhenProductTypeDoesNotExist_ShouldReturnNotfound()
        {
            //Arrange
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            var surchargeRuleManager = CreateSurchargeRuleManager(dbContext);

            var productTypeSurchargeRule = new ProductTypeSurchargeRule()
            {
                ProductTypeId = 6,
                FlatItemSurcharge = 10,
                FlatCartSurcharge = 11,
                PercentageItemSurcharge = 11
            };

            //Act
            var result = surchargeRuleManager.SetSurchargeRulesForProductType(productTypeSurchargeRule).Result;

            //Assert
            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        [Fact]
        public void GetProductTypeSurchargeRule_ShouldReturnProductTypeSurchargeRule()
        {
            //Arrange
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            var surchargeRuleManager = CreateSurchargeRuleManager(dbContext);

            var productTypeSurchargeRule = new ProductTypeSurchargeRule()
            {
                ProductTypeId = 1,
                FlatItemSurcharge = 10,
                FlatCartSurcharge = 11,
                PercentageItemSurcharge = 11
            };

            dbContext.ProductTypeSurchargeRules.Add(productTypeSurchargeRule);
            dbContext.SaveChanges();

            //Act
            var result = surchargeRuleManager.GetSurchargeRulesForProductTypes().Result;

            //Assert
            Assert.Equal(BusinessLogicResultEnum.Success, result.businessLogicResult);
            Assert.Equal(productTypeSurchargeRule.ProductTypeId, result.data[0].ProductTypeId);
            Assert.Equal(productTypeSurchargeRule.FlatItemSurcharge, result.data[0].FlatItemSurcharge);
            Assert.Equal(productTypeSurchargeRule.FlatCartSurcharge, result.data[0].FlatCartSurcharge);
            Assert.Equal(productTypeSurchargeRule.PercentageItemSurcharge, result.data[0].PercentageItemSurcharge);
        }

        [Fact]
        public void GetProductTypeSurchargeRule_WhenNoRulesExist_ShouldReturnNotFound()
        {
            //Arrange
            var dbContext = new ConnectionFactory().CreateSQLiteContext();
            var surchargeRuleManager = CreateSurchargeRuleManager(dbContext);

            //Act
            var result = surchargeRuleManager.GetSurchargeRulesForProductTypes().Result;

            //Assert
            Assert.Equal(BusinessLogicResultEnum.NotFound, result.businessLogicResult);
        }

        private SurchargeRuleManager CreateSurchargeRuleManager(InsuranceDbContext dbContext)
        {
            return new SurchargeRuleManager(_logger, _productApiClient, dbContext);
        }
    }
}
