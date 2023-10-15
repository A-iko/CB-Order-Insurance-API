using System.Text.Json.Serialization;

namespace Insurance.Api.Models.ProductApi
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("salesPrice")]
        public decimal SalesPrice { get; set; }
        [JsonPropertyName("productTypeId")]
        public int ProductTypeId { get; set; }
        [JsonIgnore]
        public ProductType? ProductType { get; set; }
    }
}
