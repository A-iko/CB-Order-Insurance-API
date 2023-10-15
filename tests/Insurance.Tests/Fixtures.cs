using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Tests
{
        public class ControllerTestFixture : IDisposable
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
                                int productId = int.Parse((string)context.Request.RouteValues["id"]);
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
