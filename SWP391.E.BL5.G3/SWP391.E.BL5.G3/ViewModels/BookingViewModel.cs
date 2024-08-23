using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public class BookingViewModel
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int? UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int NumPeople { get; set; }

        public string Message { get; set; }
    }
}
