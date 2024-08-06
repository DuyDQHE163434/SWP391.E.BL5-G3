using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly traveltestContext _context;

        public RestaurantsController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Restaurants.ToList());
        }

    }
}
