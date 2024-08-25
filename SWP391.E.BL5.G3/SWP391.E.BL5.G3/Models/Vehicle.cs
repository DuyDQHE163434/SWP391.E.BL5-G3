using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            Bookings = new HashSet<Booking>();
            Tours = new HashSet<Tour>();
        }

        public int VehicleId { get; set; }
        [Required(ErrorMessage = "Vehicle Name name cannot be empty.")]
        public string VehicleName { get; set; } = null!;
        public string? Image { get; set; }
        public string? Location { get; set; }
        public int? ProvinceId { get; set; }
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(11, ErrorMessage = "The contact number cannot be longer than 11 characters.")]
        [MinLength(10, ErrorMessage = "The contact number must be at least 10 characters long.")]
        public string? ContactNumber { get; set; }
        public string? VehicleSupplier { get; set; }
        public bool? Transmission { get; set; }
        public int? VehicleType { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "The Baggage Number must be a non-negative number.")]
        public int? BaggageNumber { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "The Number Of Seats must be a non-negative number.")]
        public int? NumberOfSeats { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "The price must be a non-negative number.")]
        public double? Price { get; set; }
        public double? Rating { get; set; }
        public int? Status { get; set; }

        public string? Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime? CreatedAt { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime? UpdatedAt { get; set; }

        public int? UserId { get; set; }

        public virtual Province? Province { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
