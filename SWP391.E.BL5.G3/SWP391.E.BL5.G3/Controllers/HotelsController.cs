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

        

        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        public IActionResult Create()
        {
            var provinces = _context.Provinces.ToList();
            ViewBag.ProvincesList = new SelectList(provinces, "ProvinceId", "ProvinceName");
            return View();
        }


        [HttpPost]
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,HotelName,Location,Description,Image,Status,Price,BookingCount,ProvinceId")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (hotel.ImageFile != null && hotel.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(hotel.ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await hotel.ImageFile.CopyToAsync(stream);
                        }

                        hotel.Image = $"/images/{fileName}";
                    }
                    else
                    {
                        hotel.Image = "/images/default.png"; // Placeholder image
                    }

                    hotel.BookingCount = 0;
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
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,HotelName,Location,Description,Image,Status,Price,BookingCount,ProvinceId")] Hotel hotel)
        {
            if (id != hotel.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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



        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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
        [Authorize(RoleEnum.Admin, RoleEnum.Travel_Agent)]
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Rooms) // Bao gồm thông tin phòng
                .FirstOrDefaultAsync(m => m.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }


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

