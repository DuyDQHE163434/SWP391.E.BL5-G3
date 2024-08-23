using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering; // Thêm namespace này

namespace SWP391.E.BL5.G3.Controllers
{
    [Route("rooms")]
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
        [HttpGet("create-room")]
        public IActionResult CreateRoom(int? hotelId)
        {
            if (hotelId == null)
            {
                return BadRequest("HotelId is required.");
            }

            // Tạo danh sách trạng thái phòng
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Text = "Active", Value = "true" },
                new SelectListItem { Text = "Inactive", Value = "false" }
            };

            ViewBag.Statuses = statuses;
            HttpContext.Session.SetInt32("HotelId", hotelId.Value);
            return View();
        }

        [HttpPost("create-room")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom([Bind("Price,Image,Status,Description")] Room room)
        {
            var hotelId = HttpContext.Session.GetInt32("HotelId");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (hotelId == null || string.IsNullOrEmpty(userId))
            {
                return BadRequest("HotelId and UserId are required.");
            }

            room.HotelId = hotelId.Value;
            room.UserId = int.Parse(userId);

            // Ghi log giá trị để kiểm tra
            System.Diagnostics.Debug.WriteLine($"HotelId: {room.HotelId}, UserId: {room.UserId},RoomId:{room.RoomId}, Price: {room.Price}, Image: {room.Image}, Status: {room.Status}, Description: {room.Description}");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Room created successfully.";
                    return RedirectToAction("DetailsForGuestCustomer", "Hotels", new { id = room.HotelId });
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

            // Tạo danh sách trạng thái
            var statuses = new List<SelectListItem>
    {
        new SelectListItem { Text = "Active", Value = "true" },
        new SelectListItem { Text = "Inactive", Value = "false" }
    };
            ViewBag.Statuses = statuses;

            return View(room);
        }



        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}
