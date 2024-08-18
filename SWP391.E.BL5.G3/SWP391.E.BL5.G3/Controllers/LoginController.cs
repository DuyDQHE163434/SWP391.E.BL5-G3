using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DAO_Context;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class LoginController : Controller
    {
        private readonly JwtUtils jwtUtils;
        traveltestContext context = new traveltestContext();
        DAO dal = new DAO();

        public LoginController(JwtUtils jwtUtils)
        {
            this.jwtUtils = jwtUtils;
        }

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

                string token = jwtUtils.GenerateJwtToken(account);
                Response.Cookies.Append("accessToken", token);

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
                ViewBag.mess1 = "Account information does not exist, check login information again";
            }
            else if (mess == 2)
            {
                ViewBag.mess1 = "Please login before proceeding.";
            }
            else if (mess == 3)
            {
                ViewBag.mess1 = "This email has been registered !!!";
            }
            else if (mess == 4)
            {
                ViewBag.mess1 = "Account registration successful !!!";
            }
            else if (mess == 5)
            {
                ViewBag.mess1 = "Change Password successful !!!";
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
                ViewBag.mess1 = "Email Account is incorrect or does not exist, please check again. !!!";
            }

            return View();
        }
        public IActionResult CheckEmailRegisterAccess()
        {
            traveltestContext context = new traveltestContext();
            DAO dal = new DAO();
            String Email = "";
            Email = HttpContext.Request.Form["email"];


            Random r = new Random();
            string OTP = r.Next(100000, 999999).ToString();


            //sendemail

            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = Email;
            string subject = "Hello " + Email;

            string body =
                "OTP code to register an account at Your Travel System Is: " + OTP;
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";

            bool result = SendEmail.theSendEmailForGotPassWord(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword, Email);

            //Check Email
            if (dal.IsEmailValid(Email) && result == true)
            {

                HttpContext.Session.SetString("Email", Email.ToString());
                HttpContext.Session.SetString("OTP", OTP.ToString());


                return RedirectToAction("ConfilmOTPregister", "Login");
            }
            else
            {
                return RedirectToAction("CheckEmailRegister", "Login", new { mess = 1 });
            }

        }

        public IActionResult ConfilmOTPregister(string messcf)
        {
            String OTP = HttpContext.Session.GetString("OTP");
            String Email = HttpContext.Session.GetString("Email");
            ViewBag.messcf = messcf;
            ViewBag.Email = Email;
            ViewBag.OTP = OTP;

            return View();
        }
        public IActionResult ConfilmOTPregisterAccess(string email, string otp)
        {

            String OTP = "";
            OTP = HttpContext.Request.Form["otpcf"];
            ViewBag.OTP = otp;
            if (OTP == otp)
            {


                return RedirectToAction("Register", "Login", new { email = email });
            }
            else
            {
                return RedirectToAction("ConfilmOTPregister", "Login", new { messcf = 1 });
            }
        }
        public IActionResult RegisterAccess()
        {
            traveltestContext context = new traveltestContext();
            DAO dal = new DAO();
            String Username = "";
            Username = HttpContext.Request.Form["username"];
            String Pass = "";
            Pass = HttpContext.Request.Form["pass"];
            String Cf_Pass = "";
            Cf_Pass = HttpContext.Request.Form["Confirm-Password"];

            String FirstName = "";
            FirstName = HttpContext.Request.Form["FirstName"];
            String LastName = "";
            LastName = HttpContext.Request.Form["LastName"];
            String PhoneNumber = "";
            PhoneNumber = HttpContext.Request.Form["PhoneNumber"];
            String Gender = "";
            Gender = HttpContext.Request.Form["Gender"];

            Random r = new Random();
            string OTP = r.Next(100000, 999999).ToString();


            //sendemail

            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = Username;
            string subject = "Hello" + Username;


            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";



            //Check Email
            if (dal.IsEmailValid(Username) == true && Pass == Cf_Pass && dal.IsPhoneNumberValidVietnam(PhoneNumber) == true && dal.IsValidFirstnameorLastname(FirstName) == true && dal.IsValidFirstnameorLastname(LastName) == true)
            {

                User usercheck = context.Users.Where(x => x.Email == Username).FirstOrDefault();
                User user = new User()
                {
                    Email = HttpContext.Request.Form["username"],
                    Password = HttpContext.Request.Form["pass"],
                    FirstName = HttpContext.Request.Form["FirstName"],
                    LastName = HttpContext.Request.Form["LastName"],
                    PhoneNumber = HttpContext.Request.Form["PhoneNumber"],
                    RoleId = 1,
                    Action = true,
                    Gender = Convert.ToBoolean(Convert.ToInt32(HttpContext.Request.Form["Gender"]))
                };
                if (usercheck != null)
                {
                    string body = "Account Creation Failed, This email has been registered, please check again. !!!";
                    bool result = SendEmail.theSendEmail(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword, Username, Pass, Cf_Pass, FirstName, LastName, PhoneNumber);
                    return RedirectToAction("Login", "Login", new { mess = 3 });
                }
                else
                {
                    string body = "You Have Successfully Registered an Account at Travel System  !!!";
                    bool result = SendEmail.theSendEmail(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword, Username, Pass, Cf_Pass, FirstName, LastName, PhoneNumber);
                    context.Add(user);
                    context.SaveChanges();
                    return RedirectToAction("Login", "Login", new { mess = 4 });
                }

            }
            else
            {
                return RedirectToAction("Register", "Login", new { mess = 1 });
            }
        }
        public IActionResult ForgotPassWord(int mess)
        {
            return View();
        }


        public IActionResult ForgotPassWordAccess()
        {
            traveltestContext context = new traveltestContext();
            DAO dal = new DAO();
            String Email = "";
            Email = HttpContext.Request.Form["email"];


            Random r = new Random();
            string OTP = r.Next(100000, 999999).ToString();


            //sendemail

            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = Email;
            string subject = "Hello " + Email;

            string body =
                "Mã OTP Change Password của Bạn Là: " + OTP;
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";

            bool result = SendEmail.theSendEmailForGotPassWord(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword, Email);

            //Check Email
            if (dal.IsEmailValid(Email) && result == true)
            {

                HttpContext.Session.SetString("Email", Email.ToString());
                HttpContext.Session.SetString("OTP", OTP.ToString());


                return RedirectToAction("ConfilmOTP", "Login", new { mess = 2 });
            }
            else
            {
                return RedirectToAction("Register", "Login", new { mess = 1 });
            }

        }

        public IActionResult ConfilmOTP(string messcf, string mess)
        {
            String OTP = HttpContext.Session.GetString("OTP");
            String Email = HttpContext.Session.GetString("Email");
            ViewBag.messcf = messcf;
            ViewBag.Email = Email;
            ViewBag.OTP = OTP;
            ViewBag.Mess = mess;
            return View();
        }
        public IActionResult ConfilmOTPAccess(string email, string otp, string mess)
        {
            String OTP = "";
            OTP = HttpContext.Request.Form["otpcf"];
            ViewBag.OTP = otp;
            if (OTP == otp)
            {


                return RedirectToAction("ChangePassWord", "Login", new { email = email });
            }
            else
            {
                return RedirectToAction("ConfilmOTP", "Login", new { messcf = 1 });
            }
        }
        public IActionResult ChangePassWord(string email)
        {
            HttpContext.Session.SetString("Email", email);
            ViewBag.Email = email;

            return View();
        }
        public IActionResult ChangePassWordAccess()
        {
            traveltestContext context = new traveltestContext();
            DAO dal = new DAO();
            String Email = "";
            Email = HttpContext.Request.Form["email"];
            String Pass = "";
            Pass = HttpContext.Request.Form["pass"];
            String Cf_Pass = "";
            Cf_Pass = HttpContext.Request.Form["Confirm-Password"];

            User users = dal.getUser(HttpContext.Session.GetString("Email"));
            if (Pass == Cf_Pass && dal.ChangePass(users, Pass))
            {



                return RedirectToAction("Login", "Login", new { mess = 5 });
            }
            else
            {
                return RedirectToAction("ChangePassWord", "Login", new { email = Email });
            }


        }
    }
}
