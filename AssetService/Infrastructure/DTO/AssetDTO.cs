using AssetService.Infrastructure.Entity;
using System.ComponentModel.DataAnnotations;

namespace AssetService.Infrastructure.DTO
{
    public record AssetDTO (
        Guid Id,
        [Required] string AssetCode,
        [Required] string AssetName,
        AssetStatus Status,
        bool IsFaulty 
        );

}
