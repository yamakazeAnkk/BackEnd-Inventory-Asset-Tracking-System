using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace AuthService.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(512)]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;
        [MaxLength(512)]
        public string? CreatedByIp { get; set; }

        [MaxLength(512)]
        public string? ReplacedByToken { get; set; }

        [MaxLength(512)]
        public string? UserAgent { get; set; }
    }
}
