using System;

namespace AuthService.Application.Dtos.User
{
    public class UserWriteDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid? DepartmentId { get; set; }
        public short? PositionLevel { get; set; }
        public string LocationCode { get; set; }
    }
}
