using ecommerceProducts.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerceProducts.Models
{
    public class CartViewModel
    {
        public IEnumerable<CheckoutItem> CheckoutItems { get; set; }
        public int CartId { get; set; }
    }
}
