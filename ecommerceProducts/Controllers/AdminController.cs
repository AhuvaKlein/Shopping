using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ecommerceProducts.data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ecommerceProducts.Controllers
{
    public class AdminController : Controller
    {
        private string _connectionString;
        private IHostingEnvironment _environment;

        public AdminController(IConfiguration configuration, IHostingEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _environment = environment;
        }


        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AddProduct()
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            return View(mgr.GetAllCategories());
        }

        [HttpPost]
        public IActionResult SubmitProduct(IFormFile ProductImage, Product p)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ProductImage.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "ProductImages", fileName);
            p.Image = fileName;
            using (Stream stream = new FileStream(fullPath, FileMode.CreateNew))
            {
                ProductImage.CopyTo(stream);
            }
            mgr.AddProduct(p);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitCategory(Category c)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            mgr.AddCategory(c);
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            User u = mgr.GetUserByEmail(user);
            if (u == null)
            {
                return Redirect("/admin/login");
            }

            else if (!mgr.Match(user.Password, u.Password))
            {
                return Redirect("/admin/login");
            }

            var claims = new List<Claim>
                {
                    new Claim("user", user.Email)
                };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();


            return RedirectToAction("index");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            ShoppingManager mgr = new ShoppingManager(_connectionString);
            mgr.AddUser(user);
            return Redirect("/admin/login");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/admin/login");
        }
    }
}