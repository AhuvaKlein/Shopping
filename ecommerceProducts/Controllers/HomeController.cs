using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ecommerceProducts.Models;
using Microsoft.Extensions.Configuration;
using ecommerceProducts.data;
using Microsoft.AspNetCore.Http;

namespace ecommerceProducts.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        private int cartId = 0;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");

        }

        public IActionResult Index(int? id)
        {
            IndexViewModel vm = new IndexViewModel();
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            vm.Categories = mgr.GetAllCategories();
            vm.Products = mgr.GetProductsForCategory(id);
            return View(vm);
        }

        public IActionResult ViewProduct(int id)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            ViewProductViewModel vm = new ViewProductViewModel
            {
                Categories = mgr.GetAllCategories(),
                Product = mgr.GetProduct(id)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddToCart(CartItem i)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);

            if (HttpContext.Session.GetString("cart") == null)
            {
                cartId = mgr.AddCart();
                HttpContext.Session.SetString("cart", cartId.ToString());
                i.CartId = cartId;
            }
            else
            {
                i.CartId = int.Parse(HttpContext.Session.GetString("cart"));
            }

            mgr.AddToCart(i);
            return Json(i);
        }

        public IActionResult ShoppingCart()
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            cartId = int.Parse(HttpContext.Session.GetString("cart"));
            CartViewModel vm = new CartViewModel();
            List<Product> products = mgr.GetProductsForCart(cartId);
            IEnumerable<CheckoutItem> c = products.Select(p => new CheckoutItem { Product = p, Quantity = mgr.GetQuantityForProduct(cartId, p.Id) });
            vm.CheckoutItems = c;
            vm.CartId = cartId;
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateCart(CartItem c)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            mgr.UpdateCart(c);
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(CartItem c)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            mgr.DeleteItem(c);
            return Redirect("/home/shoppingcart");
        }

    }
}
