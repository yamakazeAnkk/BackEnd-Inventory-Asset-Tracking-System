using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetService.Domain.Entities
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public Guid Id { get; set; }
        [Required]
        [Column("category_name")]
        public string CategoryName { get; set; }

        public List<Asset> Assets { get; set; } = new List<Asset>(); // Navigation property to the list of assets associated with this brand
        public DateTime Created_at { get; set; } = DateTime.UtcNow;// Timestamp for when the asset was created
        public DateTime Updated_at { get; set; } // Timestamp for when the asset was last updated
    }
}
