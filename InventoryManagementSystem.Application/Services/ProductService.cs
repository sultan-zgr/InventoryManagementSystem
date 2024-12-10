using AutoMapper;
using InventoryManagementSystem.Application.DTOs.Product;
using InventoryManagementSystem.Application.Interfaces;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Repositories;

namespace InventoryManagementSystem.Application.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private const string ProductCacheKey = "products";

        public ProductService(ProductRepository productRepository, ICacheService cacheService, IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            // Redis'ten ürünleri getir
            var productsFromCache = await _cacheService.GetAsync<IEnumerable<ProductDTO>>(ProductCacheKey);
            if (productsFromCache != null)
                return productsFromCache;

            // Redis'te yoksa MongoDB'den al ve cache'e ekle
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products);

            await _cacheService.SetAsync(ProductCacheKey, productDtos, TimeSpan.FromMinutes(10));
            return productDtos;
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid id)
        {
            // Redis key'i ürün ID'ye göre oluştur
            var cacheKey = $"{ProductCacheKey}:{id}";
            var productFromCache = await _cacheService.GetAsync<ProductDTO>(cacheKey);

            if (productFromCache != null)
                return productFromCache;

            // Redis'te yoksa MongoDB'den al ve cache'e ekle
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            var productDto = _mapper.Map<ProductDTO>(product);
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));
            return productDto;
        }

        public async Task AddProductAsync(CreateProductDTO createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _productRepository.AddAsync(product);

            // Cache'i temizle
            await _cacheService.RemoveAsync(ProductCacheKey);
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found.");

            _mapper.Map(updateProductDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);

            // Cache'i temizle
            await _cacheService.RemoveAsync(ProductCacheKey);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            await _productRepository.DeleteAsync(id);

            // Cache'i temizle
            await _cacheService.RemoveAsync(ProductCacheKey);
        }
    }
}
