using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SWP391.E.BL5.G3.Models;
using System.Diagnostics;

namespace SWP391.E.BL5.G3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kiểm tra vai trò người dùng
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                // Truyền vai trò tới view thông qua ViewData
                ViewData["Role"] = role;

                return View(); // Hiển thị view mặc định với thông tin vai trò
            }

            // Nếu không phải là người dùng đã xác thực
            return View("GuestHome"); // Hiện thị GuestHome cho khách
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
