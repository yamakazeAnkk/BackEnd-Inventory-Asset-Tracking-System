using System;

namespace AuthService.Application.Dtos.User
{
    public class UserReadDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid? DepartmentId { get; set; }
        public short? PositionLevel { get; set; }
        public string LocationCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
