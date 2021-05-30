using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ICartService iCartService;

        public CartController(ICartService iCartService, DatabaseContext context)
        {
            this.iCartService = iCartService;
            _context = context;
        }

        [Route("index")]
        [Route("")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") == null) return RedirectToAction("index", "Home");
            var userId = (int)HttpContext.Session.GetInt32("id");
            ViewBag.cartItems = iCartService.List(userId);
            return View();
        }
        
        [Route("create")]
        public IActionResult Create()
        {
            return View("create");
        }
        
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FoodId,Quantity,Status,CreatedAt,UpdatedAt")] Cart cart)
        {
            if (!ModelState.IsValid) return View(cart);
            await iCartService.Create(cart);
            return RedirectToAction("index");
        }
        
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(CartItem cartItem)
        {
            var cart = _context.Carts.SingleOrDefault(m => m.UserId == cartItem.UserId && m.FoodId == cartItem.FoodId);
            cart.Quantity = cartItem.Quantity;
            _context.Update(cart);
            await _context.SaveChangesAsync();
            ViewBag.cartItems = iCartService.List(cartItem.UserId);
            return RedirectToAction("index");
        }

        [Route("delete/{userid}/{foodId}")]
        public async Task<IActionResult> Delete(int userid, int foodId)
        {
            try
            {
                var cart = _context.Carts.SingleOrDefault(m => m.UserId == userid && m.FoodId == foodId);
                cart.Quantity = 66;

                _context.Remove(cart);
                await _context.SaveChangesAsync();
                ViewBag.cartItems = iCartService.List(userid);
                return RedirectToAction("index");

            }
            catch (Exception)
            {

                return NotFound();
            }

        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.UserId == id);
        }

        [Route("checkout")]
        public IActionResult CheckOut()
        {
            var idUser = (int)HttpContext.Session.GetInt32("id");
            var listCart = iCartService.List(idUser);
            HttpContext.Session.SetString("listcart", JsonConvert.SerializeObject(listCart));

            return RedirectToAction("create", "orders");

        }
    }
}
