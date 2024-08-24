using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;
using System.Security.Claims;

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
        [AllowAnonymous]

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
            var vehicles = _context.Vehicles
                    .Include(item => item.Province)
                    .ToList();

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
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult AddVehicle(Vehicle vehicle, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                vehicle.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.CopyTo(stream);
                        stream.Position = 0;

                        var imageUpload = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, stream)
                        };

                        var uploadResult = _cloudinary.Upload(imageUpload);
                        vehicle.Image = uploadResult.SecureUrl.ToString();
                    }
                }

                vehicle.Rating = 5;
                vehicle.CreatedAt = DateTime.Now;
                vehicle.UpdatedAt = vehicle.CreatedAt;

                _context.Add(vehicle);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Add Vehicle successfully!";
                return RedirectToAction(nameof(ListVehicles));
            }

            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");
            return View(vehicle);
        }

        // Edit the selected restaurant
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult EditVehicle(int? id)
        {
            if (id == null || _context.Vehicles == null)
            {
                return NotFound();
            }

            var vehicle = _context.Vehicles.Find(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

            return View(vehicle);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult EditVehicle(int id, Vehicle vehicle, IFormFile imageFile)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vehicle.UserId = vehicle.UserId;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageFile.CopyTo(stream);
                            stream.Position = 0;

                            var imageUpload = new ImageUploadParams()
                            {
                                File = new FileDescription(imageFile.FileName, stream)
                            };

                            var uploadResult = _cloudinary.Upload(imageUpload);
                            vehicle.Image = uploadResult.SecureUrl.ToString();
                        }
                    }
                    else
                    {
                        vehicle.Image = vehicle.Image;
                    }

                    vehicle.Rating = vehicle.Rating;

                    vehicle.CreatedAt = vehicle.CreatedAt;
                    vehicle.UpdatedAt = DateTime.Now;

                    _context.Update(vehicle);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Update Vehicle successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckVehicleExisted(vehicle.VehicleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListVehicles));
            }

            ViewData["BusinessType"] = new SelectList(_context.BusinessTypes.ToList(), "BusinessTypeId", "BusinessTypeName");
            ViewData["CuisineType"] = new SelectList(_context.CuisineTypes.ToList(), "CuisineTypeId", "CuisineTypeName");
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

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
                TempData["SuccessMessage"] = "Delete Vehicle successfully!";
            }

            return RedirectToAction(nameof(ListVehicles));
        }

        // Check if the vehicle is existed or not
        public bool CheckVehicleExisted(int id)
        {
            return (_context.Vehicles?.Any(item => item.VehicleId == id)).GetValueOrDefault();
        }
    }
}
