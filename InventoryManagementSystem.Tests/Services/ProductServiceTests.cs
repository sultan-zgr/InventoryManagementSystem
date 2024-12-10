using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using InventoryManagementSystem.Application.DTOs.Product;
using InventoryManagementSystem.Application.Interfaces;
using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using Moq;
using Xunit;

namespace InventoryManagementSystem.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();
            _productService = new ProductService(
                _productRepositoryMock.Object,
                _cacheServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_Should_Return_Products_From_Cache_When_Cache_Exists()
        {
            // Arrange
            var cachedProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = Guid.NewGuid(), Name = "Cached Product", Price = 100, Stock = 10 }
            };

            _cacheServiceMock.Setup(c => c.GetAsync<IEnumerable<ProductDTO>>("products"))
                .ReturnsAsync(cachedProducts);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(cachedProducts);
            _cacheServiceMock.Verify(c => c.GetAsync<IEnumerable<ProductDTO>>("products"), Times.Once);
            _productRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllProductsAsync_Should_Fetch_From_Database_When_Cache_Is_Empty()
        {
            // Arrange: Redis'ten boş veri döndürme
            _cacheServiceMock
                .Setup(c => c.GetAsync<IEnumerable<ProductDTO>>("products"))
                .ReturnsAsync((IEnumerable<ProductDTO>)null);

            // Arrange: MongoDB'den alınacak veri
            var products = new List<Product>
                    {
                        new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 100, Stock = 10 }
                    };

            var productDtos = new List<ProductDTO>
                    {
                        new ProductDTO { Id = products[0].Id, Name = "Product 1", Price = 100, Stock = 10 }
                    };

            // Repository ve Mapper Setup
            _productRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ProductDTO>>(products))
                .Returns(productDtos);

            // Cache'e kaydetme setup
            _cacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<ProductDTO>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(productDtos);

            // Doğrulama kontrolleri
            _cacheServiceMock.Verify(
                c => c.GetAsync<IEnumerable<ProductDTO>>("products"),
                Times.Once
            );

            _productRepositoryMock.Verify(
                repo => repo.GetAllAsync(),
                Times.Once
            );

            _cacheServiceMock.Verify(
                c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<ProductDTO>>(),
                    It.IsAny<TimeSpan>()
                ),
                Times.Once
            );
        }


        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Product_From_Cache_When_Cache_Exists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cachedProduct = new ProductDTO { Id = productId, Name = "Cached Product", Price = 100 };

            _cacheServiceMock.Setup(c => c.GetAsync<ProductDTO>($"products:{productId}"))
                .ReturnsAsync(cachedProduct);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.Should().BeEquivalentTo(cachedProduct);
            _cacheServiceMock.Verify(c => c.GetAsync<ProductDTO>($"products:{productId}"), Times.Once);
            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Never);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Fetch_From_Database_When_Cache_Is_Empty()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product 1", Price = 50, Stock = 5 };
            var productDto = new ProductDTO { Id = productId, Name = "Product 1", Price = 50, Stock = 5 };

            _cacheServiceMock.Setup(c => c.GetAsync<ProductDTO>($"products:{productId}"))
                .ReturnsAsync((ProductDTO)null);

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDto);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.Should().BeEquivalentTo(productDto);
            _cacheServiceMock.Verify(c => c.GetAsync<ProductDTO>($"products:{productId}"), Times.Once);
            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
            _cacheServiceMock.Verify(c => c.SetAsync($"products:{productId}", productDto, It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
