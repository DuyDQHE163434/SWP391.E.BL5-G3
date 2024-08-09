using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Hotel
    {
        public Hotel()
        {
            Bookings = new HashSet<Booking>();
            Tours = new HashSet<Tour>();
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

        [Required(ErrorMessage = "Image URL is required.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        public int BookingCount { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
