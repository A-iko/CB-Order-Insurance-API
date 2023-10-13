using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Insurance.Api.Controllers;
using System.Text.Json;
using Insurance.Api.Models.ProductAPI;

namespace Insurance.Api
{
    public static class BusinessRules
    {
        public static Product GetProduct(string baseAddress, int productID)
        {
            HttpClient client = new HttpClient{ BaseAddress = new Uri(baseAddress)};

            var productJson = client.GetAsync(string.Format("/products/{0:G}", productID)).Result.Content.ReadAsStringAsync().Result;
            var product = JsonSerializer.Deserialize<Product>(productJson);

            var productTypeJson = client.GetAsync(string.Format("/product_types/{0:G}", product.ProductTypeId)).Result.Content.ReadAsStringAsync().Result;
            product.ProductType = JsonSerializer.Deserialize<ProductType>(productTypeJson);

            return product;
        }
    }
}