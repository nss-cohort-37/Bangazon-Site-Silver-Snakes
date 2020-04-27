using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
    {
        _userManager = usermanager;
        _context = context;
    }
        // GET: Orders
        public async Task<ActionResult> Index()
        { 
            var user = await GetCurrentUserAsync();
            var order = await _context.Order
                .Where(o => o.UserId == user.Id)
                .Include(u => user.PaymentTypes)
                .Include(op => op.OrderProducts)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.PaymentType == null);
            var viewModel = new OrderDetailViewModel();
            var lineItems = order.OrderProducts.Select(op => new OrderLineItem()
            {
                Product = op.Product,
                Units = op.Product.Quantity,
                Cost = op.Product.Price,
            });
            double totalCost = 0;
            foreach(var item in lineItems)
            {
                totalCost += item.Cost;
            }
            viewModel.LineItems = lineItems;
            viewModel.Total = totalCost;
            return View(viewModel);
        }

   

        // GET: Orders/Details/5
        public async  Task<ActionResult> Details()
        {
            
            return View();
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Orders/Edit/5
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

        // GET: Orders/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Orders/Delete/5
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