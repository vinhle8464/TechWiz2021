using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("FoodRating")]
    public class FoodRatingController : Controller
    {
        private readonly IFoodRatingService foodRatingService;
        public FoodRatingController(IFoodRatingService _foodRatingService)
        {
            foodRatingService = _foodRatingService;
        }
        [Route("rating")]
        public async Task<IActionResult> Rating(int idFood)
        {
            ViewBag.idfood = idFood;
            ViewBag.listCmt = await foodRatingService.FindComment(idFood);
            return View();
        }
        [Route("rating")]
        [HttpPost]
        public async Task<IActionResult> Rating(FoodRating foodRating)
        {
            if (HttpContext.Session.GetInt32("id") == null) return Redirect("/home/login");
            foodRating.UserId = (int)HttpContext.Session.GetInt32("id");
            foodRating.CreatedAt = DateTime.Now;
            foodRating.Status = 1;
            await foodRatingService.Add(foodRating);
            return RedirectToAction("index", new RouteValueDictionary(new { controller = "home" }));
        }
    }
}
