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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            var today = DateTime.Today;

            // Today's Sales
            var todaysSales = _traveltestContext.Payments
                .Where(p => p.PaymentDate.Date == today)
                .Sum(p => p.Amount);

            // Total Tours
            var totalTours = _traveltestContext.Tours.Count(); 

            // Total Tour Guides
            var totalTourGuides = _traveltestContext.TourGuides.Count(); 

            // Monthly Sales
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var monthlySales = _traveltestContext.Payments
                .Where(p => p.PaymentDate >= startOfMonth)
                .Sum(p => p.Amount);

            // Prepare comparison values
            var salesComparison = CalculateSalesComparison(today); 
            var monthlyComparison = CalculateMonthlyComparison(today); 

            var distinctYears = _traveltestContext.Payments
                .Select(p => p.PaymentDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Fetch feedback ratings
            var feedbackData = _traveltestContext.Feedbacks
                .Where(f => f.ParentId == null)
                .GroupBy(f => f.Rating)
                .Select(g => new
                {
                    Rating = g.Key,
                    Count = g.Count()
                })
                .OrderBy(f => f.Rating)
                .ToList();

            // Prepare data for feedback ratings
            var feedbackCounts = new int[6]; 

            foreach (var item in feedbackData)
            {
                if (item.Rating < 1)
                {
                    feedbackCounts[0] += item.Count; 
                }
                else if (item.Rating <= 2)
                {
                    feedbackCounts[1] += item.Count; 
                }
                else if (item.Rating <= 3)
                {
                    feedbackCounts[2] += item.Count; 
                }
                else if (item.Rating <= 4)
                {
                    feedbackCounts[3] += item.Count; 
                }
                else if (item.Rating <= 5)
                {
                    feedbackCounts[4] += item.Count; 
                }
            }

            // Pass data to ViewData
            ViewData["TodaysSales"] = todaysSales.ToString("N2"); 
            ViewData["SalesComparison"] = salesComparison.ToString("F2"); 
            ViewData["TotalTours"] = totalTours;
            ViewData["TotalTourGuides"] = totalTourGuides;
            ViewData["MonthlySales"] = monthlySales.ToString("N2"); 
            ViewData["MonthlyComparison"] = monthlyComparison.ToString("F2"); 
            ViewData["DistinctYears"] = distinctYears;
            ViewData["FeedbackCounts"] = feedbackCounts;

            // Prepare monthly sales data for the chart
            var monthlySalesData = GetMonthlySalesData(today.Year); 
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddTourGuide(List<string> FirstNames, List<string> LastNames, List<string> Emails, List<string> PhoneNumbers, List<string> Descriptions)
        {
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            if (FirstNames != null && FirstNames.Count > 0)
            {
                for (int i = 0; i < FirstNames.Count; i++)
                {
                    
                    var tourGuide = new TourGuide
                    {
                        FirstName = FirstNames[i],
                        LastName = LastNames[i],
                        Email = Emails[i],
                        PhoneNumber = PhoneNumbers[i],
                        Description = Descriptions[i]
                    };

                     _traveltestContext.TourGuides.Add(tourGuide);
                    _traveltestContext.SaveChanges();   
                }
            }

            // Chuyển hướng hoặc trả về view sau khi thêm thành công
            return RedirectToAction("TourGuideManagement"); // Hoặc view khác
        }

        //[Authorize(Enum.RoleEnum.Admin)]
        // GET: TourGuide/CreateTourGuide
        [AllowAnonymous]
        public IActionResult CreateTourGuide()
        {
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;
            return View();
        }

        // POST: TourGuide/CreateTourGuide
        [HttpPost]
        //[Authorize(Enum.RoleEnum.Admin)]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTourGuide([Bind("FirstName,LastName,PhoneNumber,Email,Description")] TourGuide tourGuide, IFormFile imageFile)
        {
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            var query = _traveltestContext.TourGuides.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(tg =>
                    tg.Email.Contains(searchQuery.Trim()) ||
                    tg.FirstName.Contains(searchQuery.Trim()) ||
                    tg.LastName.Contains(searchQuery.Trim()) ||
                    (tg.FirstName + " " + tg.LastName).Contains(searchQuery.Trim()) ||
                    tg.PhoneNumber.Contains(searchQuery.Trim()));
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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

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
                                                       f.User.FirstName.ToLower().Contains(searchQuery.ToLower().Trim()) ||
                                                       f.User.LastName.ToLower().Contains(searchQuery.ToLower().Trim()) ||
                                                       (f.User.FirstName.ToLower() + " " + f.User.LastName.ToLower()).Contains(searchQuery.ToLower().Trim())).ToList();
            }

            // Filter out feedbacks with EntityId = 6 for display
            var feedbacksToDisplay = allFeedbacks
                .Where(f => f.EntityId != 6)
                .OrderByDescending(f => f.CreatedDate) // Order by creation date
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Apply pagination

            var totalItems = feedbacksToDisplay.Count; // Total items before pagination

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
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            var feedback = _traveltestContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);

            if (feedback == null)
            {
                return NotFound();
            }

            var user_1 = _traveltestContext.Users.FirstOrDefault(u => u.UserId == feedback.UserId);

            if (user_1 == null)
            {
                return NotFound();
            }

            // Check if there is an existing reply
            var existingReply = _traveltestContext.Feedbacks.FirstOrDefault(f => f.ParentId == feedback.FeedbackId && f.UserId == user.UserId);

            var viewModel = new ReplyFeedbackViewModel
            {
                Feedback = feedback,
                UserAvatar = user_1.Image,
                UserFirstName = user_1.FirstName,
                UserLastName = user_1.LastName,
                ReplyContent = existingReply?.Content.Trim() ?? "", // Populate existing reply content if it exists
                Rating = existingReply?.Rating.ToString() ?? "0" // Populate existing rating if it exists
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReplyFeedback(ReplyFeedbackViewModel model)
        {
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            var existingReply = _traveltestContext.Feedbacks.FirstOrDefault(f => f.ParentId == model.Feedback.FeedbackId && f.UserId == user.UserId);

            if (existingReply != null)
            {
                // Update existing reply
                existingReply.Content = model.ReplyContent;
                existingReply.Rating = double.Parse(model.Rating);
                existingReply.ModifiedDate = DateTime.Now;

                _traveltestContext.Feedbacks.Update(existingReply);
            }
            else
            {
                // Create a new reply
                var replyFeedback = new Feedback
                {
                    ParentId = model.Feedback.FeedbackId,
                    UserId = user.UserId,
                    EntityId = 6,
                    Content = model.ReplyContent.Trim(),
                    Rating = double.Parse(model.Rating),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = null
                };

                _traveltestContext.Feedbacks.Add(replyFeedback);
            }

            await _traveltestContext.SaveChangesAsync();

            return RedirectToAction("FeedbackManagement", new { page = 1 });
        }

        public IActionResult RequestUnaccept(int id, string email)
        {
            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

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

            var user = HttpContext.Items["User"] as User;
            ViewBag.User = user;

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file uploaded.";
                return RedirectToAction("TourGuideManagement");
            }

            try
            {
                // Process the Excel file and extract data into a list of TourGuideDTO objects
                TourGuidePreviewViewModel model = ExtractTourGuidesFromExcel(file);
                return View("PreviewTourGuide", model);
            }
            catch (Exception ex)
            {
                // Set the error message in TempData to display on the redirected page
                TempData["Error"] = ex.Message;
                return RedirectToAction("TourGuideManagement");
            }
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
                        csv.Read();
                        csv.ReadHeader();

                        var requiredColumns = new List<string> { "FirstName", "LastName", "PhoneNumber", "Email", "Description" };
                        var missingColumns = requiredColumns.Where(col => !csv.HeaderRecord.Contains(col)).ToList();

                        // Check for missing columns
                        if (missingColumns.Any())
                        {
                            throw new Exception($"The following required columns are missing in the CSV file: {string.Join(", ", missingColumns)}");
                        }

                        var records = csv.GetRecords<TourGuideDTO>().ToList();

                        var listTourGuide = _traveltestContext.TourGuides.ToList();

                        var nonDuplicateList = new List<TourGuideDTO>();
                        var duplicateList = new List<TourGuideDTO>();

                        foreach (var r in records)
                        {
                            var existingTourGuide = listTourGuide.FirstOrDefault(x =>
                                x.FirstName.ToLower().Trim() == r.FirstName.ToLower().Trim() &&
                                x.LastName.ToLower().Trim() == r.LastName.ToLower().Trim() &&
                                x.PhoneNumber.ToLower().Trim() == r.PhoneNumber.ToLower().Trim() &&
                                x.Email.ToLower().Trim() == r.Email.ToLower().Trim());

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

