using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;
using TechWizProject.ViewModels;

namespace TechWizProject.Services
{
    public interface IFoodRatingService
    {
        Task Add(FoodRating foodRating);
        Task<List<UserAndFoodRating>> FindComment(int idFood);
    }
}
