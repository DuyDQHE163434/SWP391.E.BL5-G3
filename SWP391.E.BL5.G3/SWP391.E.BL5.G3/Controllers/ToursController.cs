using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;
using SWP391.E.BL5.G3.ViewModels;
using System.Security.Claims;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class ToursController : Controller
    {
        private readonly traveltestContext _context;

        public ToursController(traveltestContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[AllowAnonymous]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public async Task<IActionResult> ListTour(string searchString, int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;

            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var toursQuery = _context.Tours.Include(t => t.Province).AsQueryable();

            // Kiểm tra vai trò người dùng
            if (User.IsInRole(RoleEnum.Travel_Agent.ToString()))
            {
                // Chỉ lấy các tour mà Travel_Agent đã tạo
                toursQuery = toursQuery.Where(t => t.UserId.ToString() == userId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                toursQuery = toursQuery.Where(t => t.Name.Contains(searchString));
            }

            int pageSize = 5; // Số lượng tour trên mỗi trang
            var totalTours = await toursQuery.CountAsync(); // Tính tổng số tour

            var tours = await toursQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Lấy danh sách tour trên trang hiện tại

            var model = new TourListViewModel
            {
                Tours = tours,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalTours / (double)pageSize),
                CurrentFilter = searchString // Lưu giá trị tìm kiếm
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TourDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Province) // Bao gồm thông tin tỉnh
                .FirstOrDefaultAsync(m => m.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [HttpGet]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult CreateTour()
        {
            // Lấy danh sách tỉnh từ cơ sở dữ liệu
            var provinces = _context.Provinces.ToList(); // có thể sử dụng ToListAsync() cho async
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "ProvinceName");

            return View();
        }

        [HttpPost]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour([Bind("Name,Description,Price,ProvinceId,UserId")] Tour tour, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                tour.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (image != null && image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    tour.Image = uniqueFileName;
                }

                tour.CreateDate = DateTime.Now;

                // Thêm tour vào cơ sở dữ liệu
                _context.Add(tour);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListTour));
            }

            // Nếu model không hợp lệ, sẽ lấy lại danh sách tỉnh
            var provinces = _context.Provinces.ToList();
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "Name");

            return View(tour);
        }

        // Edit Tour
        [HttpGet]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public async Task<IActionResult> EditTour(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            var provinces = await _context.Provinces.ToListAsync();
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "ProvinceName", tour.ProvinceId); // Giữ lại tỉnh đã chọn

            return View(tour);
        }

        [HttpPost]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTour(int id, [Bind("TourId,Name,Image,Description,Price,Duration,AirPlane,Rating,Itinerary,Inclusions,Exclusions,GroupSize,Guide,ProvinceId,UserId")] Tour tour, IFormFile image)
        {
            if (id != tour.TourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tour.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (image != null && image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        Directory.CreateDirectory(uploadsFolder);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        tour.Image = uniqueFileName;
                    }
                    else
                    {
                        // Giữ lại tên file cũ nếu không upload ảnh mới
                        var existingTour = await _context.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.TourId == id);
                        tour.Image = existingTour.Image;
                    }

                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.TourId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListTour));
            }

            // Nếu model không hợp lệ, lấy lại danh sách tỉnh
            var provinces = await _context.Provinces.ToListAsync();
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "ProvinceName", tour.ProvinceId); // Giữ lại tỉnh đã chọn

            return View(tour);
        }



        // Delete Tour
        [HttpGet]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public async Task<IActionResult> DeleteTour(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        [HttpPost, ActionName("DeleteTour")]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTourConfirmed(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListTour));
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.TourId == id);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListTourForGuests(string searchString, int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;

            var toursQuery = _context.Tours.Include(t => t.Province).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                toursQuery = toursQuery.Where(t => t.Name.Contains(searchString));
            }

            int pageSize = 5; // Số lượng tour trên mỗi trang
            var totalTours = await toursQuery.CountAsync(); // Tính tổng số tour

            var tours = await toursQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Lấy danh sách tour trên trang hiện tại

            var model = new TourListViewModel
            {
                Tours = tours,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalTours / (double)pageSize),
                CurrentFilter = searchString // Lưu giá trị tìm kiếm
            };

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BookingTour(int tourId)
        {
            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
            {
                return NotFound(); // Nếu không tìm thấy tour
            }

            var bookingModel = new Booking
            {
                TourId = tourId
            };

            return View(bookingModel); // Chuyển thông tin tour cho view
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingTour([Bind("Name,Phone,StartDate,EndDate,NumPeople,Message,TourId,UserId")] Booking booking)
        {
            // Gán UserId từ Claim và chuyển đổi nó sang int
            //var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //booking.UserId = Convert.ToInt32(userIdString); // Chuyển đổi từ string sang int, có thể dùng int.Parse(userIdString) nếu bạn chắc chắn về kiểu

            // Kiểm tra tính hợp lệ cho model
            if (!ModelState.IsValid)
            {
                var bookingg = new Booking
                {
                    Name = booking?.Name,
                    Phone = booking?.Phone,
                    StartDate = booking?.StartDate,
                    EndDate = booking?.EndDate,
                    UserId = booking?.UserId,
                    NumPeople = booking?.NumPeople,
                    Message = booking?.Message,
                    TourId = booking?.TourId,
                    Status = (int)BookingStatusEnum.Pending
                };
                // Thêm booking vào cơ sở dữ liệu
                _context.Bookings.Add(bookingg);
                await _context.SaveChangesAsync();

                // Chuyển hướng về danh sách tour sau khi đặt
                return RedirectToAction(nameof(ListTourForGuests));
            }


            // Nếu model không hợp lệ, trả lại tương ứng với thông tin đã nhập
            return View(booking);
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingConfirm([Bind("Name,Phone,StartDate,EndDate,NumPeople,Message,TourId,UserId")] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                var bookingg = new Booking
                {
                    Name = booking?.Name,
                    Phone = booking?.Phone,
                    StartDate = booking?.StartDate,
                    EndDate = booking?.EndDate,
                    UserId = booking?.UserId,
                    NumPeople = booking?.NumPeople,
                    Message = booking?.Message,
                    TourId = booking?.TourId,
                };
                _context.Bookings.Add(bookingg);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListTourForGuests));
            }

            return View(booking);
        }


        [HttpGet]
        [Authorize] // Bảo vệ action này, chỉ cho phép người dùng đã xác thực
        public async Task<IActionResult> MyBookingTours()
        {
            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truy vấn các booking của người dùng
            var bookings = await _context.Bookings
                .Where(b => b.UserId.ToString() == userId) // Hoặc điều chỉnh theo cách bạn lưu userId
                .Include(b => b.Tour) // Bao gồm thông tin tour
                .ToListAsync();

            return View("MyBookingTours", bookings); // Đảm bảo trả về view mới
        }

    }
}
