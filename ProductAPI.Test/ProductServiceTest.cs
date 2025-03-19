using AutoMapper;
using Moq;
using ProductAPI.Data;
using ProductAPI.Dtos;
using ProductAPI.Models;
using ProductAPI.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ProductAPI.Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IForbiddenWordRepository> _forbiddenWordRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _forbiddenWordRepositoryMock = new Mock<IForbiddenWordRepository>();
            _mapperMock = new Mock<IMapper>();
            _productService = new ProductService(
                _productRepositoryMock.Object,
                _forbiddenWordRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenInvalidPrice()
        {
            var productDto = new ProductDto { Name = "Product", Price = 60000, Quantity = 10, Category = ProductCategory.Electronics };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.AddProductAsync(productDto));
            Assert.Equal("Price for Electronics must be between 50 and 50000.", exception.Message);
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenNegativeQuantity()
        {
            var productDto = new ProductDto { Name = "Product", Price = 100, Quantity = -1, Category = ProductCategory.Electronics };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.AddProductAsync(productDto));
            Assert.Equal("Quantity cannot be negative.", exception.Message);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProduct_WhenValidData()
        {
            var productDto = new ProductDto { Name = "ValidProduct", Price = 100, Quantity = 10, Category = ProductCategory.Electronics };
            _productRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            await _productService.AddProductAsync(productDto);

            _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            var productDto = new ProductDto { Name = "Product", Price = 100, Quantity = 10, Category = ProductCategory.Electronics };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.UpdateProductAsync(1, productDto));
            Assert.Equal("Product not found.", exception.Message);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            var productId = 999;
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.DeleteProductAsync(productId));
            Assert.Equal("Product not found.", exception.Message);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
        {
            var productId = 1;
            var existingProduct = new Product { Id = productId, Name = "TestProduct", Price = 100, Quantity = 5, Category = ProductCategory.Electronics };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _productRepositoryMock.Setup(repo => repo.DeleteAsync(productId)).Returns(Task.CompletedTask);

            await _productService.DeleteProductAsync(productId);

            _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);

            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenProductNameIsForbidden()
        {
            var forbiddenWord = "Trash";
            var productDto = new ProductDto { Name = forbiddenWord, Price = 100, Quantity = 10, Category = ProductCategory.Electronics };

            _forbiddenWordRepositoryMock.Setup(repo => repo.IsForbiddenWordAsync(forbiddenWord)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.AddProductAsync(productDto));
            Assert.Equal("Product name contains a forbidden word.", exception.Message);
        }


    }
}
