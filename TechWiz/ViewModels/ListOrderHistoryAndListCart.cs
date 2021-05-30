using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;

namespace TechWizProject.ViewModels
{
    public class ListOrderHistoryAndListCart
    {
        public List<CartItem> CartItems { get; set; }
        public List<OrderHistoryViewModel> OrderHistoryViewModels { get; set; }
    }
}
