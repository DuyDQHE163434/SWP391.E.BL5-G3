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
using System.Linq;
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
            else if (userRole == 1)
            {
                restaurants = _context.Restaurants
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
        public IActionResult AddRestaurant(Restaurant restaurant, IFormFile imageFile, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                restaurant.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var uploadedImageURLS = new List<string>();

                foreach (var fileImage in images)
                {
                    if (fileImage != null && fileImage.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            fileImage.CopyTo(stream);
                            stream.Position = 0;

                            var uploadImage = new ImageUploadParams()
                            {
                                File = new FileDescription(fileImage.FileName, stream)
                            };

                            var uploadResult = _cloudinary.Upload(uploadImage);
                            uploadedImageURLS.Add(uploadResult.SecureUrl.ToString());
                        }
                    }
                }

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

                restaurant.Rating = 5;

                restaurant.PriceList = string.Join(",", uploadedImageURLS);

                restaurant.CreatedAt = DateTime.Now;
                restaurant.UpdatedAt = restaurant.CreatedAt;

                _context.Add(restaurant);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Add Restaurant successfully!";
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
        public IActionResult EditRestaurant(int id, Restaurant restaurant, IFormFile imageFile, List<IFormFile> images)
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

                    var uploadedImageURLS = new List<string>();

                    foreach (var fileImage in images)
                    {
                        if (fileImage != null && fileImage.Length > 0)
                        {
                            using (var stream = new MemoryStream())
                            {
                                fileImage.CopyTo(stream);
                                stream.Position = 0;

                                var uploadImage = new ImageUploadParams()
                                {
                                    File = new FileDescription(fileImage.FileName, stream)
                                };

                                var uploadResult = _cloudinary.Upload(uploadImage);
                                uploadedImageURLS.Add(uploadResult.SecureUrl.ToString());
                                restaurant.PriceList = string.Join(",", uploadedImageURLS);
                            }
                        }
                        else
                        {
                            restaurant.PriceList = restaurant.PriceList;
                        }
                    }

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
                    else
                    {
                        restaurant.Image = restaurant.Image;
                    }

                    restaurant.Rating = restaurant.Rating;

                    restaurant.CreatedAt = restaurant.CreatedAt;
                    restaurant.UpdatedAt = DateTime.Now;

                    _context.Update(restaurant);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Update Restaurant successfully!";
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
                TempData["SuccessMessage"] = "Delete Restaurant successfully!";
            }

            return RedirectToAction(nameof(ListRestaurants));
        }

        // Check if the restaurant is existed or not
        public bool CheckRestaurantExisted(int id)
        {
            return (_context.Restaurants?.Any(item => item.RestaurantId == id)).GetValueOrDefault();
        }

        [Authorize(RoleEnum.Customer)]
        public IActionResult BookRestaurant(int id)
        {
            var restaurant = _context.Restaurants.Find(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // Tạo một đối tượng booking mới mặc định
            var booking = new Booking
            {
                RestaurantId = id
            };

            // Truyền tên nhà hàng vào ViewBag để hiển thị trong view
            ViewBag.RestaurantName = restaurant.RestaurantName;

            return View(booking);
        }

        [HttpPost]
        [Authorize(RoleEnum.Customer)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookRestaurant([Bind("Name,Phone,StartDate,EndDate,NumPeople,Message,RestaurantId")] Booking booking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ Claims

            if (!ModelState.IsValid)
            {
                // Gán UserId và trạng thái cho booking
                booking.UserId = Convert.ToInt32(userId);
                booking.Status = (int)BookingStatusEnum.Pending; // Trạng thái ban đầu
                booking.EndDate = booking.StartDate;

                // Lưu vào cơ sở dữ liệu
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt bàn thành công!"; // Thông báo thành công
                return RedirectToAction("ViewRestaurantList"); // Chuyển hướng về danh sách nhà hàng
            }

            // Nếu model không hợp lệ, truyền lại thông tin nhà hàng vào ViewBag
            ViewBag.RestaurantName = _context.Restaurants.Find(booking.RestaurantId)?.RestaurantName;

            return View(booking); // Trả lại thông tin booking và nhà hàng
        }
    }
}
