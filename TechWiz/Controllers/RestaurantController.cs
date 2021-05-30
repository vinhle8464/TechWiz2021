using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models.DTO;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("restaurants")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService restaurantService;
        private readonly TagService tagService;
        public RestaurantController(IRestaurantService restaurantService, TagService tagService)
        {
            this.restaurantService = restaurantService;
            this.tagService = tagService;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var listResDto = new List<RestaurantDTO>();
            var listRestaurant = await restaurantService.LoadAllRestaurant();
            foreach (var item in listRestaurant)
            {

                var restDto = new RestaurantDTO {Restaurant = item};
                if (item.IdTag != null)
                {
                    var tag = await tagService.Find(item.IdTag);
                    restDto.Tag = tag.NameTag;
                }
                listResDto.Add(restDto);
            }

            ViewBag.restaurants = listResDto;
            return View();
        }

        [Route("details/{idRes}")]
        public async Task<IActionResult> Details(int idRes)
        {
            var restaurant = await restaurantService.GetInfoRestaurant(idRes);
            if (restaurant == null) return RedirectToAction("Index", "Home");
            ViewBag.restaurantDetail = restaurant;
            ViewBag.listFood = await restaurantService.FindFood(idRes);
            ViewBag.cmt = await restaurantService.FindCmt(idRes);
            return View();
        }

    }
}
