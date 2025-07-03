using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetService.Domain.Entities
{
    public class Asset
    {
        [Key]
        [Column("asset_id")]
        public Guid Id { get; set; }
        [Required, Column("asset_code")]
        public string AssetCode { get; set; }
        [Required, Column("asset_name")]
        public string AssetName { get; set; }
        public AssetStatus Status { get; set; } = AssetStatus.available;
        public bool IsFaulty { get; set; } = false; // Indicates if the asset is faulty

        public Guid BrandId { get; set; } // Foreign key to the Brand entity
        public Brand? Brand { get; set; } // Navigation property to the Brand entity
        public Guid CategoryId { get; set; } // Foreign key to the Category entity
        public Category? Category { get; set; } // Navigation property to the Category entity
        public Guid SupplierId { get; set; } // Foreign key to the Supplier entity
        public Supplier? Supplier { get; set; } // Navigation property to the Supplier entity
        public DateTime Created_at { get; set; } = DateTime.UtcNow; // Timestamp for when the asset was created
        public DateTime Updated_at { get; set; } // Timestamp for when the asset was last updated
    }

    public enum AssetStatus
    {
        available,
        inUse,
    }
}
