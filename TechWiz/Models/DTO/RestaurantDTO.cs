using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechWizProject.Models.DTO
{
    public class RestaurantDTO
    {
        public Restaurant Restaurant { get; set; }
        public string Tag { get; set; }
        public List<Category> Categories { get; set; }
        public List<Food> Foods { get; set; }

    }
}
