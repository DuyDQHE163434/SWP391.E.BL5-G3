using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;
using X.PagedList;

namespace SWP391.E.BL5.G3.Controllers
{
    public class HotelsController : Controller
    {
        private readonly traveltestContext _context;

        public HotelsController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            return View(DisplayPagedList(page, 5));
        }

        public IPagedList<Hotel> DisplayPagedList(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            var hotels = _context.Hotels.ToPagedList(pageNumber, pageSize);
            return hotels;
        }

    }
}
