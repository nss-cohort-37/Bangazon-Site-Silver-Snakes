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
        public async Task<ActionResult> Index(string searchString, string citySearchString)
        {
            if (searchString != null)
            {
                var products = await _context.Product
                    .Where(p => p.Title.Contains(searchString))
                    .Include(p => p.ProductType)
                    .ToListAsync();

                return View(products);
            }
            else if (citySearchString != null)
            {
                var products = await _context.Product
                    .Where(p => p.City == citySearchString)
                    .Include(p => p.ProductType)
                    .ToListAsync();

                return View(products);
            }
            else
            {
                var products = await _context.Product
                    .Include(p => p.ProductType)
                    .ToListAsync();

                return View(products);
            }
        }

        // GET: Products/Details/5
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



        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            var productTypes = await _context.ProductType
                .Select(p => new SelectListItem() { Text = p.Label, Value = p.ProductTypeId.ToString() })
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
                    ProductTypeId = productViewModel.ProductTypeId,
                    City = productViewModel.City
                };

                if (productViewModel.LocalDelivery)
                {
                    product.City = productViewModel.City;
                }

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
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Product.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.ProductId == id);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product product)
        {
            try
            {
                var myProduct = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
                _context.Remove(myProduct);
                await _context.SaveChangesAsync();
                TempData["deleteSuccessful"] = "Your product has been deleted";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["DeleteUnsuccessful"] = "This product is being purchase and cannot be deleted";
                var myProduct = await _context.Product.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.ProductId == id);
                return View(myProduct);
            }
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }
}