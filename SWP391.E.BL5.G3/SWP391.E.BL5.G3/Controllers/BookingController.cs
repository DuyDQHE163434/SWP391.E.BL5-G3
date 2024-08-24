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
            _logger.LogInformation("Quá trình tạo đặt phòng bắt đầu cho RoomId: {RoomId}, UserId: {UserId}", booking.RoomId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin phòng từ cơ sở dữ liệu
                    var room = await _context.Rooms.FindAsync(booking.RoomId);
                    if (room == null)
                    {
                        _logger.LogWarning("Phòng với RoomId: {RoomId} không tìm thấy.", booking.RoomId);
                        TempData["ErrorMessage"] = "Phòng không có sẵn.";
                        return RedirectToAction("Create", new { roomId = booking.RoomId });
                    }

                    if (room.Status == true) // Kiểm tra nếu phòng đã được đặt
                    {
                        _logger.LogWarning("Phòng với RoomId: {RoomId} đã được đặt.", booking.RoomId);
                        TempData["ErrorMessage"] = "Phòng không có sẵn.";
                        return RedirectToAction("Create", new { roomId = booking.RoomId });
                    }

                    // Lấy UserId từ Claims
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        _logger.LogWarning("UserId không tìm thấy khi tạo đặt phòng cho RoomId: {RoomId}", booking.RoomId);
                        TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt phòng.";
                        return RedirectToAction("Create", new { roomId = booking.RoomId });
                    }

                    booking.UserId = int.Parse(userId);

                    // Tạo đối tượng Booking mới và lưu vào cơ sở dữ liệu
                    var newBooking = new Booking
                    {
                        Name = booking?.Name,
                        Phone = booking?.Phone,
                        StartDate = booking?.StartDate,
                        EndDate = booking?.EndDate,
                        UserId = booking?.UserId,
                        NumPeople = booking?.NumPeople,
                        Message = booking?.Message,
                        HotelId = booking?.HotelId,
                        RoomId = (int)(booking?.RoomId),
                        Status = (int)BookingStatusEnum.Pending // Đặt trạng thái là Pending
                    };

                    _context.Bookings.Add(newBooking);

                    // Cập nhật trạng thái phòng và UserId trong Room
                    room.Status = true;
                    _context.Rooms.Update(room);

                    // Lưu tất cả các thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Đặt phòng được tạo thành công với BookingId: {BookingId}", newBooking.BookingId);

                    return RedirectToAction("Confirmation", new { bookingId = newBooking.BookingId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Đã xảy ra lỗi trong khi tạo đặt phòng cho RoomId: {RoomId}, UserId: {UserId}", booking.RoomId, booking.UserId);
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn.");
                }
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
