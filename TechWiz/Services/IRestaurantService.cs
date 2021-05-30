using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public interface IRestaurantService
    {
        public Task<List<Restaurant>> LoadAllRestaurant();
        public Task<List<Restaurant>> LoadRestaurant(int numberOfRestaurant);
        public Task<UserAndRestauants> FindCmt(int idRes);
        public Task<List<FoodAndCate>> FindFood(int idRes);
        public Task<Restaurant> GetInfoRestaurant(int idRes);

    }
}
