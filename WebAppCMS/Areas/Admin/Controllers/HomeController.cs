using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data;
using WebAppCMS.Data.Interfaces;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin,Supervisor")]
    public class HomeController : Controller
    {
        private readonly ICMSRepository _repo;

        public HomeController(ICMSRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            // Product
            ViewBag.ProductRecordCount = await _repo.GetProductCountAsync();
            ViewBag.ProductLastModified = await _repo.GetLastModifiedProductAsync();

            // Category
            ViewBag.CategoryRecordCount = await _repo.GetCategoryCountAsync();
            ViewBag.CategoryLastModified = await _repo.GetLastModifiedCategoryAsync();

            // Order
            ViewBag.OrderRecordCount = await _repo.GetOrderCountAsync();
            ViewBag.OrderLastModified = await _repo.GetLastModifiedOrderAsync();

            // User
            ViewBag.UserRecordCount = await _repo.GetAppUserCountAsync();
            ViewBag.UserLastModified = await _repo.GetLastModifiedAppUserAsync();

            return View();
        }
    }
}
