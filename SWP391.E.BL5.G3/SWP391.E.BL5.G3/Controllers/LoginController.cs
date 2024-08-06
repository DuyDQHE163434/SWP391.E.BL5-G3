using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class LoginController : Controller
    {
        traveltestContext context = new traveltestContext();
        public IActionResult Login(int mess)
        {
            if (mess == 1)
            {
                ViewBag.mess1 = "Thông tin tài khoản không tồn tại , kiểm tra lại thông tin đăng nhập";
            }
            else if (mess == 2)
            {
                ViewBag.mess1 = "Vui lòng đăng nhập trước khi thao tác";
            }
            else if (mess == 3)
            {
                ViewBag.mess1 = "This email has been registered !!!";
            }
            else if (mess == 4)
            {
                ViewBag.mess1 = "Account registration successful !!!";
            }
            else
            {
                ViewBag.mess1 = "";
            }
            return View();
        }
    }
}
