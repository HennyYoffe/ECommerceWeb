using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW58_Ecommerce_April14.Models
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartItem ShoppingCart { get; set; }
        public Product Product { get; set; }
        public decimal Total { get; set; }
    }
}
