using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly traveltestContext _context;
        private readonly Cloudinary _cloudinary;

        public VehiclesController(traveltestContext context, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _context = context;
            var cloudinarySettingsValue = cloudinarySettings.Value;
            var account = new Account(
                cloudinarySettingsValue.CloudName,
                cloudinarySettingsValue.ApiKey,
                cloudinarySettingsValue.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        // View vehicle list (User role: Guest, Customer)
        [AllowAnonymous]
        public IActionResult ViewVehicleList(string currentSearchString, string searchString, int? page)
        {
            var vehicles = _context.Vehicles.Include(item => item.Province).ToList();

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
                    .ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var totalItems = _context.Vehicles.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            vehicles = vehicles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(vehicles);
        }

        // View details of the selected vehicle (User role: Guest, Customer)
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult ViewVehicleDetails(int? id)
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

        // View vehicle list (User role: Admin, Travel Agent)
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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

        // View details of the selected vehicle (User role: Admin, Travel Agent)
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult AddVehicle()
        {
            var provinces = _context.Provinces.ToList();
            ViewData["Province"] = new SelectList(provinces, "ProvinceId", "ProvinceName");
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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
