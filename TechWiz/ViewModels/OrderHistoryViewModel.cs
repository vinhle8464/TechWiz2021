using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechWizProject.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public int OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string NameRestaurant { get; set; }
        public string State { get; set; }
        public double Total { get; set; }
        public string Image { get; set; }
        public int CountOrder { get; set; }
        public DateTime Time { get; set; }


    }
}
