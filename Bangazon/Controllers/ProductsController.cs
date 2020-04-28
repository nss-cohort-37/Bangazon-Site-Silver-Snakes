using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _userManager = usermanager;
            _context = context;
        }
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        // GET: Products/Details/5
        public async  Task<ActionResult> Details(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
            var viewModel = new ProductDetailViewModel()
            {
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            };
            return View(viewModel);
        }



        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            var productTypes = await _context.ProductType
                .Select(p => new SelectListItem() { Text = p.Label, Value = p.ProductTypeId.ToString()})
                .ToListAsync();
            var viewmodel = new ProductFormViewModel();
            viewmodel.ProductTypeOptions = productTypes;
            return View(viewmodel);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductFormViewModel productViewModel)
        {
            try
            {
                var user = await GetCurrentUserAsync();

                var product = new Product
                {
                    DateCreated = productViewModel.DateCreated,
                    Price = productViewModel.Price,
                    Title = productViewModel.Title,
                    Description = productViewModel.Description,
                    UserId = user.Id,
                    Quantity = productViewModel.Quantity,
                    ProductTypeId = productViewModel.ProductTypeId
                };
             

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = product.ProductId });
            }
            catch (Exception ex)
            {
               var productTypes = await _context.ProductType
                .Select(p => new SelectListItem() { Text = p.Label, Value = p.ProductTypeId.ToString() })
                .ToListAsync();
                productViewModel.ProductTypeOptions = productTypes;
                ViewData["ErrorMessage"] = "Please Select a category.";
                return View(productViewModel);
 
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Products/Edit/5
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

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Products/Delete/5
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