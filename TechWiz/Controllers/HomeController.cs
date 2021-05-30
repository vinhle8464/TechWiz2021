
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models.DTO;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly IUserService userService;
        private readonly CategoryService categoryService;
        private readonly IFoodService iFoodService;
        private readonly IRestaurantService iRestaurantService;
        private readonly TagService tagService;

        public HomeController(IUserService _userService, CategoryService _categoryService, IFoodService iFoodService, IRestaurantService _iRestaurantService, TagService _tagService)
        {
            userService = _userService;
            categoryService = _categoryService;
            this.iFoodService = iFoodService;
            iRestaurantService = _iRestaurantService;
            tagService = _tagService;
        }

        [Route("")]
        [Route("~/")]
        public async Task<IActionResult> Index()
        {
            var listResDto = new List<RestaurantDTO>();
            var listRestaurant = await iRestaurantService.LoadRestaurant(4);
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
            ViewBag.foods = await iFoodService.GetFoodNumber(12);
            ViewBag.categories = await categoryService.GetListCategory(10);
            return View("index");
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await userService.EmailExists(username);
            if (user == null)
            {
                ViewBag.msg = "Login unsuccessful, please try again!";
                return View();
            }
            if (user.Password != ServiceCommon.ServiceCommon.ComputeSha256Hash(password, user.Salt)) return View();
            HttpContext.Session.SetString("username", user.FirstName + user.LastName);
            HttpContext.Session.SetInt32("id", user.Id);
            return user.Role switch
            {
                "customer" => RedirectToAction("index"),
                "admin" => Redirect("/admin/home/index"),
                "staff" => RedirectToAction("Login"),
                _ => RedirectToAction("Login")
            };
        }

        [Route("Logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("id");
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }
    }
}

