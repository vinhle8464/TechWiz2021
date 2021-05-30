using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly DatabaseContext db;
        public RestaurantService(DatabaseContext _db)
        {
            db = _db;
        }
        public async Task<UserAndRestauants> FindCmt(int idRes)
        {
            var a = await (from user in db.Users
                           join res in db.RestaurantRatings on user.Id equals res.UserId
                           where idRes == res.RestaurantId
                           orderby res.CreatedAt
                           select new UserAndRestauants
                           {
                               FullName = user.FirstName + " " + user.LastName,
                               Comment = res.Comment,
                               CreateAt = res.CreatedAt,
                               Photo = user.Avatar
                           }).FirstOrDefaultAsync();
            return a;
        }

        public async Task<List<FoodAndCate>> FindFood(int idRes)
        {
            return await (from f in db.Foods
                          join cate in db.Categories on f.CategoryId equals cate.Id
                          where f.RestaurantId == idRes
                          select new FoodAndCate
                          {
                              FoodId = f.Id,
                              FoodName = f.Name,
                              CateName = cate.Name,
                              Desc = f.Description,
                              Price = f.Price,
                              Photo = f.Images
                          }
                          ).ToListAsync();
        }

        public async Task<Restaurant> GetInfoRestaurant(int idRes)
        {
            return await db.Restaurants.FindAsync(idRes);
        }

        public async Task<List<Restaurant>> LoadAllRestaurant()
        {
            return await db.Restaurants.ToListAsync();
        }

        public async Task<List<Restaurant>> LoadRestaurant(int numberOfRestaurant)
        {
            return await db.Restaurants.Take(numberOfRestaurant).ToListAsync();
        }
    }
}
