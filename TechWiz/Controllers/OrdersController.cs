
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("orders")]
    public class OrdersController : Controller
    {

        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [Route("index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await orderService.GetAllOrder());
        }
        
        [HttpGet]
        [Route("details")]
        public IActionResult Details(int id)
        {
            return RedirectToAction("Index", "ordertrackings" +
                "", new { idOrder = id });
        }

        [Route("create")]
        public async Task<IActionResult> Create()
        {
            var listCart = new List<CartItem>();
            if (HttpContext.Session.GetString("listcart") != null)
            {
                listCart = JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString("listcart"));

            }

            var order = new Order();
            double total = 0;
            var foodId = 0;

            foreach (var item in listCart)
            {
                total += (item.Price * item.Quantity);
                order.UserId = item.UserId;
                foodId = item.FoodId;
            }


            order.ShipperId = orderService.GetIdRestaurant(foodId);
            order.TotalPrice = total;
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;
            order.Status = 1;
            await orderService.Create(order, listCart);
            HttpContext.Session.Remove("listcart");
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

            var order = await orderService.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TotalPrice,ShipperId,Status,CreatedAt,UpdatedAt")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(order);
            try
            {
                order.UpdatedAt = DateTime.Now;
                await orderService.Update(order);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!orderService.OrderExists(order.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        [Route("cancel")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await orderService.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                await orderService.Cancel(id);
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History()
        {

            var userId = (int)HttpContext.Session.GetInt32("id");
            ViewBag.HistoryOrders = await orderService.GetOrderHistory(userId);
            return View("history");

        }

        [HttpGet]
        [Route("preorder")]
        public async Task<IActionResult> PreOrder(int OrderId)
        {
            int UserId = (int)HttpContext.Session.GetInt32("id");
            ViewBag.HistoryOrders = await orderService.GetCartItemPreOrder(UserId, OrderId);

            return RedirectToAction("history");
        }

    }
}
