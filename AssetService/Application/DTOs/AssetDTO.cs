using AssetService.Domain.Entities;
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
