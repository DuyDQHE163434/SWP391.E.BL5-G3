using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;
using X.PagedList;
using System.Threading.Tasks;

namespace SWP391.E.BL5.G3.Controllers
{
    public class HotelsController : Controller
    {
        private readonly traveltestContext _context;

        public HotelsController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, string filter = "None")
        {
            // Lấy tất cả khách sạn từ cơ sở dữ liệu
            var hotels = _context.Hotels.AsQueryable();

            // Lọc theo tiêu chí được chọn
            switch (filter)
            {
                case "HighestPrice":
                    hotels = hotels.OrderByDescending(h => h.Price);
                    break;
                case "LowestPrice":
                    hotels = hotels.OrderBy(h => h.Price);
                    break;
                case "MostBooked":
                    hotels = hotels.OrderByDescending(h => h.BookingCount);
                    break;
                default:
                    // Không sắp xếp gì nếu filter là "None"
                    hotels = hotels.OrderBy(h => h.HotelId);
                    break;
            }

            // Chuyển đổi danh sách khách sạn thành phân trang
            var pagedHotels = hotels.ToPagedList(page, 5);

            // Lưu giá trị filter vào ViewData để sử dụng trong View
            ViewData["CurrentFilter"] = filter;

            // Hiển thị thông báo thành công hoặc lỗi nếu có
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(pagedHotels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelName,Location,Description,Image,Status,Price")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    hotel.BookingCount = 0; // Khởi tạo số lần đặt phòng
                    _context.Add(hotel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Hotel added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    TempData["ErrorMessage"] = "Failed to add hotel. Please try again.";
                }
            }
            return View(hotel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,HotelName,Location,Description,Image,Status,Price")] Hotel hotel)
        {
            if (id != hotel.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Hotel updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.HotelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update hotel. Please try again.";
                        throw;
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "Failed to update hotel. Please try again.";
                }
            }
            return View(hotel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            try
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hotel deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to delete hotel. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            // Nếu phòng đã được đặt thì không cho đặt lại
            if (hotel.Status) // Phòng đã được đặt
            {
                TempData["ErrorMessage"] = "This room is already booked.";
                return RedirectToAction("Error");
            }

            // Đánh dấu phòng đã được đặt
            hotel.Status = true;
            hotel.BookingCount++; // Tăng số lần đặt phòng
            _context.Update(hotel);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room booked successfully!";
            return RedirectToAction("BookingSuccess", new { id = hotel.HotelId });
        }

        public IActionResult BookingSuccess(int id)
        {
            var hotel = _context.Hotels.Find(id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        public IActionResult Error()
        {
            ViewBag.Message = TempData["ErrorMessage"];
            return View();
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
    }
}
