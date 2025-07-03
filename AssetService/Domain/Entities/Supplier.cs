using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetService.Domain.Entities
{
    public class Supplier
    {
        [Key]
        [Column("supplier_id")]
        public Guid Id { get; set; }
        [Required]
        [Column("supplier_name")]
        public string SupplierName { get; set; }
        [Required]
        [Column("supplier_contact")]
        public string SupplierContact { get; set; }

        public List<Asset> Assets { get; set; } = new List<Asset>(); // Navigation property to the list of assets associated with this supplier
        public DateTime Created_at { get; set; } = DateTime.UtcNow; // Timestamp for when the asset was created
        public DateTime Updated_at { get; set; } // Timestamp for when the asset was last updated
    }
}
