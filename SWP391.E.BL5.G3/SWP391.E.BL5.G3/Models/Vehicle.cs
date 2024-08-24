﻿using System;
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

        public string VehicleName { get; set; } = null!;
        public string? Image { get; set; }
        public string? Location { get; set; }

        public int? ProvinceId { get; set; }

        public string? ContactNumber { get; set; }
        public string? VehicleSupplier { get; set; }
        public bool? Transmission { get; set; }
        public int? VehicleType { get; set; }
        public int? BaggageNumber { get; set; }
        public int? NumberOfSeats { get; set; }
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
