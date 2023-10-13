using Insurance.Api.Controllers;
using Insurance.Api.Models.ProductAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Insurance.Tests
{
    public class InsuranceTests: IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;
        private readonly Microsoft.Extensions.Logging.ILogger<InsuranceController> _logger;

        public InsuranceTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
            _logger = new LoggerFactory().CreateLogger<InsuranceController>();
        }

        [Fact]
        public void CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAdd1000EurosToInsuranceCost()
        {
            const float expectedInsuranceValue = 1000;

            var dto = new InsuranceController.InsuranceDto
                      {
                          ProductId = 1,
                      };
            var sut = new InsuranceController(_logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(expectedInsuranceValue, result.InsuranceValue);
        }

        [Fact]
        public void CalculateInsurance_GivenLaptopUnder500Euros_ShouldReturn500Euros()
        {
            const float expectedInsuranceValue = 500;

            var dto = new InsuranceController.InsuranceDto
            {
                          ProductId = 2,
                      };
            var sut = new InsuranceController(_logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(expectedInsuranceValue, result.InsuranceValue);
        }

        [Fact]
        public void CalculateInsurance_GivenSmartphoneOver2000Euros_ShouldReturn2500Euros()
        {
            const float expectedInsuranceValue = 2500;

            var dto = new InsuranceController.InsuranceDto
            {
                          ProductId = 3,
                      };

            var sut = new InsuranceController(_logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(expectedInsuranceValue, result.InsuranceValue);
        }

        [Fact]
        public void CalculateInsurance_GivenUninsurableProduct_ShouldReturn0Euros()
        {
            const float expectedInsuranceValue = 0;

            var dto = new InsuranceController.InsuranceDto
            {
                          ProductId = 4,
                      };
            var sut = new InsuranceController(_logger);

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(expectedInsuranceValue, result.InsuranceValue);
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