using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SWP391.E.BL5.G3.Authorization;

using SWP391.E.BL5.G3.DAO_Context;

using SWP391.E.BL5.G3.Models;
using System.Globalization;

namespace SWP391.E.BL5.G3.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly traveltestContext _traveltestContext;
        private readonly Cloudinary _cloudinary;

        public AdminController(traveltestContext traveltestContext, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _traveltestContext = traveltestContext;
            var cloudinarySettingsValue = cloudinarySettings.Value;
            var account = new Account(
                cloudinarySettingsValue.CloudName,
                cloudinarySettingsValue.ApiKey,
                cloudinarySettingsValue.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Dashboard()
        {
            var today = DateTime.Today;

            // Today's Sales
            var todaysSales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Date == today)
                .Sum(p => p.Amount);

            // Total Tours
            var totalTours = _traveltestContext.Tours.Count(); // Assuming you have a Bookings DbSet

            // Total Tour Guides
            var totalTourGuides = _traveltestContext.TourGuides.Count(); // Assuming you have a TourGuides DbSet

            // Monthly Sales
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var monthlySales = _traveltestContext.Payments
                .Where(p => p.PaymentDate >= startOfMonth)
                .Sum(p => p.Amount);

            // Prepare comparison values
            var salesComparison = CalculateSalesComparison(today); // Implement this method based on your logic
            var monthlyComparison = CalculateMonthlyComparison(today); // Implement this method based on your logic

            var distinctYears = _traveltestContext.Payments
                .Select(p => p.PaymentDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Fetch feedback ratings
            var feedbackData = _traveltestContext.Feedbacks
                .GroupBy(f => f.Rating)
                .Select(g => new
                {
                    Rating = g.Key,
                    Count = g.Count()
                })
                .OrderBy(f => f.Rating)
                .ToList();

            // Prepare data for feedback ratings
            var feedbackCounts = new int[6]; // Adjusting to account for <1 star and 5 star ratings

            foreach (var item in feedbackData)
            {
                if (item.Rating < 1)
                {
                    feedbackCounts[0] += item.Count; // Count for <1 star
                }
                else if (item.Rating <= 2)
                {
                    feedbackCounts[1] += item.Count; // Count for 1-2 stars
                }
                else if (item.Rating <= 3)
                {
                    feedbackCounts[2] += item.Count; // Count for 2-3 stars
                }
                else if (item.Rating <= 4)
                {
                    feedbackCounts[3] += item.Count; // Count for 3-4 stars
                }
                else if (item.Rating <= 5)
                {
                    feedbackCounts[4] += item.Count; // Count for 4-5 stars
                }
            }

            // Pass data to ViewData
            ViewData["TodaysSales"] = todaysSales.ToString("N2"); // Format as decimal with 2 decimal places
            ViewData["SalesComparison"] = salesComparison.ToString("F2"); // Format as decimal with 2 decimal places
            ViewData["TotalTours"] = totalTours;
            ViewData["TotalTourGuides"] = totalTourGuides;
            ViewData["MonthlySales"] = monthlySales.ToString("N2"); // Format as decimal with 2 decimal places
            ViewData["MonthlyComparison"] = monthlyComparison.ToString("F2"); // Format as decimal with 2 decimal places
            ViewData["DistinctYears"] = distinctYears;
            ViewData["FeedbackCounts"] = feedbackCounts;

            // Prepare monthly sales data for the chart
            var monthlySalesData = GetMonthlySalesData(today.Year); // Get sales data for the current year
            ViewData["MonthlySalesData"] = monthlySalesData;

            return View();
        }

        private decimal CalculateSalesComparison(DateTime today)
        {
            var yesterdaySales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Date == today.AddDays(-1))
                .Sum(p => p.Amount);

            if (yesterdaySales == 0) return 0; // Avoid division by zero

            var todaysSales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Date == today)
                .Sum(p => p.Amount);

            return ((todaysSales - yesterdaySales) / yesterdaySales) * 100; // Percentage change
        }

        private decimal CalculateMonthlyComparison(DateTime today)
        {
            var lastMonthSales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Month == today.AddMonths(-1).Month && p.PaymentDate.Year == today.Year)
                .Sum(p => p.Amount);

            if (lastMonthSales == 0) return 0; // Avoid division by zero

            var currentMonthSales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Month == today.Month && p.PaymentDate.Year == today.Year)
                .Sum(p => p.Amount);

            return ((currentMonthSales - lastMonthSales) / lastMonthSales) * 100; // Percentage change
        }

        private decimal[] GetMonthlySalesData(int year)
        {
            decimal[] monthlySalesData = new decimal[12];

            for (int month = 1; month <= 12; month++)
            {
                monthlySalesData[month - 1] = _traveltestContext.Payments
                    .Where(p => p.PaymentDate.Year == year && p.PaymentDate.Month == month)
                    .Sum(p => p.Amount);
            }

            return monthlySalesData;
        }

        //[Authorize(Enum.RoleEnum.Admin)]
        // GET: TourGuide/CreateTourGuide
        [AllowAnonymous]
        public IActionResult CreateTourGuide()
        {
            return View();
        }

        // POST: TourGuide/CreateTourGuide
        [HttpPost]
        //[Authorize(Enum.RoleEnum.Admin)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTourGuide([Bind("FirstName,LastName,PhoneNumber,Email,Description")] TourGuide tourGuide, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                tourGuide.FirstName = tourGuide.FirstName?.Trim();
                tourGuide.LastName = tourGuide.LastName?.Trim();
                tourGuide.PhoneNumber = tourGuide.PhoneNumber?.Trim();
                tourGuide.Email = tourGuide.Email?.Trim();
                tourGuide.Description = tourGuide.Description?.Trim();

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(stream);
                        stream.Position = 0;

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, stream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        tourGuide.Image = uploadResult.SecureUrl.ToString();
                    }

                    _traveltestContext.Add(tourGuide);
                    await _traveltestContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(TourGuideManagement));
            }

            return View(tourGuide);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditTourGuide(int id)
        {
            var tourGuide = await _traveltestContext.TourGuides.FindAsync(id);
            if (tourGuide == null)
            {
                return NotFound();
            }

            return View(tourGuide);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> EditTourGuide([Bind("TourGuideId,FirstName,LastName,PhoneNumber,Email,Description,Image,Rate")] TourGuide tourGuide, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingTourGuide = await _traveltestContext.TourGuides.FindAsync(tourGuide.TourGuideId);

                if (existingTourGuide == null)
                {
                    return NotFound();
                }

                existingTourGuide.FirstName = tourGuide.FirstName.Trim();
                existingTourGuide.LastName = tourGuide.LastName.Trim();
                existingTourGuide.PhoneNumber = tourGuide.PhoneNumber.Trim();
                existingTourGuide.Email = tourGuide.Email.Trim();
                existingTourGuide.Description = tourGuide.Description.Trim();

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(stream);
                        stream.Position = 0;

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, stream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        existingTourGuide.Image = uploadResult.SecureUrl.ToString();
                    }
                }
                else
                {
                    existingTourGuide.Image = tourGuide.Image;
                }

                _traveltestContext.Update(existingTourGuide);
                await _traveltestContext.SaveChangesAsync();

                return RedirectToAction(nameof(TourGuideManagement));
            }

            return View(tourGuide);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TourGuideManagement(string searchQuery, int page = 1, int pageSize = 1)
        {
            var query = _traveltestContext.TourGuides.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(tg =>
                    tg.Email.Contains(searchQuery) ||
                    tg.FirstName.Contains(searchQuery) ||
                    tg.LastName.Contains(searchQuery) ||
                    (tg.FirstName + " " + tg.LastName).Contains(searchQuery) ||
                    tg.PhoneNumber.Contains(searchQuery));
            }

            var tourGuides = await query.ToListAsync();

            var tourGuidesToDisplay = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Apply pagination

            var totalItems = tourGuides.Count; // Total items before pagination

            var pagedResult = new PagedResult<TourGuide>
            {
                Items = tourGuidesToDisplay,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            ViewData["SearchQuery"] = searchQuery;
            return View(pagedResult);
        }

        public IActionResult ListRegisterTravelAgent()
        {
            DAO dal = new DAO();
            List<User> listuserregistertravelagent = dal.GetListUserRegisterTravelAgent();
            ViewBag.ListUserTravelAgent = listuserregistertravelagent;
            return View();
        }

        public IActionResult ListAccount()
        {
            DAO dal = new DAO();
            List<User> ListAccount = dal.GetListAccount();
            ViewBag.ListAccount = ListAccount;
            return View();


        }

        [AllowAnonymous]
        public async Task<IActionResult> FeedbackManagement(string searchQuery, int page = 1, int pageSize = 3)
        {
            var query = _traveltestContext.Feedbacks
                .Include(f => f.User); // Include the User information

            // Fetch all feedbacks first
            var allFeedbacks = await query.ToListAsync();

            // Create a list of replied feedback IDs
            var repliedFeedbackIds = allFeedbacks
                .Where(f => allFeedbacks.Any(r => r.ParentId == f.FeedbackId))
                .Select(f => f.FeedbackId)
                .ToList();

            // Apply the search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                allFeedbacks = allFeedbacks.Where(f => f.Content.Contains(searchQuery) ||
                                                       f.User.FirstName.ToLower().Contains(searchQuery.ToLower()) ||
                                                       f.User.LastName.ToLower().Contains(searchQuery.ToLower()) ||
                                                       (f.User.FirstName.ToLower() + " " + f.User.LastName.ToLower()).Contains(searchQuery.ToLower())).ToList();
            }

            // Filter out feedbacks with EntityId = 6 for display
            var feedbacksToDisplay = allFeedbacks
                .Where(f => f.EntityId != 6)
                .OrderByDescending(f => f.CreatedDate) // Order by creation date
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Apply pagination

            var totalItems = allFeedbacks.Count; // Total items before pagination

            var pagedResult = new PagedResult<Feedback>
            {
                Items = feedbacksToDisplay,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            ViewData["RepliedFeedbackIds"] = repliedFeedbackIds;
            ViewData["SearchQuery"] = searchQuery;

            return View(pagedResult);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ReplyFeedback(int id)
        {
            var feedback = _traveltestContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return NotFound();
            }

            var user = _traveltestContext.Users.FirstOrDefault(u => u.UserId == feedback.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ReplyFeedbackViewModel
            {
                Feedback = feedback,
                UserAvatar = user.Image,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReplyFeedback(ReplyFeedbackViewModel model)
        {
            var u = (User)HttpContext.Items["User"];

            var replyFeedback = new Feedback();

            if (!ModelState.IsValid)
            {
                // Set the ParentId to the FeedbackId of the feedback being replied to
                replyFeedback.ParentId = model.Feedback.FeedbackId;
                replyFeedback.UserId = u.UserId;
                replyFeedback.EntityId = 6;
                replyFeedback.Content = model.ReplyContent;
                replyFeedback.Rating = double.Parse(model.Rating);
                replyFeedback.CreatedDate = DateTime.UtcNow;
                replyFeedback.ModifiedDate = null; // or set the modified date if needed

                // Add the new feedback to the database
                _traveltestContext.Feedbacks.Add(replyFeedback);
                await _traveltestContext.SaveChangesAsync();

                // Redirect to the feedback management page or another appropriate page
                return RedirectToAction("FeedbackManagement", new { page = 1 });
            }

            // If the model state is invalid, return the view with the existing data to show validation errors
            return View(replyFeedback);
        }
        
        public IActionResult RequestUnaccept(int id, string email)
        {
            DAO dal = new DAO();
            string fromEmail = "duydqhe163434@fpt.edu.vn";
            string toEmail = email;
            string subject = "Hello " + email;

            string body = "Vì Một Số Lý Do Nào Đó Từ Phía Của Chúng Tôi Không Thể Cung Cấp Dịch Vụ Travelagent Cho Bạn Được Nữa Mọi Chi Tiết Xin Vui Lòng Liên Hệ Admin ";
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "duydqhe163434@fpt.edu.vn";
            string smtpPassword = "htay mxgi flsx dxde";
            bool result = SendEmail.theSendEmailRegisterTravelAgent(fromEmail, toEmail, subject, body, smtpServer, smtpPort, smtpUsername, smtpPassword);
            string stt = "Unaccept";
            dal.AccessRegisterTravelAgent(id, stt);
            return RedirectToAction("ListRegisterTravelAgent", "Admin");
        }

        public IActionResult ResetPass(int id, string email)
        {
            DAO dal = new DAO();
            
          
            dal.ResetPass(id, email);
            return RedirectToAction("ListAccount", "Admin");
        }

        public IActionResult AddAccount(int mess)
        {
            ViewBag.mess = mess;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAccountAccess()
        {
            traveltestContext context = new traveltestContext();
            DAO dal = new DAO();
            String Username = "";
            Username = HttpContext.Request.Form["username"];
            String Pass = "";
            Pass = HttpContext.Request.Form["pass"];
            String Cf_Pass = "";
            Cf_Pass = HttpContext.Request.Form["Confirm-Password"];

            String FirstName = "";
            FirstName = HttpContext.Request.Form["FirstName"];
            String LastName = "";
            LastName = HttpContext.Request.Form["LastName"];
            String PhoneNumber = "";
            PhoneNumber = HttpContext.Request.Form["PhoneNumber"];
            String Gender = "";
            Gender = HttpContext.Request.Form["Gender"];
            String SelectAccount = "";
            SelectAccount = HttpContext.Request.Form["SelectAccount"];
            IFormFile imageFile = HttpContext.Request.Form.Files["imageFile"];
         



            //Check Email
            if (dal.IsEmailValid(Username) == true && Pass == Cf_Pass && dal.IsPhoneNumberValidVietnam(PhoneNumber) == true && dal.IsValidFirstnameorLastname(FirstName) == true && dal.IsValidFirstnameorLastname(LastName) == true)
            {

                User usercheck = context.Users.Where(x => x.Email == Username).FirstOrDefault();

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(stream);
                        stream.Position = 0;

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, stream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        User user = new User()
                        {

                            Email = HttpContext.Request.Form["username"],
                            Password = HttpContext.Request.Form["pass"],
                            FirstName = HttpContext.Request.Form["FirstName"],
                            LastName = HttpContext.Request.Form["LastName"],
                            PhoneNumber = HttpContext.Request.Form["PhoneNumber"],
                            RoleId = Convert.ToInt32(HttpContext.Request.Form["SelectAccount"]),
                            Action = true,
                            Image = uploadResult.SecureUrl.ToString(),
                            Gender = Convert.ToBoolean(Convert.ToInt32(HttpContext.Request.Form["Gender"]))
                        };
                        
                        if (usercheck == null)
                        {
                            context.Add(user);
                            context.SaveChanges();
                            return RedirectToAction("ListAccount", "Admin");
                        }
                        else
                        {                           
                            return RedirectToAction("AddAccount", "Admin", new { mess = 1 });
                        }

                    }


                }

                return RedirectToAction("ListAccount", "Admin");

            }
            else
            {
                return RedirectToAction("AddAccount", "Admin", new { mess = 1 });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ImportTourGuide(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("TourGuideManagement"); // Return to the main page if no file was uploaded
            }

            // Process the Excel file and extract data into a list of TourGuideDTO objects
            TourGuidePreviewViewModel model = ExtractTourGuidesFromExcel(file);

            return View("PreviewTourGuide", model);
        }

        private TourGuidePreviewViewModel ExtractTourGuidesFromExcel(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                        var records = csv.GetRecords<TourGuideDTO>().ToList();

                        var listTourGuide = _traveltestContext.TourGuides.ToList();

                        var nonDuplicateList = new List<TourGuideDTO>();
                        var duplicateList = new List<TourGuideDTO>();

                        foreach (var r in records)
                        {
                            var existingTourGuide = listTourGuide.FirstOrDefault(x =>
                                x.FirstName.ToLower() == r.FirstName.ToLower() &&
                                x.LastName.ToLower() == r.LastName.ToLower() &&
                                x.PhoneNumber.ToLower() == r.PhoneNumber.ToLower() &&
                                x.Email.ToLower() == r.Email.ToLower());

                            if (existingTourGuide == null)
                            {
                                nonDuplicateList.Add(r);
                            }
                            else
                            {
                                duplicateList.Add(r);
                            }
                        }

                        // Return as a tuple
                        return new TourGuidePreviewViewModel
                        {
                            NonDuplicates = nonDuplicateList,
                            Duplicates = duplicateList
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the Excel file: " + ex.Message);
            }
        }

    }

    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }

    public class ReplyFeedbackViewModel
    {
        public Feedback Feedback { get; set; }
        public string UserAvatar { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string ReplyContent { get; set; }
        public string Rating { get; set; }  
    }

    public class TourGuideDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Description { get; set; }
    }

    public class TourGuidePreviewViewModel
    {
        public List<TourGuideDTO> NonDuplicates { get; set; }
        public List<TourGuideDTO> Duplicates { get; set; }
    }
}

