using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class UserLoginHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public DateTime LoginAt { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string IpAddress { get; set; }

        [Required]
        [MaxLength(20)]
        public string Outcome { get; set; }
    }
} 