using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.PaymentTypeModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{
    public class PaymentTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentTypesController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager)
        {
            _userManager = usermanager;
            _context = context;
        }
        // GET: PaymentTypes
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var paymentTypes = _context.PaymentType
                .Where(pt => pt.UserId == user.Id);
            return View(paymentTypes);
        }

        // GET: PaymentTypes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymentTypes/Create
        public ActionResult Create()
        {
            var viewModel = new PaymentTypeCreateViewModel();
            return View(viewModel);
        }

        // POST: PaymentTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PaymentTypeCreateViewModel PaymentTypeCreateViewModel)
        {
            try
            {
                var paymentType = new PaymentType
                {
                    Description = PaymentTypeCreateViewModel.Description,
                    AccountNumber = PaymentTypeCreateViewModel.AccountNumber
                };
                var user = await GetCurrentUserAsync();
                paymentType.UserId = user.Id;

                _context.PaymentType.Add(paymentType);
                await _context.SaveChangesAsync();

                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentTypes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymentTypes/Edit/5
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

        // GET: PaymentTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentType = await _context.PaymentType
                .FirstOrDefaultAsync(pt => pt.PaymentTypeId == id);
            if (paymentType == null)
            {
                return NotFound();
            }

            return View(paymentType);

        }

        // POST: PaymentTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var paymentType = await _context.PaymentType.FindAsync(id);
            _context.PaymentType.Remove(paymentType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}