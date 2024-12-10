using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Bağlantı dizesini buraya ekleyin (veya çevresel değişkenlerden okuyabilirsiniz)
            optionsBuilder.UseNpgsql("Host=localhost;Database=InventoryDB;Username=postgres;Password=password");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
