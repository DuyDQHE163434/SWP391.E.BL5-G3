using Microsoft.AspNetCore.Mvc;

namespace SWP391.E.BL5.G3.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult UserFeedback()
        {
            return View();
        }
    }
}
