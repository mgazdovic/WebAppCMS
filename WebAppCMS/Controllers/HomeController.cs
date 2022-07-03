using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data.Interfaces;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICMSRepository _repo;

        public HomeController(ILogger<HomeController> logger, ICMSRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IActionResult> Index(string filter, int? categoryId)
        {
            string filterInput = "";
            if (!String.IsNullOrEmpty(filter))
            {
                filterInput = filter;
            }
            ViewBag.filter = filter;

            if (categoryId.HasValue)
            {
                var category = await _repo.GetCategoryByIdAsync(categoryId.Value);
                ViewBag.CategoryId = category.Id;
                ViewBag.CategoryName = category.Name;
            }

            var products = await _repo.ProductQueryFilterAsync(filterInput, categoryId, 0, 0, false);
            var availableProducts = products.Where(p => p.IsAvailable);

            ViewBag.Categories = await GetCategorySelectList();
            ViewBag.totalCount = await _repo.GetProductCountAsync();
            return View(availableProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
