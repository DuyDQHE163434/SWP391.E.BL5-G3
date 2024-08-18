using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SWP391.E.BL5.G3.Controllers
{
    public class UserController : Controller
    {
        private readonly traveltestContext _context;

        public UserController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = user.UserId;
            ViewBag.UserImage = user.Image ?? "default-user.png";

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                // Kiểm tra và lưu hình ảnh
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    user.Image = fileName;
                }

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            return View("Profile", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string RenewPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != CurrentPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Profile");
            }

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới có khớp nhau không
            if (NewPassword != RenewPassword)
            {
                TempData["ErrorMessage"] = "Xác nhận mật khẩu không khớp.";
                return RedirectToAction("Profile");
            }

            // Cập nhật mật khẩu mới
            user.Password = NewPassword;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
            return RedirectToAction("Profile");
        }
    }
}
