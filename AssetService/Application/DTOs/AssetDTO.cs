using AssetService.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace AssetService.Application.DTOs
{
    public record AssetDTO(
        Guid Id,
        [Required] string AssetCode,
        [Required] string AssetName,
        AssetStatus Status,
        bool IsFaulty
        );

}
