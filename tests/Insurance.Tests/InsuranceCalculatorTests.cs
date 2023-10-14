using Insurance.Api.BusinessLogic;
using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Models.ProductApi;
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
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests
{
    public class InsuranceCalculatorTests: IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;
        private readonly Microsoft.Extensions.Logging.ILogger<InsuranceCalculator> _logger;
        private readonly ProductApiClient _productApiClient;

        public InsuranceCalculatorTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<InsuranceCalculator>();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5002"); //New code

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _productApiClient = new ProductApiClient(loggerFactory.CreateLogger<ProductApiClient>(), new Microsoft.Extensions.Options.OptionsWrapper<ProductApiClientConfiguration>(new ProductApiClientConfiguration()), mockFactory.Object);
        }

        [Fact]
        public async Task CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAdd1000EurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 1000;
            const int productId = 1;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(InsuranceCalculatorResult.Success, result.insuranceCalculatorResult);
            Assert.Equal(expectedInsuranceValue, result.insuranceDto.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenLaptopUnder500Euros_ShouldReturn500Euros()
        {
            const float expectedInsuranceValue = 500;
            const int productId = 2;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(2);

            Assert.Equal(InsuranceCalculatorResult.Success, result.insuranceCalculatorResult);
            Assert.Equal(expectedInsuranceValue, result.insuranceDto.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenSmartphoneOver2000Euros_ShouldReturn2500Euros()
        {
            const float expectedInsuranceValue = 2500;
            const int productId = 3;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(InsuranceCalculatorResult.Success, result.insuranceCalculatorResult);
            Assert.Equal(expectedInsuranceValue, result.insuranceDto.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUninsurableProduct_ShouldReturn0Euros()
        {
            const float expectedInsuranceValue = 0;
            const int productId = 4;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(InsuranceCalculatorResult.Success, result.insuranceCalculatorResult);
            Assert.Equal(expectedInsuranceValue, result.insuranceDto.InsuranceValue);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProductType_ShouldReturnNotFound()
        {
            const int productId = 5;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(InsuranceCalculatorResult.NotFound, result.insuranceCalculatorResult);
        }

        [Fact]
        public async Task CalculateInsurance_GivenUnavailableProduct_ShouldReturnNotFound()
        {
            const int productId = 6;

            var insuranceCalculator = new InsuranceCalculator(_logger, _productApiClient);

            var result = await insuranceCalculator.CalculateInsurance(productId);

            Assert.Equal(InsuranceCalculatorResult.NotFound, result.insuranceCalculatorResult);
        }
    }

    public class ControllerTestFixture: IDisposable
    {
        private readonly IHost _host;

        public ControllerTestFixture()
        {
            _host = new HostBuilder()
                   .ConfigureWebHostDefaults(
                        b => b.UseUrls("http://localhost:5002")
                              .UseStartup<ControllerTestStartup>()
                    )
                   .Build();

            _host.Start();
        }

        public void Dispose() => _host.Dispose();
    }

    public class ControllerTestStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(
                ep =>
                {
                    ep.MapGet(
                        "products/{id:int}",
                        context =>
                        {
                            int productId = int.Parse((string) context.Request.RouteValues["id"]);
                            var product = new
                                            {
                                                id = productId,
                                                name = "Test Product",
                                                productTypeId = 1,
                                                salesPrice = 750
                                            };
                            switch (productId)
                            {
                                case 1:
                                    break;
                                case 2:
                                    product = new
                                    {
                                        id = productId,
                                        name = "Test Laptop",
                                        productTypeId = 2,
                                        salesPrice = 400
                                    };
                                    break;
                                case 3:
                                    product = new
                                    {
                                        id = productId,
                                        name = "Test Smartphone",
                                        productTypeId = 3,
                                        salesPrice = 2000
                                    };
                                    break;
                                case 4:
                                    product = new
                                    {
                                        id = productId,
                                        name = "Test Uninsurable",
                                        productTypeId = 4,
                                        salesPrice = 2000
                                    };
                                    break;
                                case 5:
                                    product = new
                                    {
                                        id = productId,
                                        name = "Test Bad ProductType",
                                        productTypeId = 5,
                                        salesPrice = 2000
                                    };
                                    break;
                                default:
                                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                                    return context.Response.WriteAsync("Not found");
                            }

                            return context.Response.WriteAsync(JsonConvert.SerializeObject(product));
                        }
                    );
                    ep.MapGet(
                        "product_types/{id:int}",
                        context =>
                        {
                            int productTypeId = int.Parse((string)context.Request.RouteValues["id"]);
                            var productType = new
                            {
                                id = 1,
                                name = "Test type",
                                canBeInsured = true
                            };
                            switch (productTypeId)
                            {
                                case 1:
                                    break;
                                case 2:
                                    productType = new
                                    {
                                        id = 2,
                                        name = "Laptops",
                                        canBeInsured = true
                                    };
                                    break;
                                case 3:
                                    productType = new
                                    {
                                        id = 3,
                                        name = "Smartphones",
                                        canBeInsured = true
                                    };
                                    break;
                                case 4:
                                    productType = new
                                    {
                                        id = 4,
                                        name = "The Uninsurables",
                                        canBeInsured = false
                                    };
                                    break;
                                default:
                                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                                    return context.Response.WriteAsync("Not found");
                            }

                            return context.Response.WriteAsync(JsonConvert.SerializeObject(productType));
                        }
                    );
                    ep.MapGet(
                        "product_types",
                        context =>
                        {
                            var productTypes = new[]
                                               {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test type",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 2,
                                                       name = "Laptops",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 3,
                                                       name = "Smartphones",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 4,
                                                       name = "The Uninsurables",
                                                       canBeInsured = false
                                                   },
                                               };

                            return context.Response.WriteAsync(JsonConvert.SerializeObject(productTypes));
                        }
                    );
                }
            );
        }
    }
}