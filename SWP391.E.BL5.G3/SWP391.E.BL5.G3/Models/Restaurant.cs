using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            Bookings = new HashSet<Booking>();
            Tours = new HashSet<Tour>();
        }

        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Restaurant name cannot be empty.")]
        public string RestaurantName { get; set; } = null!;
        public string? Location { get; set; }
        public int? ProvinceId { get; set; }
        public int? BusinessTypeId { get; set; }
        public int? CuisineTypeId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "The average price must be a non-negative number.")]
        public double? AveragePrice { get; set; }
        public string? PriceList { get; set; }
        public TimeSpan? OpenedTime { get; set; }
        public TimeSpan? ClosedTime { get; set; }
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(10, ErrorMessage = "The contact number cannot be longer than 10 characters.")]
        [MinLength(10, ErrorMessage = "The contact number must be at least 10 characters long.")]
        public string? ContactNumber { get; set; }
        public string? Description { get; set; }
        public string? Summary { get; set; }
        public string? Parking { get; set; }
        public double? Rating { get; set; }
        public string? Regulations { get; set; }
        public string? Utilities { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime? CreatedAt { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime? UpdatedAt { get; set; }
        public string? Image { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual BusinessType? BusinessType { get; set; }
        public virtual CuisineType? CuisineType { get; set; }
        public virtual Province? Province { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
