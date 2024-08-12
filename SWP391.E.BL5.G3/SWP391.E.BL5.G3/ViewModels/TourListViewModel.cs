using System.Collections.Generic;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.ViewModels
{
    public class TourListViewModel
    {
        public IEnumerable<Tour> Tours { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public string CurrentFilter { get; set; } // Thêm để giữ giá trị tìm kiếm hiện tại
    }
}
