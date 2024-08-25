using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.DTOs;
using SWP391.E.BL5.G3.Enum;
using SWP391.E.BL5.G3.Models;
using SWP391.E.BL5.G3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class HotelsController : Controller
    {
        private readonly traveltestContext _context;

        public HotelsController(traveltestContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string filter = "None", string searchString = "", int? page = 1)
        {
            var hotels = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                hotels = hotels.Where(h => h.HotelName.Contains(searchString));
            }

            switch (filter)
            {
                case "HighestPrice":
                    hotels = hotels.OrderByDescending(h => h.Price.GetValueOrDefault());
                    break;
                case "LowestPrice":
                    hotels = hotels.OrderBy(h => h.Price.GetValueOrDefault());
                    break;
                case "MostBooked":
                    hotels = hotels.OrderByDescending(h => h.BookingCount.GetValueOrDefault());
                    break;
                default:
                    hotels = hotels.OrderBy(h => h.HotelId);
                    break;
            }

            ViewData["CurrentFilter"] = filter;
            ViewData["CurrentSearch"] = searchString;

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            try
            {
                var totalCount = await hotels.CountAsync();
                var pagedHotels = await hotels.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return View(pagedHotels);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while loading the hotels: {ex.Message}";
                return View(new List<Hotel>());
            }
        }



        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        public IActionResult Create()
        {
            var provinces = _context.Provinces.ToList();
            ViewBag.ProvincesList = new SelectList(provinces, "ProvinceId", "ProvinceName");
            return View();
        }


        [HttpPost]
        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelName,Location,Description,Status,Price,ProvinceId")] Hotel hotel, IFormFile Image)
        {
            if (!ModelState.IsValid)
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
            else { 
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
                        hotel.Image = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        // Assign a default image if no image is uploaded
                        hotel.Image = "/images/default.png"; // Placeholder image
                    }

                    hotel.BookingCount = 0; // Set the initial booking count to 0
                    hotel.CreatedAt = DateTime.Now; 
                    _context.Add(hotel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Hotel added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to add hotel: {ex.Message}";
                }
            }

            ViewBag.ProvincesList = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName");
            return View(hotel);
        }


        [HttpGet]
        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        public IActionResult Edit(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(h => h.HotelId == id);

            if (hotel == null)
            {
                return NotFound();
            }

            var provincesList = _context.Provinces.ToList();

            var viewModel = new HotelViewModel
            {
                Hotel = hotel,
                ProvincesList = provincesList
            };

            return View(viewModel);
        }


        [HttpPost]
        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("HotelId,HotelName,Location,Description,Image,Status,Price,BookingCount,ProvinceId")] Hotel hotel, IFormFile? Image, string? CurrentImage)
        {
            if (ModelState.IsValid)
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
                        hotel.Image = $"/images/{uniqueFileName}";
                    }
                    else
                    {
                        // Assign a default image if no image is uploaded
                        hotel.Image = CurrentImage; // Placeholder image
                    }
                    hotel.UpdatedAt = DateTime.Now;
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Hotel updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.HotelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update hotel due to concurrency issue. Please try again.";
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to update hotel: {ex.Message}";
                }
            }

            ViewBag.ProvincesList = new SelectList(_context.Provinces.ToList(), "ProvinceId", "ProvinceName", hotel.ProvinceId);
            return View(hotel);
        }



        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        //[Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                TempData["ErrorMessage"] = "Hotel not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var bookings = _context.Bookings.Where(b => b.HotelId == id);
                if (bookings.Any())
                {
                    _context.Bookings.RemoveRange(bookings);
                }

                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hotel and related records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error deleting the hotel: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.Include(h => h.Rooms)
                .SingleOrDefaultAsync(h => h.HotelId == id);

            if (hotel == null)
            {
                return NotFound();
            }

            // Xử lý thuộc tính Status
            bool? statusBool = null;

            // Kiểm tra nếu Status là bool
            if (hotel.Status is bool boolStatus)
            {
                statusBool = boolStatus;
            }

            // Lưu trữ vào ViewBag để sử dụng trong view
            ViewBag.StatusBool = statusBool;

            return View(hotel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ListHotelForGuestCustomer(string searchString = "", int? page = 1)
        {
            var hotels = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                hotels = hotels.Where(h => h.HotelName.Contains(searchString));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            try
            {
                var totalCount = await hotels.CountAsync();
                var pagedHotels = await hotels.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return View(pagedHotels);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while loading the hotels: {ex.Message}";
                return View(new List<Hotel>());
            }
        }


        public IActionResult Error()
        {
            ViewBag.Message = TempData["ErrorMessage"];
            return View();
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }



    }

}

