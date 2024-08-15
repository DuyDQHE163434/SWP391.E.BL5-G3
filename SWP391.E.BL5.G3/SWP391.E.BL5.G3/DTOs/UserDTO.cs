using SWP391.E.BL5.G3.Enum;

namespace SWP391.E.BL5.G3.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleEnum Roles { get; set; }
    }
}
