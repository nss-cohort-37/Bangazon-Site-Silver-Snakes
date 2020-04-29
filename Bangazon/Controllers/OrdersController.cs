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
           var orderProducts = await _context.OrderProduct.Where(op => op.OrderId == order.OrderId).ToListAsync();
            if (order == null || orderProducts.Count < 1)
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
            var paymentTypes = await _context.PaymentType.Where(p => p.UserId == user.Id)
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

                TempData["orderConfirmed"] = "Your order has been processed. Thanks for shopping.";
                return RedirectToAction("Index", "Products");
            }
            catch
            {
                return View();
            }
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete()
        {
            try
            {
                var user =  await GetCurrentUserAsync();
                var order = await _context.Order
                .Where(o => o.UserId == user.Id).FirstOrDefaultAsync(o => o.PaymentType == null);

                DeleteOrderProducts(order.OrderId);
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();

                // TODO: Add delete logic here
                TempData["cancelOrder"] = "Your order has been canceled.";
                return RedirectToAction("Index", "Products");
            }
            catch(Exception ex)
            {
                return View();
            }
        }   
        private async void DeleteOrderProducts(int orderId)
        {
            var orderProducts = await _context.OrderProduct.Where(op => op.OrderId == orderId).ToListAsync();
            foreach(var op in orderProducts)
            {
                _context.OrderProduct.Remove(op);
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }
}