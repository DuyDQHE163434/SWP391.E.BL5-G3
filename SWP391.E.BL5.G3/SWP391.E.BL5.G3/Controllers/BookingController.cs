using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class BookingController : Controller
    {
        private readonly traveltestContext _context;

        public BookingController(traveltestContext context)
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
    }
}
