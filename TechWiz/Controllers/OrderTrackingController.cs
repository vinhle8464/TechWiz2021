using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("ordertrackings")]
    public class OrderTrackingsController : Controller
    {
        private OrderTrackingService orderTrackingService;
        private IWebHostEnvironment webHostEnvironment;

        public OrderTrackingsController(OrderTrackingService _orderTrackingService, IWebHostEnvironment webHostEnvironment)
        {
            this.orderTrackingService = _orderTrackingService;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: OrderTrackings
        [Route("index")]
        [Route("")]
        public async Task<IActionResult> Index(int idOrder)
        {

            return View(await orderTrackingService.GetAllOrderTrackingAndAllElementInOrderDetailsById(idOrder));
        }

        // GET: OrderTrackings/Details/5
        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Debug.WriteLine("idorder" + id);
            var orderTracking = await orderTrackingService.GetOrderTrackingDetails(id);
            if (orderTracking == null)
            {
                return NotFound();
            }

            return View(orderTracking);
        }


        [HttpGet]
        [Route("accepted")]
        public async Task<IActionResult> Accepted(int id)
        {

            await orderTrackingService.AcceptOrder(id);


            return RedirectToAction("index", "orders");

        }

      
    }
}
