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
            var products = await _repo.GetAllProductsAsync();
            ViewBag.ProductRecordCount = products.Count();
            ViewBag.ProductLastModified = products.OrderByDescending(p => p.ModifiedAt).FirstOrDefault();

            var categories = await _repo.GetAllCategoriesAsync();
            ViewBag.CategoryRecordCount = categories.Count();
            ViewBag.CategoryLastModified = categories.OrderByDescending(c => c.ModifiedAt).FirstOrDefault();

            var orders = await _repo.GetAllOrdersAsync();
            ViewBag.OrderRecordCount = orders.Count();
            ViewBag.OrderLastModified = orders.OrderByDescending(o => o.ModifiedAt).FirstOrDefault();

            var users = await _repo.GetAllUsersAsync();
            ViewBag.UserRecordCount = users.Count();
            ViewBag.UserLastModified = users.OrderByDescending(o => o.ModifiedAt).FirstOrDefault();

            return View();
        }
    }
}
