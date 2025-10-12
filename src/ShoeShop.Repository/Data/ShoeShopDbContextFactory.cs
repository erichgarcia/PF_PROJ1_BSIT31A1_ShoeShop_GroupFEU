using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoeShop.Repository.Data
{
    public class ShoeShopDbContextFactory : IDesignTimeDbContextFactory<ShoeShopDbContext>
    {
        public ShoeShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();
            optionsBuilder.UseSqlite("Data Source=shoeshop.db");

            return new ShoeShopDbContext(optionsBuilder.Options);
        }
    }
}