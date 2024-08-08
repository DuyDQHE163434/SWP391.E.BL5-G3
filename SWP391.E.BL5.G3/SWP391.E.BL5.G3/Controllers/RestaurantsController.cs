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

        // view list of restaurants
        public IActionResult Index(string currentSearchString, string searchString, int? page)
        {
            var restaurants = new List<Restaurant>();

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

            restaurants = restaurants.OrderBy(item => item.RestaurantName).ToList();

            return View(restaurants.ToPagedList(pageNumber, pageSize));
        }

        // view details of a restaurant
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

    }
}
