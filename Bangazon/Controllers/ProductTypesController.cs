using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductTypesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProductType
        public async Task<ActionResult> Index()
        {
            var viewModel = new ProductTypesViewModel();

            viewModel.Categories = await _context
                .ProductType
                .Select(pt => new ProductCategories()
                {
                    CategoryId = pt.ProductTypeId,
                    CategoryName = pt.Label,
                    ProductCount = pt.Products.Count(),
                    Products = pt.Products.OrderByDescending(p => p.DateCreated).Take(3)
                }).ToListAsync();

            return View(viewModel);
        }








        // GET: ProductType/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
            var viewModel = new ProductDetailViewModel()
            {
                Id = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            };
            return View(viewModel);
        }


        // GET: ProductType/Create
        public ActionResult Create()
        {
            return View();
        }




        // POST: ProductType/Create
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

        // GET: ProductType/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductType/Edit/5
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

        // GET: ProductType/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductType/Delete/5
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
        private async Task<ApplicationUser> GetUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }
}