using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SWP391.E.BL5.G3.Controllers
{
    public class BookingController : Controller
    {
        private readonly traveltestContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(traveltestContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

       
        // GET: Booking/Create/5
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create(int roomId)
        {
            _logger.LogInformation("Quá trình đặt phòng bắt đầu cho RoomId: {RoomId}", roomId);

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                _logger.LogWarning("Phòng với RoomId: {RoomId} không tìm thấy.", roomId);
                return NotFound(); // Không tìm thấy phòng
            }

            var bookingModel = new Booking
            {
                RoomId = roomId,
                HotelId = room.HotelId // Có thể cần thêm HotelId nếu có

            };

            return View(bookingModel); // Chuyển thông tin phòng cho view
        }

        // POST: Booking/Create
        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Bảo vệ action này, chỉ cho phép người dùng đã xác thực
        public async Task<IActionResult> Create([Bind("Name,Phone,StartDate,EndDate,NumPeople,Message,RoomId,HotelId")] Booking booking)
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

                TempData["SuccessMessage"] = "Đặt phòng thành công!"; // Thông báo thành công
                return RedirectToAction("Confirmation", new { bookingId = booking.BookingId });

            }



            return View(booking);
        }



        // GET: Booking/Confirmation/5
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Confirmation(int bookingId)
        {
            _logger.LogInformation("Lấy xác nhận đặt phòng cho BookingId: {BookingId}", bookingId);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                _logger.LogWarning("Đặt phòng với BookingId: {BookingId} không tìm thấy.", bookingId);
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/MyBookingRooms
        [HttpGet]
        [Authorize] // Bảo vệ action này, chỉ cho phép người dùng đã xác thực
        public async Task<IActionResult> MyBookingRooms()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Lấy các đặt phòng cho UserId: {UserId}", userId);

            var bookings = await _context.Bookings
                .Where(b => b.UserId.ToString() == userId)
                .Include(b => b.Room)
                .ToListAsync();

            return View(bookings); // Đảm bảo trả về view mới
        }
    }
}
