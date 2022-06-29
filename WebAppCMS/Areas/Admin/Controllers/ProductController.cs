using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppCMS.Data.Interfaces;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Supervisor")]
    public class ProductController : Controller
    {
        private readonly ICMSRepository _repo;

        public ProductController(ICMSRepository repo)
        {
            _repo = repo;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _repo.GetAllProductsAsync();
            if (categoryId.HasValue)
            {
                products = products.Where(p => categoryId == null || p.CategoryId == categoryId).ToList();
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
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UnitPrice,CategoryId,ImageFile")] Product product)
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
                product.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                if (product.ImageFile != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        product.ImageFile.CopyTo(stream);
                        var file = stream.ToArray();

                        product.Image = file;
                    }
                }

                await _repo.InsertProductAsync(product);
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

            int productId = id.Value;

            var product = await _repo.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await GetCategorySelectList();
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UnitPrice,CategoryId,IsAvailable,ImageFile,CreatedAt")] Product product)
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
                product.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                if (product.ImageFile != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        product.ImageFile.CopyTo(stream);
                        var file = stream.ToArray();

                        product.Image = file;
                    }
                }

                try
                {
                    await _repo.UpdateProductAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await ProductExists(product.Id);
                    if (!exists)
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
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int productId = id.Value;

            var product = await _repo.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            var existingProduct = await _repo.GetProductByIdAsync(id);
            return existingProduct != null;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }

        private async Task<List<SelectListItem>> GetCategorySelectList()
        {
            var categories = await _repo.GetAllCategoriesAsync();
                
            var selectList = categories
                .Select
                (
                    item => new SelectListItem() { Text = item.Name, Value = item.Id.ToString() }
                ).ToList();

            return selectList;
        }
    }
}
