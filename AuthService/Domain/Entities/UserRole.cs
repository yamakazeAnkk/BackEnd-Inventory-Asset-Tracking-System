using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public Guid? ScopeDepartmentId { get; set; }
        public Department ScopeDepartment { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
} 