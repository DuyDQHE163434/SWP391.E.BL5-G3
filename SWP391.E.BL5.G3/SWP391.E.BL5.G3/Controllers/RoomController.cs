using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering; // Thêm namespace này

namespace SWP391.E.BL5.G3.Controllers
{
    public class RoomController : Controller
    {
        private readonly traveltestContext _context;

        public RoomController(traveltestContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> DetailsRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }



        // GET: /rooms/create-room
        [HttpGet]
        public IActionResult CreateRoom(int? hotelId)
        {
            if (hotelId == null)
            {
                return BadRequest("HotelId is required.");
            }

            // Tạo danh sách trạng thái phòng
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "Available", Value = "true" },
                new SelectListItem { Text = "Booked", Value = "false" }
            };

            ViewBag.Statuses = statuses;
            ViewBag.HotelId = hotelId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom([Bind("Price,Status,Description,HotelId")] Room room, IFormFile Image)
        {

            if (ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }
            else
            {
                try
                {
                    if (Image != null && Image.Length > 0)
                    {
                        // Generate a unique file name to prevent overwriting
                        var fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        var extension = Path.GetExtension(Image.FileName);
                        var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                        // Save the file to the wwwroot/images directory
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        // Store the relative path in the database
                        room.Image = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        // Assign a default image if no image is uploaded
                        room.Image = "/images/default.png"; // Placeholder image
                    }
                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Room created successfully.";
                    return RedirectToAction("Details", "Hotels", new { id = room.HotelId });
                }
                catch (DbUpdateException ex)
                {
                    // Ghi log lỗi
                    System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the room.");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the room.");
                }
            }

            return View(room);
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }

        [HttpGet]   
        public IActionResult EditRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _context.Rooms.FirstOrDefault(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "Available", Value = "true" },
                new SelectListItem { Text = "Booked", Value = "false" }
            };

            ViewBag.Statuses = statuses;

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoom([Bind("RoomId,Price,Status,Description,HotelId")] Room room, IFormFile? Image, string? CurrentImage)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    if (Image != null && Image.Length > 0)
                    {
                        // Generate a unique file name to prevent overwriting
                        var fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        var extension = Path.GetExtension(Image.FileName);
                        var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                        // Save the file to the wwwroot/images directory
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        // Store the relative path in the database
                        room.Image = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        // Assign a default image if no image is uploaded
                        room.Image = CurrentImage; // Placeholder image
                    }
                    _context.Update(room);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Room updated successfully!";
                    return RedirectToAction("Details", "Hotels", new { id = room.HotelId });
                }
                catch (DbUpdateConcurrencyException)
                {
                        TempData["ErrorMessage"] = "Failed to update hotel due to concurrency issue. Please try again.";
                        throw;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to update hotel: {ex.Message}";
                }
            }
            return View(room);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            var hotelId = room.HotelId;
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Room and related records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error deleting the room: {ex.Message}";
            }

            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }
    }
}
