using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
<<<<<<< Updated upstream
        public string Password { get; set; } = null!;
=======
        public string? Password { get; set; }
>>>>>>> Stashed changes
        public string? Image { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public bool? Action { get; set; }
        public string? Description { get; set; }
        public bool? Gender { get; set; }
<<<<<<< Updated upstream
=======

        // Không cần thêm ProfileImage vào đây nếu bạn chỉ dùng nó trong form mà không cần lưu vào cơ sở dữ liệu
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
>>>>>>> Stashed changes
    }
}
