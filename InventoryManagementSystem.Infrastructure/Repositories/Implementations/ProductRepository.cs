using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Data;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace InventoryManagementSystem.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MongoDbContext _dbContext;

        public ProductRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.InsertOneAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            await _dbContext.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dbContext.Products.DeleteOneAsync(p => p.Id == id);
        }
    }
}
