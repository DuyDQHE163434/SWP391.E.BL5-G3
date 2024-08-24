using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Tour
    {
        public Tour()
        {
            Bookings = new HashSet<Booking>();
        }

        public int TourId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string? Name { get; set; }
        public string? Image { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? VehicleId { get; set; }
        public int? HotelId { get; set; }
        public string? AirPlane { get; set; }
        public decimal? Rating { get; set; }
        public string? Duration { get; set; }
        public bool? Status { get; set; }
        public int? StaffId { get; set; }
        public int? RestaurantId { get; set; }
        public string? Itinerary { get; set; }
        public string? Inclusions { get; set; }
        public string? Exclusions { get; set; }
        public int? GroupSize { get; set; }
        public string? Guide { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        public int? ProvinceId { get; set; }

        public int? UserId { get; set; } // Thêm thuộc tính UserId


        public virtual User User { get; set; } // Thêm phần này


        

        public virtual Hotel? Hotel { get; set; }
        public virtual Province? Province { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual TourGuide? Staff { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
