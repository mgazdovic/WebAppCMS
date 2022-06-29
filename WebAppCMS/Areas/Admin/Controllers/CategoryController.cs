﻿using System;
using System.Collections.Generic;
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
    public class CategoryController : Controller
    {
        private readonly ICMSRepository _repo;

        public CategoryController(ICMSRepository repo)
        {
            _repo = repo;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetAllCategoriesAsync());
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                var categories = await _repo.GetAllCategoriesAsync();
                var existingCategory = categories.FirstOrDefault(c => c.Name == category.Name);
                if (existingCategory != null)
                {
                    TempData["Message"] = "Category already exists!";
                }
                else
                {
                    category.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                    await _repo.InsertCategoryAsync(category);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int categoryId = id.Value;

            var category = await _repo.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedAt")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            var categories = await _repo.GetAllCategoriesAsync();
            var existingCategory = categories.FirstOrDefault(c => c.Name == category.Name && c.Id != category.Id);
            if (existingCategory != null)
            {
                ModelState.AddModelError("Name", "Category already exists! Please choose a different name.");
            }

            if (ModelState.IsValid)
            {
                category.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                try
                {
                    await _repo.UpdateCategoryAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await CategoryExists(category.Id);
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
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int categoryId = id.Value;

            var category = await _repo.GetCategoryByIdAsync(categoryId);
                
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            var existingCategory = await _repo.GetCategoryByIdAsync(id);
            return existingCategory != null;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }
    }
}
