using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391.E.BL5.G3.Models;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        ViewBag.UserImage = user.Image != null ? $"~/images/avatar/{user.Image}" : "~/images/avatar/default-user.png";

        return View(user);
    }

    [HttpPost]
    public IActionResult UpdateProfile(User model)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);

        if (user != null)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.Gender = model.Gender;

            try
            {

                _context.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating profile: {ex.Message}";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "User not found!";
        }

        return RedirectToAction("Profile");
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

        if (user.Password != CurrentPassword)
        {
            TempData["ErrorMessage"] = "Current password is incorrect.";
            return RedirectToAction("Profile");
        }

        if (NewPassword != RenewPassword)
        {
            TempData["ErrorMessage"] = "New password and confirmation do not match.";
            return RedirectToAction("Profile");
        }

        user.Password = NewPassword;

        _context.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password changed successfully!";
        return RedirectToAction("Profile");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateProfileImage(IFormFile ProfileImage)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Login");
        }

        var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Profile");
        }

        if (ProfileImage == null || ProfileImage.Length == 0)
        {
            TempData["ErrorMessage"] = "No image uploaded.";
            return RedirectToAction("Profile");
        }

        try
        {
            // Xác định thư mục lưu trữ ảnh
            var folder = "wwwroot/images/avatar";
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfileImage.FileName);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), folder);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(user.Image))
            {
                var oldFilePath = Path.Combine(uploadsFolder, user.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Lưu ảnh mới
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            // Cập nhật thông tin người dùng
            user.Image = uniqueFileName;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile image updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating profile image: {ex.Message}";
            if (ex.InnerException != null)
            {
                TempData["ErrorMessage"] += $" Inner Exception: {ex.InnerException.Message}";
            }
        }

        return RedirectToAction("Profile");
    }
}