using Microsoft.EntityFrameworkCore;
using AssetService.Infrastructure.Entity; // Importing the Asset entity

namespace AssetService.Infrastructure.Data
{
    public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options)
    {
        public DbSet<Asset> Assets { get; set; } // DbSet for Asset entity
    }
}
