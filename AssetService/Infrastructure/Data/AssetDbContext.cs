using Microsoft.EntityFrameworkCore;
using AssetService.Domain.Entities; // Importing the Asset entity

namespace AssetService.Infrastructure.Data
{
    public class AssetDbContext(DbContextOptions<AssetDbContext> options) : DbContext(options)
    {
        public DbSet<Asset> Assets { get; set; } // DbSet for Asset entity
        public DbSet<Category> Categories { get; set; } // DbSet for Category entity
        public DbSet<Brand> Brands { get; set; } // DbSet for Brand entity
        public DbSet<Supplier> Suppliers { get; set; } // DbSet for Supplier entity

    }
}
