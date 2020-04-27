using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bangazon.Controllers
{
    public class OrderProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderProductsController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _userManager = usermanager;
            _context = context;
        }
        // GET: OrderProducts
        public ActionResult Index()
        {
            return View();
        }

        // GET: OrderProducts/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrderProducts/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: OrderProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var order = new Order
                {
                    UserId = user.Id,
                };
                var orderProduct = new OrderProduct
                {
                    ProductId = product.ProductId,
                    OrderId = order.OrderId
                };
                order.OrderProducts.Add(orderProduct);
                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderProducts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderProducts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderProducts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

    }
}