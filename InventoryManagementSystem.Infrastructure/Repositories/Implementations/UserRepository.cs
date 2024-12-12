using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Infrastructure.Data;
using InventoryManagementSystem.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Infrastructure.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllPagedAsync(int page, int pageSize)
        {
            // Sayfalama mantığı
            return await _context.Users
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToListAsync();
        }
        public async Task<User> GetByEmailConfirmationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        }

    }
}
