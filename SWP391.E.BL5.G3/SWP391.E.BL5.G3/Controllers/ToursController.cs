using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DAO_Context;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Extensions;
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

            int userrole = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Role));

            var toursQuery = _context.Tours.Include(t => t.Province).AsQueryable();

            // Kiểm tra vai trò người dùng
            if (userrole == 2)
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
            if (!ModelState.IsValid)
            {
                tour.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (image != null && image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu không tồn tại

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    tour.Image = uniqueFileName; // Gán tên hình ảnh cho tour
                }

                tour.CreateDate = DateTime.Now;

                // Thêm tour vào cơ sở dữ liệu
                _context.Add(tour);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListTour));
            }

            // Nếu model không hợp lệ, sẽ lấy lại danh sách tỉnh
            var provinces = await _context.Provinces.ToListAsync();
            ViewBag.ProvinceList = new SelectList(provinces, "ProvinceId", "ProvinceName");

            //Console.WriteLine("=====================");
            //Console.WriteLine(tour.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

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

            if (!ModelState.IsValid)
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

            int pageSize = 6; // Số lượng tour trên mỗi trang
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
                TourId = tourId,
                NumPeople = 1
            };

            ViewBag.MaxPeople = tour.GroupSize;
            ViewBag.Duration = tour.Duration; // Truyền Duration xuống View
            ViewBag.Price = tour.Price; // Truyền giá tour xuống View
            ViewBag.TotalPrice = tour.Price; // Tổng tiền cho 1 người

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
        [Authorize] // Bảo vệ action này
        public async Task<IActionResult> MyBookingTours()
        {
            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truy vấn các booking có trạng thái Pending hoặc Confirmed của người dùng
            var bookings = await _context.Bookings
                .Where(b => b.UserId.ToString() == userId && (b.Status == (int)BookingStatusEnum.Pending || b.Status == (int)BookingStatusEnum.Confirmed))
                .Include(b => b.Tour) // Bao gồm thông tin tour
                .Include(b => b.Restaurant)
                .Include(b => b.Vehicle)
                .ToListAsync();

            return View("MyBookingTours", bookings); // Trả về view với danh sách bookings
        }


        [HttpGet]
        [Authorize] // Bảo vệ action này
        public async Task<IActionResult> HistoryBookingTours()
        {
            // Lấy ID của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truy vấn các booking có trạng thái Done hoặc Canceled của người dùng
            var historyBookings = await _context.Bookings
                .Where(b => b.UserId.ToString() == userId && (b.Status == (int)BookingStatusEnum.Done || b.Status == (int)BookingStatusEnum.Canceled))
                .Include(b => b.Tour) // Bao gồm thông tin tour
                .Include (b => b.Restaurant)
                .Include(b => b.Vehicle)
                .ToListAsync();

            return View("HistoryBookingTours", historyBookings); // Trả về view với danh sách bookings lịch sử
        }

        [HttpPost]
        [Authorize] // Bảo vệ action này
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTour(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(); // Nếu không tìm thấy booking theo ID
            }

            // Cập nhật trạng thái của booking thành Canceled
            booking.Status = (int)BookingStatusEnum.Canceled;

            // Lưu vào cơ sở dữ liệu
            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookingTours)); // Quay lại danh sách tour của người dùng
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewDetailBookingTour(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(); // Nếu không tìm thấy booking
            }

            // Chuyển thông tin booking đến view để xem chi tiết
            return View(booking);
        }


        [HttpGet]
        [Authorize] // Bảo vệ action này
        public async Task<IActionResult> EditBookingTour(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(); // Nếu không tìm thấy booking
            }

            // Chuyển thông tin booking đến view để chỉnh sửa
            return View(booking);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBookingTour([Bind("BookingId,Name,Phone,StartDate,EndDate,NumPeople,Message,TourId,UserId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin booking trong cơ sở dữ liệu
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBookingTours)); // Quay về danh sách tours đã đặt
            }
            return View(booking); // Nếu có lỗi, trở về view để người dùng chỉnh sửa lại
        }

        [HttpPost]
        public async Task<IActionResult> Pay([Bind("Name,Phone,StartDate,EndDate,NumPeople,Message,TourId,UserId")] Booking booking)
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
                    Status = (int)BookingStatusEnum.Pending
                };
                // Thêm booking vào cơ sở dữ liệu
                _context.Bookings.Add(bookingg);
                await _context.SaveChangesAsync();

                // Lấy thông tin tour từ cơ sở dữ liệu
                var tour = await _context.Tours.FindAsync(booking.TourId);
                if (tour == null)
                {
                    return NotFound(); // Nếu tour không tồn tại
                }

                string vnp_Returnurl = "https://localhost:7295/VnPay/ReturnUrl";
                string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL thanh toán của VNPAY
                string vnp_TmnCode = "CA6G65VO"; // Mã định danh merchant kết nối (Terminal Id)
                string vnp_HashSecret = "NTJ0PDLNZOM2W7ZPSCFKWI85HPCDDD7R"; // Secret key cần được cung cấp

                long totalAmount = (long)(tour.Price * booking.NumPeople * 0.5); // Số tiền phải thanh toán


                var order = new OrderInfo
                {
                    OrderId = DateTime.Now.Ticks, // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
                    Amount = totalAmount, // Giả lập số tiền thanh toán
                    Status = "0", // Trạng thái thanh toán
                    CreatedDate = DateTime.Now
                };

                // Save order to db (Giả lập lưu đơn hàng vào DB)

                // Build URL for VNPAY
                var vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); // Số tiền thanh toán
                vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", bookingg.BookingId.ToString());
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

                // Add Params of 2.1.0 Version
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                var payment = new Payment
                {
                    BookingId = bookingg.BookingId, // Khóa ngoại liên kết với bản ghi booking
                    Amount = totalAmount, // Số tiền thanh toán
                    PaymentDate = DateTime.Now, // Ngày thanh toán
                    TransactionId = order.OrderId.ToString(), // Mã giao dịch tạm thời
                    Status = (int)BookingStatusEnum.Pending, // Trạng thái thanh toán ban đầu
                    ResponseCode = null, // Chưa có thông tin phản hồi
                    Message = null // Hoặc có thể là thông điệp mặc định
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

                return Redirect(paymentUrl);
            }
            return View(booking);
        }

        [HttpGet]
        [Authorize(RoleEnum.Travel_Agent)]
        //[AllowAnonymous]
        public async Task<IActionResult> BookingTourInTravelAgent()
        {
            List<Booking> booking = _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Restaurant)
                .Include(b => b.Vehicle)
                .Include(b => b.User).Where(x => x.Status == 1 || x.Status == 2 || x.Status == 3 || x.Status == 4 ).ToList();
            ViewBag.Booking = booking;

            return View();
        }

        public IActionResult RequestAccept(int id, string email)
        {
            DAO dal = new DAO();
            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = email;
            string subject = "Hello " + email;

            string body = "Chuyen di cua ban da duoc dat thanh cong vui long kiem tra lich trinh chuyen di";
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";
            bool result = SendEmail.theSendEmailRegisterTravelAgent(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword);
            string stt = "Accept";
            dal.AccessBookingTravel(id, stt);
            return RedirectToAction("BookingTourInTravelAgent", "Tours");
        }

        public IActionResult RequestUnaccept(int id, string email)
        {
            DAO dal = new DAO();
            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = email;
            string subject = "Hello " + email;

            string body = "Vì Một Số Lý Do Nào Đó Từ Phía Của Chúng Tôi Không Thể Nhan Tour Cua Ban ";
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";
            bool result = SendEmail.theSendEmailRegisterTravelAgent(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword);
            string stt = "Unaccept";
            dal.AccessBookingTravel(id, stt);
            return RedirectToAction("BookingTourInTravelAgent", "Tours");
        }
    }
}











