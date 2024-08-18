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
        public string RestaurantName { get; set; } = null!;
        public string? Image { get; set; }
        public string? Location { get; set; }
        public int ProvinceId { get; set; }
        public int? BusinessTypeId { get; set; }
        public int? CuisineTypeId { get; set; }
        public double? AveragePrice { get; set; }
        public string? PriceList { get; set; }
        public TimeSpan? OpenedTime { get; set; }
        public TimeSpan? ClosedTime { get; set; }
        public string? ContactNumber { get; set; }
        public string? Description { get; set; }
        public string? Summary { get; set; }
        public string? Parking { get; set; }
        public int? Rating { get; set; }
        public string? Regulations { get; set; }
        public string? Utilities { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual BusinessType? BusinessType { get; set; }
        public virtual CuisineType? CuisineType { get; set; }
        public virtual Province Province { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }

}
