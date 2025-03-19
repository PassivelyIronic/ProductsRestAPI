using ProductAPI.Models;

namespace ProductAPI.Dtos
{
    public class ProductDto
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductCategory Category { get; set; }
    }
}
