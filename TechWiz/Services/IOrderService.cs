﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public interface IOrderService
    {
        public Task<List<Order>> GetAllOrder();
        public bool OrderExists(int id);
        public Task Create(Order order, List<CartItem> listcart);
        public Task<Order> Find(int? id);
        public Task Update(Order order);
        public Task Cancel(int? id);
        public int GetIdRestaurant(int foodid);
        public Task<List<OrderHistoryViewModel>> GetOrderHistory(int userId);
        public Task<List<CartItem>> GetCartItemPreOrder(int userId, int orderId);
    }
}
