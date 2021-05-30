﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private DatabaseContext db;

        public OrderServiceImpl(DatabaseContext db)
        {
            this.db = db;
        }

        public async Task Create(Order order, List<CartItem> listcart)
        {
            var ord = AddOrder(order);

            AddOrderDetails(order.Id, listcart);
            AddOrderTrackings(ord);
            var queryaccounts =
            from ordertracking in db.OrderTrackings
            where
            ordertracking.OrderId == ord.Id && ordertracking.State != "Process"
            select ordertracking;
            foreach (var ordertracking in queryaccounts)
            {
                ordertracking.Status = 0;
                Debug.WriteLine("state " + ordertracking.State);
                Debug.WriteLine("status " + ordertracking.Status);

            }
            DeleteAllCart();
            await db.SaveChangesAsync();

        }
        private Order AddOrder(Order order)
        {
            db.Add(order);
            db.SaveChanges();
            return order;

        }
        private async void AddOrderDetails(int idorder, List<CartItem> listcart)
        {
            foreach (var item in listcart)
            {
                await db.OrderDetails.AddAsync(
                 new OrderDetail { OrderId = idorder, FoodId = item.FoodId, FoodOrigin = item.Name, Price = item.Price, Quantity = item.Quantity, Discount = 0, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }

                );
            }

            //  await db.SaveChangesAsync();
        }

        private async void AddOrderTrackings(Order order)
        {

            await db.OrderTrackings.AddRangeAsync(
              new OrderTracking { OrderId = order.Id, State = "Process", Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "Accepted", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "Prepared", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "Wait for Shipper", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "On The Way", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "Delivered", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
              new OrderTracking { OrderId = order.Id, State = "Cancel", Status = 0, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
              );




        }

        public async Task Cancel(int? id)
        {
            var order = (from x in db.Orders
                         where x.Id == id
                         select x).First();
            order.UpdatedAt = DateTime.Now;
            order.Status = 0;

            var orderTracking = (from x in db.OrderTrackings
                                 where x.OrderId == id && x.State == "Cancel"
                                 select x).First();
            orderTracking.Status = 1;

            await db.SaveChangesAsync();
        }

        public async Task<Order> Find(int? id)
        {
            return await db.Orders.FindAsync(id);
        }

        public async Task<List<Order>> GetAllOrder()
        {
            return await db.Orders.Where(p => p.Status == 1).ToListAsync();
        }



        public bool OrderExists(int id)
        {
            return db.Orders.Any(e => e.Id == id);

        }

        public async Task Update(Order order)
        {
            db.Update(order);
            await db.SaveChangesAsync();
        }

        public int GetIdRestaurant(int foodid)
        {
            return db.Foods.Where(p => p.Id == foodid).Select(x => x.RestaurantId).FirstOrDefault();

        }

        public void DeleteAllCart()
        {
            foreach (var entity in db.Carts)
                db.Carts.Remove(entity);
        }

        public async Task<List<OrderHistoryViewModel>> GetOrderHistory(int userId)
        {
            var ListOrderHistory = new List<OrderHistoryViewModel>();

            foreach (var item in await db.Orders.Where(p => p.Status == 0).ToListAsync())
            {

                var listOrderHistory = new OrderHistoryViewModel();
                listOrderHistory.OrderId = item.Id;
                listOrderHistory.Total = item.TotalPrice;
                var CheckOrdertracking = await db.OrderTrackings.FirstOrDefaultAsync(p => p.State == "Delivered" && p.Status == 1);
                listOrderHistory.State = CheckOrdertracking.State;
                listOrderHistory.Time = (DateTime)CheckOrdertracking.UpdatedAt;
                var CountOrderDetail = await db.OrderDetails.Where(p => p.OrderId == CheckOrdertracking.OrderId).ToListAsync();
                foreach (var item1 in CountOrderDetail)
                {
                    var food = await db.Foods.SingleOrDefaultAsync(p => p.Id == item1.FoodId);
                    listOrderHistory.Image = food.Images;
                }
                listOrderHistory.CountOrder = CountOrderDetail.Count();
                var ListRestaurant = await db.Restaurants.FirstOrDefaultAsync(p => p.Id == item.ShipperId);
                listOrderHistory.NameRestaurant = ListRestaurant.Name;

                // addd list ListOrderHistory
                ListOrderHistory.Add(listOrderHistory);
                //===============            
            }

            return ListOrderHistory;
        }

        public async Task<List<CartItem>> GetCartItemPreOrder(int userId, int orderId)
        {
            var ListCartItem = new List<CartItem>();

            foreach (var item in await db.OrderDetails.Where(p => p.OrderId == orderId).ToListAsync())
            {
                var cartItem = new CartItem();
       
                cartItem.UserId = userId;
                cartItem.FoodId = (int)item.FoodId;
                cartItem.Price = item.Price;
                cartItem.Quantity = item.Quantity;
                var food = await db.Foods.FirstOrDefaultAsync(p => p.Id == item.FoodId);
                cartItem.Name = food.Name;
                cartItem.Description = food.Description;
                cartItem.Images = food.Images;

                // add list ListCartItem
                ListCartItem.Add(cartItem);

            }
            foreach (var item in ListCartItem)
            {
                var CheckCart = db.Carts.SingleOrDefault(p => p.UserId == userId && p.FoodId == item.FoodId);
                if (CheckCart != null)
                {
                    CheckCart.Quantity += item.Quantity;
                    db.Update(CheckCart);
                    await db.SaveChangesAsync();

                }
                else
                {                  
                        db.Add(new Cart { UserId = item.UserId, FoodId = item.FoodId, Quantity = item.Quantity, Status = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
                        await db.SaveChangesAsync();                    
                }
                
            }
           
            return ListCartItem;
        }
    }
}
