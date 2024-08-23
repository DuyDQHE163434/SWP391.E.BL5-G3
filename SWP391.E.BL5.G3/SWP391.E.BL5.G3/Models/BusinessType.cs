using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class BusinessType
    {
        public BusinessType()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public int BusinessTypeId { get; set; }
        public string BusinessTypeName { get; set; } = null!;

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
