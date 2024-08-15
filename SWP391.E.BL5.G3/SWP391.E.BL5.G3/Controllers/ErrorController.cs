using Microsoft.AspNetCore.Mvc;

namespace SWP391.E.BL5.G3.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error401()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }
    }
}
