using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Dtos;
using ProductAPI.Services;
using ProductAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProductAPI.Test
{
    public class ProductControllerTest
    {
        [Fact]
        public async Task GetAllProducts_ShouldReturnOkResult_WithProductList()
        {
            var testProducts = new List<ProductDto>
            {
                new ProductDto { Name = "Demo1", Price = 1.0M, Quantity = 5, Category = ProductCategory.Electronics },
                new ProductDto { Name = "Demo2", Price = 3.75M, Quantity = 10, Category = ProductCategory.Books }
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(testProducts);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.GetAllProducts() as OkObjectResult;

            Assert.NotNull(result);
            var products = result.Value as List<ProductDto>;
            Assert.Equal(testProducts.Count, products.Count);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOkResult_WithProduct()
        {
            var testProduct = new ProductDto { Name = "Demo3", Price = 15.99M, Quantity = 3, Category = ProductCategory.Clothing };
            var productId = 1;
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(testProduct);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.GetProductById(productId) as OkObjectResult;

            Assert.NotNull(result);
            var product = result.Value as ProductDto;
            Assert.Equal(testProduct.Name, product.Name);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var productId = 999;
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((ProductDto)null);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.GetProductById(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnCreatedAtActionResult_WhenProductIsValid()
        {
            
            var newProduct = new ProductDto { Name = "Demo4", Price = 9.99M, Quantity = 10, Category = ProductCategory.Books };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.AddProductAsync(newProduct)).Returns(Task.CompletedTask);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.AddProduct(newProduct) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal("GetProductById", result.ActionName);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnBadRequest_WhenProductIsInvalid()
        {
            var invalidProduct = new ProductDto { Name = "", Price = 0, Quantity = 0, Category = ProductCategory.Electronics };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.AddProductAsync(invalidProduct)).ThrowsAsync(new InvalidOperationException("Invalid product"));

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.AddProduct(invalidProduct);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenProductExists()
        {
            var productId = 1;
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.DeleteProduct(productId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var productId = 999;
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.DeleteProductAsync(productId)).ThrowsAsync(new InvalidOperationException("Product not found"));

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.DeleteProduct(productId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
