using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;
using SWP391.E.BL5.G3.ViewModels;

namespace SWP391.E.BL5.G3.Controllers
{
    public class ToursController : Controller
    {
        private readonly traveltestContext _context;

        public ToursController(traveltestContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListTour(string searchString, int pageNumber = 1)
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
        [Authorize(Roles = "Admin,Travel_Agent")]
        public IActionResult CreateTour()
        {
            // Lấy danh sách tỉnh từ cơ sở dữ liệu
            var provinces = _context.Provinces.ToList(); // có thể sử dụng ToListAsync() cho async
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "ProvinceName");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Travel_Agent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour([Bind("Name,Description,Price,ProvinceId")] Tour tour, IFormFile image)
        {
            if (ModelState.IsValid)
            {
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
        [Authorize(Roles = "Admin,Travel_Agent")]
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
        [Authorize(Roles = "Admin,Travel_Agent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTour(int id, [Bind("TourId,Name,Image,Description,Price,Duration,AirPlane,Rating,Itinerary,Inclusions,Exclusions,GroupSize,Guide,ProvinceId")] Tour tour, IFormFile image)
        {
            if (id != tour.TourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
        [Authorize(Roles = "Admin,Travel_Agent")]
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
        [Authorize(Roles = "Admin,Travel_Agent")]
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
    }
}
