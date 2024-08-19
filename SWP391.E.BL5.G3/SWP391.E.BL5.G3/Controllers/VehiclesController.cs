using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult VehicleDetails(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = _context.Vehicles
                    .Include(item => item.Province)
                    .FirstOrDefault(item => item.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // Add a new vehicle
        public IActionResult AddVehicle()
        {
            var provinces = _context.Provinces.ToList();
            ViewData["Province"] = new SelectList(provinces, "ProvinceId", "ProvinceName");
            return View();
        }

        [HttpPost]
        public IActionResult AddVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListVehicles));
            }
            var provinces = _context.Provinces.ToList();
            ViewData["Province"] = new SelectList(provinces, "ProvinceId", "ProvinceName");
            return View(vehicle);
        }

        // Delete the selected vehicle
        [HttpPost]
        //[AllowAnonymous]
        public IActionResult DeleteVehicle(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = _context.Vehicles.FirstOrDefault(item => item.VehicleId == id);

            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ListVehicles));
        }
    }
}
