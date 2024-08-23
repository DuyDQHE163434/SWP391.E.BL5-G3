using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SWP391.E.BL5.G3.Models;
using System.Diagnostics;
using SWP391.E.BL5.G3.Authorization;
using CloudinaryDotNet.Actions;
using SWP391.E.BL5.G3.Enum;

namespace SWP391.E.BL5.G3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JwtUtils _jwtUtils;

        public HomeController(ILogger<HomeController> logger, JwtUtils jwtUtils)
        {
            _logger = logger;
            _jwtUtils = jwtUtils;
        }

        public IActionResult Index()
        {
            // Kiểm tra vai trò người dùng
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var roleName =  ((RoleEnum)int.Parse(role)).ToString();

                // Truyền vai trò tới view thông qua ViewData
                ViewData["Role"] = roleName;

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
