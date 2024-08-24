using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumPeople { get; set; }
        public string? Message { get; set; }

        public int? TourId { get; set; }
        public int? HotelId { get; set; }
        public int? RestaurantId { get; set; }
        public int? VehicleId { get; set; }
        public int? ProvinceId { get; set; }

        public int? Status { get;set; }

        public virtual Hotel? Hotel { get; set; }
        public virtual Province? Province { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual Tour? Tour { get; set; }
        public virtual User? User { get; set; }
        public virtual Vehicle? Vehicle { get; set; }

        public virtual Room? Room { get; set; }
        public int? RoomId { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

    }
}
