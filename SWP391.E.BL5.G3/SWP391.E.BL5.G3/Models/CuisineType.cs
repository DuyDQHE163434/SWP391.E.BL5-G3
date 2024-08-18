using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class CuisineType
    {
        public CuisineType()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public int CuisineTypeId { get; set; }
        public string CuisineTypeName { get; set; } = null!;

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
