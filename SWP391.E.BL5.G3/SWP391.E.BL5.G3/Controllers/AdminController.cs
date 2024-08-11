using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SWP391.E.BL5.G3.Models;

namespace SWP391.E.BL5.G3.Controllers
{
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

        public IActionResult Index()
        {
            return View();
        }

        // GET: TourGuide/CreateTourGuide
        public IActionResult CreateTourGuide()
        {
            return View();
        }

        // POST: TourGuide/CreateTourGuide
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> EditTourGuide(int id)
        {
            var tourGuide = await _traveltestContext.TourGuides.FindAsync(id);
            if (tourGuide == null)
            {
                return NotFound();
            }

            return View(tourGuide);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> TourGuideManagement(string searchQuery)
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

            ViewData["SearchQuery"] = searchQuery;
            return View(tourGuides);
        }
    }

    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
