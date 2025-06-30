using System.ComponentModel.DataAnnotations.Schema;

namespace AssetService.Infrastructure.Entity
{
    public class Asset
    {
        public Guid Id { get; set; }
        public required string AssetCode { get; set; }
        public required string AssetName { get; set; }
        public AssetStatus Status { get; set; } = AssetStatus.available;
        public bool IsFaulty { get; set; } = false; // Indicates if the asset is faulty

        //public Guid Brand_id { get; set; } // Foreign key to the Brand entity
        //public Guid Category_id { get; set; } // Foreign key to the Category entity
        public DateTime Created_at { get; set; } // Timestamp for when the asset was created
        public DateTime Updated_at { get; set; } // Timestamp for when the asset was last updated

    }

    public enum AssetStatus
    {
        available,
        inUse,
    }
}
