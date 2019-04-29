using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HW58_Ecommerce_April14.Models;
using Microsoft.AspNetCore.Authorization;
using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace HW58_Ecommerce_April14.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _environment;
        private string _connectionString;

        public HomeController(IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            IEnumerable<Category> categories = mgr.GetCategories();
            return View(categories);
        }
        [HttpPost]
        public IActionResult GetProducts(int id)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            IEnumerable<Product> products = mgr.GetProductsForCatId(id);
            return Json(products);
        }
        public IActionResult ShowProduct(int id)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            Product p = mgr.GetProductForId(id);
            return View(p);

        }
        [Authorize]
        public IActionResult AddProduct()
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            IEnumerable<Category> names = mgr.GetCategories();
            return View(names);
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddProduct(Product product, int cid, string category, IFormFile file)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            if (category == null)
            {
                product.CategoryId = cid;
            }
            else
            {
                int id = mgr.AddCategory(category);
                product.CategoryId = id;
            }
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "uploadedfiles", fileName);
            using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
            {
                file.CopyTo(stream);
            }
            product.FileName = fileName;
            User user = mgr.GetByEmail(User.Identity.Name);
            product.UserId = user.Id;
            mgr.AddProduct(product);
            return Redirect("/home/index");
        }
        public IActionResult AddItemtoCart(int productid, int quantity)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            ShoppingCartItem item = new ShoppingCartItem();
            item.ProductId = productid;
            item.Quantity = quantity;
            string id = HttpContext.Session.GetString("id");
            if (id != null)
            {
                item.ShoppingCartId =int.Parse(id);
            }
            else
            {
                int scid = mgr.AddShoppingCart();
                HttpContext.Session.SetString("id", $"{scid}");
                item.ShoppingCartId = scid;
            }
            mgr.AddItemToShoppingCart(item);
            int shoppingcartid = item.ShoppingCartId;
            return Json(shoppingcartid);
        }
        public IActionResult ShowCart()
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);

            string id = HttpContext.Session.GetString("id");
            if (id != null)
            {
                List<ShoppingCartViewModel> vm = new List<ShoppingCartViewModel>();
               IEnumerable<ShoppingCartItem> items =  mgr.GetShoppingCartItemsForShoppingCartId(int.Parse(id));
                foreach(ShoppingCartItem s in items)
                {
                    Product p = mgr.GetProductForId(s.ProductId);
                    vm.Add(new ShoppingCartViewModel
                    {
                        ShoppingCart = s,
                        Product = p, 
                        Total = s.Quantity * p.Amount,
                    });
                   
                }
                return View(vm);
            }
            else
            {
                return Redirect("/home/index");
            }
          
        }
        public IActionResult EditQuantity (int scid, int pid,int quantity)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);

            return Redirect("/home/showcart");
        }
        public IActionResult DeleteItem(int scid, int pid)
        {
            ECommerceManager mgr = new ECommerceManager(_connectionString);
            mgr.DeleteItemFromShoppingCart(scid, pid);
            return Redirect("/home/showcart");

        }
    }
}
