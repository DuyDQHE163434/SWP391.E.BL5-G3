using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public int Email { get; set; }
        public string Password { get; set; } = null!;
        public string? Image { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public bool? Action { get; set; }
        public string? Description { get; set; }
        public bool? Gender { get; set; }
    }
}
