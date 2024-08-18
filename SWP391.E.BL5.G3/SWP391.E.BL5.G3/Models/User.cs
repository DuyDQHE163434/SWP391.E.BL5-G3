using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class User
    {
        public User()
        {
            Bookings = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
            Tours = new HashSet<Tour>(); // Thêm mục này để lưu các tour mà người dùng đã tạo
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Image { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public bool? Action { get; set; }
        public string? Description { get; set; }
        public bool? Gender { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Tour> Tours { get; set; } // Thêm phần này

    }
}
