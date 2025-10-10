using EduMap.Models.Entities;

namespace EduMap.Models.Requests
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required Role Role { get; set; }
    }
}
