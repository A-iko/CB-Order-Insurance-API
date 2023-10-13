using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.Api.Controllers
{
    public class HomeController: Controller
    {
        [HttpPost]
        [Route("api/insurance/product")]
        public InsuranceDto CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            int productId = toInsure.ProductId;

            BusinessRules.GetProductType(ProductApi, productId, ref toInsure);
            BusinessRules.GetSalesPrice(ProductApi, productId, ref toInsure);

            toInsure.InsuranceValue = 0f; //Do we ever need to do this?

            if (!toInsure.ProductTypeHasInsurance)
                return toInsure;

            switch(toInsure.SalesPrice)
            {
                case < 500:
                    toInsure.InsuranceValue = 0;
                    break;
                case >= 500 and < 2000:
                    toInsure.InsuranceValue += 1000;
                    break;
                case >= 2000:
                    toInsure.InsuranceValue += 2000;
                    break;
            }

            if (toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones") //TODO: Don't hardcode this, also we probably shouldn't do this using the name instead of the ID
                toInsure.InsuranceValue += 500;

            return toInsure;
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore]
            public string ProductTypeName { get; set; }
            [JsonIgnore]
            public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore]
            public float SalesPrice { get; set; }
        }

        private const string ProductApi = "http://localhost:5002";
    }
}