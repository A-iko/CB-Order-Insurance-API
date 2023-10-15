using System.Collections.Generic;
using System.Linq;

namespace Insurance.Api.Models
{
    public class CartInsuranceDto
    {
        public decimal TotalInsuranceValue { get; set; } = 0;
        public decimal CartSurchargeValue { get; set; } = 0;
        public List<InsuranceDto> InsuredProducts { get; set; }
    }
}
