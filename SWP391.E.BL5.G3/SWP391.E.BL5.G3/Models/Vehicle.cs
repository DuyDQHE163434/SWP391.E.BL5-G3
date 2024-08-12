using System;
using System.Collections.Generic;

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
        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ProvinceId { get; set; }

        public virtual Province Province { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
