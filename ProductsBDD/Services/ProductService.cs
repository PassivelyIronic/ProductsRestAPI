using AutoMapper;
using ProductAPI.Data;
using ProductAPI.Dtos;
using ProductAPI.Models;
using System.Text.RegularExpressions;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IForbiddenWordRepository _forbiddenWordRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository,
                              IForbiddenWordRepository forbiddenWordRepository,
                              IMapper mapper)
        {
            _productRepository = productRepository;
            _forbiddenWordRepository = forbiddenWordRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                Category = p.Category
            }).ToList();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category
            };
        }

        public async Task AddProductAsync(ProductDto productDto)
        {
            await ValidateProductNameAsync(productDto.Name);

            await ValidatePriceAsync(productDto.Category, productDto.Price);

            await ValidateQuantityAsync(productDto.Quantity);

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Category = productDto.Category
            };

            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(int id, ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            await ValidateProductNameAsync(productDto.Name);

            await ValidatePriceAsync(productDto.Category, productDto.Price);

            await ValidateQuantityAsync(productDto.Quantity);

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            product.Category = productDto.Category;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            await _productRepository.DeleteAsync(id);
        }

        private async Task ValidateProductNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Product name is required.");

            if (name.Length < 3 || name.Length > 20)
                throw new InvalidOperationException("Product name must be between 3 and 20 characters.");

            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]+$"))
                throw new InvalidOperationException("Product name can only contain letters and numbers.");

            var isForbidden = await _forbiddenWordRepository.IsForbiddenWordAsync(name);
            if (isForbidden)
                throw new InvalidOperationException("Product name contains a forbidden word.");

            var existingProduct = await _productRepository.GetByNameAsync(name);
            if (existingProduct != null)
                throw new InvalidOperationException("A product with this name already exists.");
        }

        private async Task ValidatePriceAsync(ProductCategory category, decimal price)
        {
            decimal minPrice = 0, maxPrice = 0;

            switch (category)
            {
                case ProductCategory.Electronics:
                    minPrice = 50;
                    maxPrice = 50000;
                    break;
                case ProductCategory.Books:
                    minPrice = 5;
                    maxPrice = 500;
                    break;
                case ProductCategory.Clothing:
                    minPrice = 10;
                    maxPrice = 5000;
                    break;
                default:
                    throw new InvalidOperationException("Invalid category.");
            }

            if (price < minPrice || price > maxPrice)
                throw new InvalidOperationException($"Price for {category} must be between {minPrice} and {maxPrice}.");
        }

        private Task ValidateQuantityAsync(int quantity)
        {
            if (quantity < 0)
                throw new InvalidOperationException("Quantity cannot be negative.");

            return Task.CompletedTask;
        }

        public async Task<bool> IsProductNameForbiddenAsync(string name)
        {
            return await _forbiddenWordRepository.IsForbiddenWordAsync(name);
        }
    }
}



