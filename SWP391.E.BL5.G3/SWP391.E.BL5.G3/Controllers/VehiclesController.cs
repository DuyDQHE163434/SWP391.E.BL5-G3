using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly traveltestContext _context;

        public VehiclesController(traveltestContext context)
        {
            _context = context;
        }

        public IActionResult ListVehicles()
        {
            var vehicles = _context.Vehicles.Include(item => item.Province);
            return View(vehicles.ToList());
        }
    }
}
