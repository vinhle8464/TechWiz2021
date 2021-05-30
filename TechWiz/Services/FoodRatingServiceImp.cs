using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public class FoodRatingServiceImpl : IFoodRatingService
    {
        public DatabaseContext db;
        public FoodRatingServiceImpl(DatabaseContext databaseContext)
        {
            db = databaseContext;
        }
        public async Task Add(FoodRating foodRating)
        {
            db.FoodRatings.Add(foodRating);
            await db.SaveChangesAsync();
        }

        public async Task<List<UserAndFoodRating>> FindComment(int idFood)
        {
            return await (from user in db.Users
                          join fd in db.FoodRatings on user.Id equals fd.UserId
                          where fd.FoodId == idFood
                          select new UserAndFoodRating { FullName = user.FirstName + user.LastName, Comment = fd.Comment }).ToListAsync();

        }
    }
}
