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

     

        // POST: OrderProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var userOpenOrder = _context.Order.Where(o => o.UserId == user.Id).FirstOrDefault(o => o.PaymentTypeId == null);
                if (userOpenOrder != null)
                {
                    var newOrderProduct = new OrderProduct
                    {
                        OrderId = userOpenOrder.OrderId,
                        ProductId = id
                    };
                    _context.OrderProduct.Add(newOrderProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Orders");
                }
                else
                {
                    var newOrder = new Order
                    {
                        UserId = user.Id,
                        DateCreated = DateTime.Now
                    };
                    _context.Order.Add(newOrder);
                    var newOrderProduct = new OrderProduct
                    {
                        Order = newOrder,
                        ProductId = id
                    };
                    _context.OrderProduct.Add(newOrderProduct);
                    await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Orders");
                }
            }
            catch(Exception ex)
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

        // POST: OrderProducts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var orderProduct = _context.OrderProduct.FirstOrDefault(op => op.ProductId == id);
                _context.OrderProduct.Remove(orderProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

    }
}