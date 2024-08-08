using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SWP391.E.BL5.G3.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SWP391.E.BL5.G3.Controllers
{
    public class ToursController : Controller
    {
        private readonly traveltestContext _context;

        public ToursController(traveltestContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListTour(string searchString)
        {
            var tours = from t in _context.Tours
                        select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                tours = tours.Where(s => s.Name.Contains(searchString));
            }

            return View(await tours.ToListAsync());
        }

        public async Task<IActionResult> TourDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .FirstOrDefaultAsync(m => m.TourId == id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        public IActionResult CreateTour()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour([Bind("Name,Description,Price,Rating,Duration,AirPlane,Status,Itinerary,Inclusions,Exclusions,GroupSize,Guide")] Tour tour, IFormFile imageFile) 
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var filePath = Path.Combine("wwwroot/images", imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    tour.Image = imageFile.FileName;
                }

                _context.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListTour));
            }
            return View(tour);
        }
    }
}
