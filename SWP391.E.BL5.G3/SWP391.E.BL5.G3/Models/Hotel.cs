using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Hotel
    {
        public Hotel()
        {
            Bookings = new HashSet<Booking>();
            Tours = new HashSet<Tour>();
            Rooms = new HashSet<Room>(); // Khởi tạo danh sách phòng
        }

        public int HotelId { get; set; }

        [Required(ErrorMessage = "Hotel Name is required.")]
        [StringLength(30, ErrorMessage = "Hotel Name cannot be longer than 30 characters.")]
        public string HotelName { get; set; } = null!;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(30, ErrorMessage = "Location cannot be longer than 30 characters.")]
        public string Location { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Description cannot be longer than 150 characters.")]
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? Image { get; set; } = null!;

        [Required(ErrorMessage = "Status is required.")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal? Price { get; set; }

        public int? BookingCount { get; set; }

        public decimal? Rating { get; set; }

        public int? ProvinceId { get; set; }

        public virtual Province? Province { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } // Thêm danh sách phòng

        
    }
}
