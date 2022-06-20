using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppCMS.Data;
using WebAppCMS.Models;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _context.Product.Include(p => p.ModifiedBy)
                .Where(p => categoryId == null || p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();
            foreach (var product in products)
            {
                await IncludeCategoryFields(product);
            }

            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await GetCategorySelectList();
            return View(products);
        }

        // GET: Admin/Product/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetCategorySelectList();
            
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UnitPrice,CategoryId")] Product product)
        {
            if(product.UnitPrice <= 0)
            {
                ModelState.AddModelError("UnitPrice", "Price must be greater than 0.");
            }
            
            if (ModelState.IsValid)
            {
                product.IsAvailable = true;
                product.CreatedAt = DateTime.Now;
                product.ModifiedAt = DateTime.Now;
                product.ModifiedBy = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetCurrentUserId());

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await GetCategorySelectList();
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await GetCategorySelectList();
            await IncludeCategoryFields(product);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UnitPrice,CategoryId,IsAvailable,CreatedAt")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (product.UnitPrice <= 0)
            {
                ModelState.AddModelError("UnitPrice", "Price must be greater than 0.");
            }

            if (ModelState.IsValid)
            {
                product.ModifiedAt = DateTime.Now;
                product.ModifiedBy = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetCurrentUserId());

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await GetCategorySelectList();
            await IncludeCategoryFields(product);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ModifiedBy)
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            await IncludeCategoryFields(product);
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }

        private async Task<List<SelectListItem>> GetCategorySelectList()
        {
            var categories = _context.Category
                .Select
                (
                    item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }
                ).ToListAsync();

            return await categories;
        }

        private async Task IncludeCategoryFields(Product product)
        {
            var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            product.CategoryName = category.Name;
        }
    }
}
