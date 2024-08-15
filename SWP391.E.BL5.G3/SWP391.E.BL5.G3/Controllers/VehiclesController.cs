using Microsoft.AspNetCore.Mvc;

namespace SWP391.E.BL5.G3.Controllers
{
    public class VehiclesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
