using System.ComponentModel.DataAnnotations;

namespace Insurance.Api.Data.Models
{
    public class ProductTypeSurchargeRule
    {
        [Key]
        public int ProductTypeId { get; set; }

        public int FlatItemSurcharge { get; set; }
        public int FlatCartSurcharge { get; set; }
        public int PercentageItemSurcharge { get; set; }

        public ProductTypeSurchargeRule(int productTypeId)
        {
            ProductTypeId = productTypeId;
        }

        public ProductTypeSurchargeRule()
        {
        }
    }
}
