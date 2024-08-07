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

        public IActionResult Index(string searchString, string currentSearchString, int? page)
        {
            var restaurants = new List<Restaurant>();

            // if search string is not null, page value will be 1
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentSearchString;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                restaurants = _context.Restaurants.Where(item =>
                    item.RestaurantName.Contains(searchString)).ToList();
            }
            else
            {
                restaurants = _context.Restaurants.ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            restaurants = restaurants.OrderByDescending(item => item.RestaurantName).ToList();

            return View(restaurants.ToPagedList(pageNumber, pageSize));
        }

    }
}
