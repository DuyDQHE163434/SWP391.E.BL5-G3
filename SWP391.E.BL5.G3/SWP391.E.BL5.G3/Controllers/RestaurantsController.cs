using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;
using System.Security.Claims;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class RestaurantsController : Controller
    {
        private readonly traveltestContext _context;
        private readonly Cloudinary _cloudinary;

        public RestaurantsController(traveltestContext context, IOptions<CloudinarySettings> cloudinarySettings)
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

        // View restaurant list (User role: Guest, Customer)
        [AllowAnonymous]
        public IActionResult ViewRestaurantList(string currentSearchString, string searchString, int? page)
        {
            var restaurants = _context.Restaurants
                    .Include(item => item.BusinessType)
                    .Include(item => item.CuisineType)
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
                restaurants = _context.Restaurants.Where(item => item.RestaurantName.Contains(searchString)).ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var totalItems = _context.Restaurants.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            restaurants = restaurants
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(restaurants);
        }

        // View details of the selected restaurant (User role: Guest, Customer)
        [AllowAnonymous]
        public IActionResult ViewRestaurantDetails(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants
                .Include(item => item.BusinessType)
                .Include(item => item.CuisineType)
                .Include(item => item.Province)
                .FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // View restaurant list (User role: Admin, Travel Agent)
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult ListRestaurants(string currentSearchString, string searchString, int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userRole = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Role));

            var restaurants = _context.Restaurants
                    .Include(item => item.BusinessType)
                    .Include(item => item.CuisineType)
                    .Include(item => item.Province)
                    .ToList();

            if (userRole == 2)
            {
                restaurants = _context.Restaurants
                    .Where(item => item.UserId.ToString() == userId)
                    .ToList();
            }

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
                    item.RestaurantName
                    .Contains(searchString))
                    .ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var totalItems = _context.Restaurants.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            restaurants = restaurants
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(restaurants);
        }

        // View details of the selected restaurant (User role: Admin, Travel Agent)
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult RestaurantDetails(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants
                .Include(item => item.BusinessType)
                .Include(item => item.CuisineType)
                .Include(item => item.Province)
                .FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // Add a new restaurant
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult AddRestaurant()
        {
            ViewData["BusinessType"] = new SelectList(_context.BusinessTypes.ToList(), "BusinessTypeId", "BusinessTypeName");
            ViewData["CuisineType"] = new SelectList(_context.CuisineTypes.ToList(), "CuisineTypeId", "CuisineTypeName");
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult AddRestaurant(Restaurant restaurant, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                restaurant.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

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
                        restaurant.Image = uploadResult.SecureUrl.ToString();
                    }
                }

                restaurant.CreatedAt = DateTime.Now;
                restaurant.UpdatedAt = restaurant.CreatedAt;

                _context.Add(restaurant);
                _context.SaveChanges();
                return RedirectToAction(nameof(ListRestaurants));
            }

            ViewData["BusinessType"] = new SelectList(_context.BusinessTypes.ToList(), "BusinessTypeId", "BusinessTypeName");
            ViewData["CuisineType"] = new SelectList(_context.CuisineTypes.ToList(), "CuisineTypeId", "CuisineTypeName");
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

            return View(restaurant);
        }

        // Edit the selected restaurant
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult EditRestaurant(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.Find(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            ViewData["BusinessType"] = new SelectList(_context.BusinessTypes.ToList(), "BusinessTypeId", "BusinessTypeName");
            ViewData["CuisineType"] = new SelectList(_context.CuisineTypes.ToList(), "CuisineTypeId", "CuisineTypeName");
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

            return View(restaurant);
        }

        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult EditRestaurant(int id, Restaurant restaurant, IFormFile? imageFile)
        {
            if (id != restaurant.RestaurantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    restaurant.UserId = restaurant.UserId;

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
                            restaurant.Image = uploadResult.SecureUrl.ToString();
                        }
                    }

                    restaurant.CreatedAt = restaurant.CreatedAt;
                    restaurant.UpdatedAt = DateTime.Now;

                    _context.Update(restaurant);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckRestaurantExisted(restaurant.RestaurantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListRestaurants));
            }

            ViewData["BusinessType"] = new SelectList(_context.BusinessTypes.ToList(), "BusinessTypeId", "BusinessTypeName");
            ViewData["CuisineType"] = new SelectList(_context.CuisineTypes.ToList(), "CuisineTypeId", "CuisineTypeName");
            ViewData["Province"] = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");

            return View(restaurant);
        }

        // Delete the selected restaurant
        [HttpPost]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult DeleteRestaurant(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ListRestaurants));
        }

        // Check if the restaurant is existed or not
        public bool CheckRestaurantExisted(int id)
        {
            return (_context.Restaurants?.Any(item => item.RestaurantId == id)).GetValueOrDefault();
        }

        // Being fixed
        // Book a restaurant
        [Authorize(RoleEnum.Customer)]
        public IActionResult BookRestaurant(int id)
        {
            var restaurant = _context.Restaurants.Find(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var booking = new Booking
            {
                RestaurantId = id
            };

            return View(booking);
        }

        [HttpPost]
        [Authorize(RoleEnum.Customer)]
        public IActionResult BookRestaurant(Booking booking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                booking.UserId = Convert.ToInt32(userId);
                booking.Status = (int)BookingStatusEnum.Pending;

                _context.Bookings.Add(booking);
                return RedirectToAction(nameof(ViewRestaurantList));
            }

            return View(booking);
        }
    }
}
