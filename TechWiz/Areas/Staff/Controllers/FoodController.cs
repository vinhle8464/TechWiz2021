using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Areas.Staff.Controllers
{
    [Route("staff")]
    [Route("staff/food")]
    public class FoodController : Controller
    {
        private readonly IFoodService iFoodService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly TagService tagService;
        private readonly IFoodsService foodsService;
        public FoodController(IFoodService iFoodService, IFoodsService foodsService, IWebHostEnvironment webHostEnvironment, TagService tagService)
        {

            this.iFoodService = iFoodService;
            this.foodsService = foodsService;
            this.webHostEnvironment = webHostEnvironment;
            this.tagService = tagService;
        }

        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> Details(int id, int RestaurantId, int CategoryId)
        {

            Debug.WriteLine("resta" + RestaurantId);

            Debug.WriteLine("carte" + CategoryId);
            var food = await iFoodService.GetFoodDetail(id);
            var restaurantAndCategory = await iFoodService.GetRestaurantAndCategory(id, RestaurantId, CategoryId);
            string[] result = restaurantAndCategory.Split(new char[] { '-' });

            ViewBag.restaurant = result[0];
            ViewBag.category = result[1];
            return View("details", food);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View("create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create(IFormFile photo, [Bind("Id,RestaurantId,CategoryId,Name,Description,Price,Images,Status,CreatedAt,UpdatedAt")] Food food)
        {
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Foods", fileName + "." + ext);
                await using (var file = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(file);
                }
                food.Images = fileName + "." + ext;
            }
            if (!ModelState.IsValid) return View("create", food);
            food.CreatedAt = DateTime.Now;
            food.UpdatedAt = DateTime.Now;
            food.Status = 1;
            await iFoodService.Create(food);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var food = await iFoodService.Edit(id);
            ViewBag.restaurants = await iFoodService.GetRestaurant();
            ViewBag.categories = await iFoodService.GetCategory();
            if (food == null)
            {
                return NotFound();
            }
            return View("edit", food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RestaurantId,CategoryId,Name,Description,Price,Images,Status,CreatedAt,UpdatedAt")] Food food, IFormFile photo)
        {
            if (id != food.Id)
            {
                return NotFound();
            }
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Foods", fileName + "." + ext);
                await using (var file = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(file);
                }
                food.Images = fileName + "." + ext;
            }

            if (!ModelState.IsValid) return View("edit", food);
            try
            {
                food.UpdatedAt = DateTime.Now;
                await iFoodService.Update(food);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!iFoodService.FoodExists(food.Id))
                {
                    return NotFound();
                }
                throw;

            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await iFoodService.GetFoodDetail(id);
            if (food == null)
            {
                return NotFound();
            }
            food.UpdatedAt = DateTime.Now;
            food.Status = 0;
            await iFoodService.Update(food);
            return RedirectToAction("index");
        }
    }
}
