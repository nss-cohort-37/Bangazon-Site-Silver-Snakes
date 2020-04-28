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
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .FirstOrDefaultAsync(o => o.PaymentTypeId == null);
            if (order == null)
            {
               return RedirectToAction(nameof(CartEmpty));
            }
            else
            {

            var viewModel = new OrderDetailViewModel();
            viewModel.Order = order;
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
        }

        public ActionResult CartEmpty()
        {
            return View();
        }
        // GET: Orders/Details/5
        public async Task<ActionResult> Details()
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
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var user = await GetCurrentUserAsync();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == id);
            var viewModel = new OrderEditViewModel();
            viewModel.Order = order;
            var paymentTypes = await _context.PaymentType
                .Select(p => new SelectListItem { Text = p.Description, Value = p.PaymentTypeId.ToString() })
                .ToListAsync();
            viewModel.PaymentTypeOptions = paymentTypes;
            viewModel.DateCreated = order.DateCreated;
            return View(viewModel);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, OrderEditViewModel viewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var dataModel = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == id);
                dataModel.DateCreated = viewModel.DateCreated;
                dataModel.PaymentTypeId = viewModel.PaymentTypeId;
                dataModel.UserId = user.Id;
                dataModel.DateCompleted = DateTime.Now;

                _context.Order.Update(dataModel);
                await _context.SaveChangesAsync();

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