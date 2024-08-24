using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Authorization;
using SWP391.E.BL5.G3.Models;
using System.Globalization;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly traveltestContext _traveltestContext;

        public FeedbackController(traveltestContext traveltestContext)
        {
            _traveltestContext = traveltestContext; 
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserFeedback(int Id)
        {

         var booking = _traveltestContext.Bookings
        .Include(b => b.Hotel)
        .Include(b => b.Restaurant)
        .Include(b => b.Tour)
        .Include(b => b.Vehicle)
        .FirstOrDefault(b => b.BookingId == Id);

            if (booking == null)
            {
                return NotFound();
            }

            var viewModel = new UserFeedbackViewModel
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                Name = booking.Name,
                Phone = booking.Phone,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                NumPeople = booking.NumPeople,
                Message = booking.Message,
                TourId = booking.TourId,
                HotelId = booking.HotelId,
                RestaurantId = booking.RestaurantId,
                VehicleId = booking.VehicleId,
                ProvinceId = booking.ProvinceId,
                Status = booking.Status,
                Hotel = booking.Hotel,
                Province = booking.Province,
                Restaurant = booking.Restaurant,
                Tour = booking.Tour,
                User = booking.User,
                Vehicle = booking.Vehicle,
                Room = booking.Room,
                RoomId = booking.RoomId,
                Payments = booking.Payments
            };

            viewModel.SetFeedbackVisibility();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTourFeedback(string HotelRating, string HotelFeedbackContent,
                                        string VehicleRating, string VehicleFeedbackContent,
                                        string GuideRating, string GuideFeedbackContent,
                                        string RestaurantRating, string RestaurantFeedbackContent,
                                        string TourRating, string TourFeedbackContent, int? TourId)
        {
            var u = (User)HttpContext.Items["User"];
            var feedbackEntries = new List<Feedback>();

            // Nếu TourId khác null, lưu tất cả feedback
            if (TourId.HasValue)
            {
                // Lưu feedback cho Tour
                feedbackEntries.Add(new Feedback
                {
                    UserId = u.UserId,
                    EntityId = 1, // Giả sử EntityId cho Tour feedback
                    Content = TourFeedbackContent,
                    Rating =  Convert.ToDouble(TourRating),
                    CreatedDate = DateTime.Now,
                });

                // Lưu feedback cho Hotel
                if (!string.IsNullOrEmpty(HotelRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 2, // Giả sử EntityId cho Hotel feedback
                        Content = HotelFeedbackContent,
                        Rating = Convert.ToDouble(HotelRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Vehicle
                if (!string.IsNullOrEmpty(VehicleRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 3, // Giả sử EntityId cho Vehicle feedback
                        Content = RestaurantFeedbackContent,
                        Rating = Convert.ToDouble(VehicleRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Guide
                if (!string.IsNullOrEmpty(GuideRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 4, // Giả sử EntityId cho Guide feedback
                        Content = GuideFeedbackContent,
                        Rating = Convert.ToDouble(GuideRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Restaurant
                if (!string.IsNullOrEmpty(RestaurantRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 5, // Giả sử EntityId cho Restaurant feedback
                        Content = VehicleFeedbackContent,
                        Rating = Convert.ToDouble(RestaurantRating),
                        CreatedDate = DateTime.Now,
                    });
                }
            }
            else // Nếu TourId là null, chỉ lưu feedback không null
            {
                // Lưu feedback cho Hotel
                if (!string.IsNullOrEmpty(HotelRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 2, // Hotel feedback
                        Content = HotelFeedbackContent,
                        Rating = Convert.ToDouble(HotelRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Vehicle
                if (!string.IsNullOrEmpty(RestaurantRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 3, // Vehicle feedback
                        Content = RestaurantFeedbackContent,
                        Rating = Convert.ToDouble(RestaurantRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Guide
                if (!string.IsNullOrEmpty(GuideRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 4, // Guide feedback
                        Content = GuideFeedbackContent,
                        Rating = Convert.ToDouble(GuideRating),
                        CreatedDate = DateTime.Now,
                    });
                }

                // Lưu feedback cho Restaurant
                if (!string.IsNullOrEmpty(VehicleRating))
                {
                    feedbackEntries.Add(new Feedback
                    {
                        UserId = u.UserId,
                        EntityId = 5, // Restaurant feedback
                        Content = VehicleFeedbackContent,
                        Rating = Convert.ToDouble(VehicleRating),
                        CreatedDate = DateTime.Now,
                    });
                }
            }

            // Lưu tất cả feedback entries vào cơ sở dữ liệu
            _traveltestContext.Feedbacks.AddRange(feedbackEntries);
            await _traveltestContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public class UserFeedbackViewModel
        {
            public int BookingId { get; set; }
            public int? UserId { get; set; }
            public string? Name { get; set; }
            public string? Phone { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int? NumPeople { get; set; }
            public string? Message { get; set; }

            public int? TourId { get; set; }
            public int? HotelId { get; set; }
            public int? RestaurantId { get; set; }
            public int? VehicleId { get; set; }
            public int? ProvinceId { get; set; }
            public int? Status { get; set; }

            public virtual Hotel? Hotel { get; set; }
            public virtual Province? Province { get; set; }
            public virtual Restaurant? Restaurant { get; set; }
            public virtual Tour? Tour { get; set; }
            public virtual User? User { get; set; }
            public virtual Vehicle? Vehicle { get; set; }
            public virtual Room? Room { get; set; }
            public int? RoomId { get; set; }

            public virtual ICollection<Payment> Payments { get; set; }

            // New properties to determine visibility in the view
            public bool ShowTourFeedback { get; set; }
            public bool ShowHotelFeedback { get; set; }
            public bool ShowRestaurantFeedback { get; set; }
            public bool ShowVehicleFeedback { get; set; }

            public UserFeedbackViewModel()
            {
                Payments = new HashSet<Payment>();
            }

            // Method to set visibility based on Booking
            public void SetFeedbackVisibility()
            {
                ShowTourFeedback = TourId.HasValue;
                ShowHotelFeedback = HotelId.HasValue;
                ShowRestaurantFeedback = RestaurantId.HasValue;
                ShowVehicleFeedback = VehicleId.HasValue;
            }
        }
    }
}
