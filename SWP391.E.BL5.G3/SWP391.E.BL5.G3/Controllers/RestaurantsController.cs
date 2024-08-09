using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391.E.BL5.G3.Models;
using X.PagedList;

namespace SWP391.E.BL5.G3.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly traveltestContext _context;

        public RestaurantsController(traveltestContext context)
        {
            _context = context;
        }

        // view list of restaurants
        public IActionResult ListRestaurants(string currentSearchString, string searchString, int? page)
        {
            var restaurants = new List<Restaurant>();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentSearchString;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                restaurants = _context.Restaurants.Where(item =>
                    item.RestaurantName.Contains(searchString)).ToList();
            }
            else
            {
                restaurants = _context.Restaurants.ToList();
            }

            ViewBag.currentSearchString = searchString;

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var totalItems = _context.Restaurants.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            restaurants = restaurants
                .OrderBy(item => item.RestaurantName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(restaurants);
        }

        // view details of a restaurant
        public IActionResult RestaurantDetails(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // add a new restaurant
        public IActionResult AddRestaurant()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRestaurant(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                restaurant.CreatedAt = DateTime.Now;
                restaurant.UpdatedAt = restaurant.CreatedAt;
                _context.Add(restaurant);
                _context.SaveChanges();
                return RedirectToAction(nameof(ListRestaurants));
            }
            return View(restaurant);
        }

        // edit restaurant
        public IActionResult EditRestaurant(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.Find(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [HttpPost]
        public IActionResult EditRestaurant(int id, Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    restaurant.CreatedAt = restaurant.CreatedAt;
                    restaurant.UpdatedAt = DateTime.Now;
                    _context.Update(restaurant);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckRestaurantExisted(restaurant.RestaurantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListRestaurants));
            }

            return View(restaurant);
        }

        // delete restaurant
        [HttpPost]
        public IActionResult DeleteRestaurant(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }

            var restaurant = _context.Restaurants.FirstOrDefault(item => item.RestaurantId == id);

            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ListRestaurants));
        }

        public bool CheckRestaurantExisted(int id)
        {
            return (_context.Restaurants?.Any(item => item.RestaurantId == id)).GetValueOrDefault();
        }

    }
}
