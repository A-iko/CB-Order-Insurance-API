using System.ComponentModel.DataAnnotations;

namespace Insurance.Api.Data.Models
{
    public class ProductTypeSurchargeRule
    {
        [Key]
        public int ProductTypeId { get; set; }

        public decimal FlatItemSurcharge { get; set; }
        public decimal FlatCartSurcharge { get; set; }
        public decimal PercentageItemSurcharge { get; set; }

        public ProductTypeSurchargeRule(int productTypeId)
        {
            ProductTypeId = productTypeId;
        }

        public ProductTypeSurchargeRule()
        {
        }
    }
}
