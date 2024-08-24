using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = null!;
        public string? Image { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = null!;
        public double? Rate { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Tour> Tours { get; set; }
    }
}
