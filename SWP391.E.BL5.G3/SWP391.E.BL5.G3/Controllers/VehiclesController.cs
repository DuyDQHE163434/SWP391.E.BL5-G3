using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly traveltestContext _context;

        public VehiclesController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult ListVehicles(string currentSearchString, string searchString, int? page)
        {
            var vehicles = new List<Vehicle>();

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
                vehicles = _context.Vehicles.Where(item =>
                    item.Location
                    .Contains(searchString))
                    .Include(item => item.Province)
                    .ToList();
            }
            else
            {
                vehicles = _context.Vehicles
                    .Include(item => item.Province)
                    .ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var totalItems = _context.Restaurants.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            vehicles = vehicles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(vehicles);
        }
    }
}
