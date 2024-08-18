using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class District
    {
        public District()
        {
            Provinces = new HashSet<Province>();
        }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;

        public virtual ICollection<Province> Provinces { get; set; }
    }
}
