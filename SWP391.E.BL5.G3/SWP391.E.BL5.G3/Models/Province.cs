using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Province
    {
        public Province()
        {
            Bookings = new HashSet<Booking>();
            Hotels = new HashSet<Hotel>();
            Restaurants = new HashSet<Restaurant>();
            Tours = new HashSet<Tour>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
       // public int DistrictId { get; set; }

        //public virtual District District { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Hotel> Hotels { get; set; }
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
