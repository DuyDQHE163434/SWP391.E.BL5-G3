using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class District
    {

        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;
        public int ProvinceId { get; set; }

        public virtual Province Province { get; set; } = null!;
    }
}
