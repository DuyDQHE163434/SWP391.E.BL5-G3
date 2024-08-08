using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.DAO_Context;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class LoginController : Controller
    {
        traveltestContext context = new traveltestContext();
        DAO dal = new DAO();
        public IActionResult LoginAccess()
        {
            String Username = HttpContext.Request.Form["username"];
            String Pass = Pass = HttpContext.Request.Form["pass"];
            User account = new User();
            account = dal.Login(Username, Pass);

            if (account != null)
            {
                HttpContext.Session.SetString("Email", account.Email.ToString());
                HttpContext.Session.SetString("FirstName", account.FirstName.ToString());
                HttpContext.Session.SetString("LastName", account.LastName.ToString());
                HttpContext.Session.SetString("RoleID", account.RoleId.ToString());
                HttpContext.Session.SetString("Phone", account.PhoneNumber.ToString());
                HttpContext.Session.SetString("username", account.Email.ToString());
                HttpContext.Session.SetString("pass", account.Password.ToString());
                //HttpContext.Session.SetString("Image", account.Image.ToString());

                if (account.Description != null)
                {
                    HttpContext.Session.SetString("descr", account.Description.ToString());
                }
                else
                {
                    HttpContext.Session.SetString("descr", "");
                }
                return RedirectToAction("index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Login", new { mess = 1 });
            }
        }
        
        // controller IActionResult của HTML Login
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
        public IActionResult Register(int mess, string email)
        {
            ViewBag.Email = email;
            if (mess == 1)
            {
                ViewBag.mess2 = "Please your imfomation again!!!";
            }
            else if (mess == 2)
            {
                ViewBag.mess2 = "This email has been registered !!!";
            }
            else
            {
                ViewBag.mess2 = "";
            }
            if (ViewBag.Email == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        public IActionResult CheckEmailRegister(int mess)
        {
            if (mess == 1)
            {
                ViewBag.mess1 = "Tài Khoản Email không đúng hoặc không tồn tại vui lòng kiểm tra lại !!!";
            }

            return View();
        }
    }
}
