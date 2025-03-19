using ProductAPI.Dtos;
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductDto productDto);
        Task UpdateProductAsync(int id, ProductDto productDto);
        Task DeleteProductAsync(int id);
        //Task<bool> IsProductNameForbiddenAsync(string name);
    }

}
