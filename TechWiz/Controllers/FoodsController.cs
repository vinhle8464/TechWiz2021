using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{

    [Route("foods")]
    public class FoodsController : Controller
    {
        private readonly IFoodService iFoodService;
        private readonly IFoodsService foodsService;
        private readonly ICategoriesService iCategoriesService;

        public FoodsController(IFoodService iFoodService, IFoodsService foodsService, ICategoriesService iCategoriesService)
        {

            this.iFoodService = iFoodService;
            this.foodsService = foodsService;
            this.iCategoriesService = iCategoriesService;
        }

        [Route("index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            ViewBag.titlePage = "All Foods";
            return View(await iFoodService.GetAllFood());
        }

        [Route("category/{idCategory}")]
        public async Task<IActionResult> SearchWithCategory(int idCategory)
        {
            var category = await iCategoriesService.FindCategory(idCategory);
            if (category == null) return RedirectToAction("Index", "Home");
            ViewBag.titlePage = category.Name;
            return View("Index", await iFoodService.GetFoodCategory(idCategory));
        }



        [Route("menu")]
        public async Task<IActionResult> Index([FromQuery(Name = "page")] int page)
        {
            var loadFood = await foodsService.LoadFoods();
            Load(page, loadFood);
            return View("Menu");
        }
        public void Load(int page, List<Food> foods)
        {
            HttpContext.Session.SetString("pageNumber", page.ToString());
            var paging = foodsService.Paging(page, foods);
            ViewBag.paging = paging;
            ViewBag.pageNumber = paging.amountOfPages;
            ViewBag.foods = paging.foods;
            ViewBag.currentPage = HttpContext.Session.GetString("pageNumber");
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] int page, [FromQuery(Name = "categoryId")] int categoryId, [FromQuery(Name = "restaurantId")] int restaurantId)
        {
            var search = await foodsService.SearchByFoodName(keyword);
            if (categoryId != 0)
            {
                search = await foodsService.FilterByCategories(keyword, categoryId);
                ViewBag.categoryId = categoryId;
                if (restaurantId != 0)
                {
                    search = await foodsService.SearchByFoodName(keyword, categoryId, restaurantId);
                    ViewBag.categoryId = categoryId;
                    ViewBag.restaurantId = restaurantId;
                }
            }
            else if (restaurantId != 0)
            {
                search = await foodsService.FilterByRestaurants(keyword, restaurantId);
                ViewBag.restaurantId = restaurantId;
                if (categoryId != 0)
                {
                    search = await foodsService.SearchByFoodName(keyword, categoryId, restaurantId);
                    ViewBag.categoryId = categoryId;
                    ViewBag.restaurantId = restaurantId;
                }
            }

            Load(page, search);

            ViewBag.keyword = keyword;

            if (ViewBag.pageNumber == 0)
            {
                ViewBag.message = "The dish was not found";
                return View("Menu");
            }
            else if (page == 0)
            {
                return RedirectToAction("menu", "StatusCode",
             new { statusCode = 404 });
            }
            else if (page > ViewBag.pageNumber)
            {
                return RedirectToAction("menu", "StatusCode",
             new { statusCode = 404 });
            }
            return View("Menu");
        }
    }
}
