using ecommerceProducts.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerceProducts.Models
{
    public class ViewProductViewModel
    {
        public List<Category> Categories { get; set; }
        public Product Product { get; set; }
        public int CartId { get; set; }
    }

}
