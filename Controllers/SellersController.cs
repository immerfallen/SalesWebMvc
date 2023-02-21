using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SalesWebMvcContext _context;
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;


        public SellersController(SalesWebMvcContext context, SellerService sellerService, DepartmentService departmentService)
        {
            _context = context;
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        // GET: Sellers
        public async  Task<IActionResult> Index()
        {
            return View(await _sellerService.FindAll());
        }

        // GET: Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                 return RedirectToAction(nameof(Error), new { message = "Id not provided" }); ;
            }

            var seller = await _context.Seller.Include(s=>s.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }

            return View(seller);
        }

        // GET: Sellers/Create
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAll();
            var formViewModel = new SellerFormViewModel{ Departments = departments};
            return View(formViewModel);
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,BaseSalary,BirthDate, Department, DepartmentId")] Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAll();
                return View(new SellerFormViewModel { 
                Departments = departments,
                Seller = seller
                });
            }
            await _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        // GET: Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });
            }

            var seller =  await _sellerService.FindById(id.Value);
            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" }); ;
            }

            List<Department> departments = await _departmentService.FindAll();

            SellerFormViewModel viewModel = new SellerFormViewModel()
            {
                Departments = departments,
                Seller = seller

            };

            return View(viewModel);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,BaseSalary,BirthDate,Department, DepartmentId")] Seller seller)
        {

            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAll();
                return View(new SellerFormViewModel
                {
                    Departments = departments,
                    Seller = seller
                });
            }

            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _sellerService.Update(seller);
                    await _context.SaveChangesAsync();
                }
                catch (ApplicationException e )
                {
                    return RedirectToAction(nameof(Error), new { message = e.Message });
                }
               
                return RedirectToAction(nameof(Index));
            }
            return View(seller);
        }

        // GET: Sellers/Delete/5
        public async  Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });
            }

            var seller = await _sellerService.FindById(id.Value);

            if (seller == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }
            try
            {
                await _sellerService.Remove(id.Value);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException e )
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seller = await _context.Seller.FindAsync(id);
           _context.Seller.Remove(seller);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerExists(int id)
        {
            return _context.Seller.Any(e => e.Id == id);
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel()
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
