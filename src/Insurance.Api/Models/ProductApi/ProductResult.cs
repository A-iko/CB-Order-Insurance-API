using Insurance.Api.Models.ProductApi;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;

namespace Insurance.Api.Models.ProductApi
{
    public class ProductApiResult<T>
    {
        public T? data { get; set; }
        public ProductApiResultEnum productApiResult { get; set; }

        public ProductApiResult(ProductApiResultEnum productApiResult, T data)
        {
            this.data = data;
            this.productApiResult = productApiResult;
        }
        public ProductApiResult(ProductApiResultEnum productApiResult)
        {
            this.productApiResult = productApiResult;
        }
    }
}
