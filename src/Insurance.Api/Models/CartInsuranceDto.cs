using System.Collections.Generic;
using System.Linq;

namespace Insurance.Api.Models
{
    public class CartInsuranceDto
    {
        public float TotalInsuranceValue { get; set; }
        public List<InsuranceDto> InsuredProducts { get; set; }

        public CartInsuranceDto(List<InsuranceDto> insuredProducts)
        {
            InsuredProducts = insuredProducts;
            TotalInsuranceValue = InsuredProducts.Sum(x => x.InsuranceValue);
        }
    }
}
