using System;
using System.Collections.Generic;

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
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Image { get; set; }
        public int? ProvinceId { get; set; }
        public string CuisineType { get; set; }
        public string ContactNumber { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }

}
