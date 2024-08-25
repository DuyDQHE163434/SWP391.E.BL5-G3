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

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if(int.Parse(role) == (int)RoleEnum.Travel_Agent)
            {
                var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                vehicles = vehicles.Where(x => x.UserId == int.Parse(userID)).ToList();
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
                vehicle.VehicleName.Trim();

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
                    vehicle.VehicleName.Trim();

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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BookingVehicle(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound(); // Trả về NotFound nếu xe không tồn tại
            }

            // Tạo một đối tượng booking mới
            var bookingModel = new Booking
            {
                VehicleId = vehicleId, // Gán ID của xe vào booking
            };

            // Truyền giá của xe vào ViewBag để hiển thị trong view
            ViewBag.Price = vehicle.Price; // Đảm bảo trường Price trong Vehicle không phải là null
            ViewBag.MaxPeople = vehicle.NumberOfSeats; // Gán MaxPeople nếu có giới hạn

            return View(bookingModel); // Trả về view với bookingModel
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingVehicle([Bind("Name,Phone,StartDate,EndDate,Message,VehicleId")] Booking booking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy UserId từ Claims
            if (userId != null)
            {
                booking.UserId = Convert.ToInt32(userId); // Gán UserId từ Claims
            }
            else
            {
                ModelState.AddModelError("", "User is not logged in."); // Thêm lỗi nếu không có UserId
                return View(booking); // Nếu không có UserId, trả lại view
            }

            if (!ModelState.IsValid)
            {
                // Gán trạng thái ban đầu cho booking
                booking.Status = (int)BookingStatusEnum.Pending; // Trạng thái ban đầu

                // Lưu vào cơ sở dữ liệu
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt xe thành công!"; // Thông báo thành công
                return RedirectToAction("ViewVehicleList"); // Chuyển hướng về danh sách xe
            }

            // Nếu model không hợp lệ, truyền lại giá xe vào ViewBag
            ViewBag.Price = _context.Vehicles.Find(booking.VehicleId)?.Price; // Gán giá xe
            ViewBag.MaxPeople = _context.Vehicles.Find(booking.VehicleId)?.NumberOfSeats; // Gán MaxPeople

            return View(booking); // Trả lại thông tin booking và xe
        }


    }
}
