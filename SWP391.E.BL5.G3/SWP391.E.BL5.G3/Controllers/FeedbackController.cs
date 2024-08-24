using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly traveltestContext _traveltestContext;

        public FeedbackController(traveltestContext traveltestContext)
        {
            _traveltestContext = traveltestContext; 
        }

        public IActionResult UserFeedback()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTourFeedback(string HotelRating, string HotelFeedbackContent,
                                        string VehicleRating, string VehicleFeedbackContent,
                                        string GuideRating, string GuideFeedbackContent,
                                        string RestaurantRating, string RestaurantFeedbackContent,
                                        string TourRating, string TourFeedbackContent)
        {
            var u = (User)HttpContext.Items["User"];
            // Lấy giá trị từ form
            var feedback = new Feedback
            {
                UserId = u.UserId,
                EntityId = 1,
                Content = TourFeedbackContent, // Lưu nội dung từ form
                Rating = double.Parse(TourRating), // Lưu overall rating
                CreatedDate = DateTime.Now,
            };

            // Lưu vào cơ sở dữ liệu
            _traveltestContext.Feedbacks.Add(feedback);
            await _traveltestContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
