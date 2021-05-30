using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWizProject.Areas.Admin.Services;
using TechWizProject.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TechWizProject.Controllers
{
    [Area("Admin")]
    [Route("Admin/Restaurants")]
    public class RestaurantsController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRestaurantsServies restaurantsServies;
        public RestaurantsController(IRestaurantsServies _restaurantsServies, IWebHostEnvironment _webHostEnvironment)
        {

            restaurantsServies = _restaurantsServies;
            webHostEnvironment = _webHostEnvironment;
        }
        
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.listRes = await restaurantsServies.FindAll();
            return View();
        }

        [Route("Detail")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await restaurantsServies.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var restaurants = new Restaurant();
            return View(restaurants);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerId,Name,Addr,Lat,Lng,Cover,Logo,ShippingFeePerKm,Status,CreatedAt,UpdatedAt")] Restaurant restaurant, IFormFile photo)
        {
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Restaurant", fileName + "." + ext);
                await using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(filestream);
                }
                restaurant.Logo = fileName + "." + ext;

            }

            if (!ModelState.IsValid) return View(restaurant);
            await restaurantsServies.Create(restaurant);
            return RedirectToAction(nameof(Index));
        }
        
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await restaurantsServies.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( IFormFile photo,int id, [Bind("Id,OwnerId,Name,Addr,Lat,Lng,Cover,Logo,ShippingFeePerKm,Status,CreatedAt,UpdatedAt")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Restaurant", fileName + "." + ext);
                await using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(filestream);
                }
                restaurant.Logo = fileName + "." + ext;

            }

            if (!ModelState.IsValid) return View(restaurant);
            try
            {
                await restaurantsServies.Edit(restaurant);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(restaurant.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await restaurantsServies.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }
        
        [HttpPost, ActionName("Delete")]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await restaurantsServies.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int id)
        {
            return restaurantsServies.ResExists(id);
        }
    }
}
