using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumPeople { get; set; }
        public string Message { get; set; }
        public int? TourId { get; set; }
        public int? HotelId { get; set; }
        public int? RestaurantId { get; set; }
        public int? VehicleId { get; set; }
        public int? ProvinceId { get; set; }

        public virtual Hotel? Hotel { get; set; }
        public virtual Province? Province { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual Tour? Tour { get; set; }
        public virtual User User { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
    }
}
