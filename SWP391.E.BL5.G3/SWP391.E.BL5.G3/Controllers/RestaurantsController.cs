using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;
using X.PagedList;

namespace SWP391.E.BL5.G3.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly traveltestContext _context;

        public RestaurantsController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            return View(DisplayPagedList(page, 5));
        }

        public IPagedList<Restaurant> DisplayPagedList(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            var restaurants = _context.Restaurants.ToPagedList(pageNumber, pageSize);
            return restaurants;
        }

    }
}
