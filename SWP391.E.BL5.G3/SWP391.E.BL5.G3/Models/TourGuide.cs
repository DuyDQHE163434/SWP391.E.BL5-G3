using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class TourGuide
    {
        public TourGuide()
        {
            Tours = new HashSet<Tour>();
        }

        public int TourGuideId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Rate { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Tour> Tours { get; set; }
    }
}
