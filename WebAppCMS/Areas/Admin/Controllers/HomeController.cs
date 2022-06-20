using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin,Supervisor")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Product
            ViewBag.ProductRecordCount = await _context.Product.CountAsync();
            ViewBag.ProductLastModified = await _context.Product.Include(p => p.ModifiedBy).OrderByDescending(p => p.ModifiedAt).FirstOrDefaultAsync();

            // Category 
            ViewBag.CategoryRecordCount = await _context.Category.CountAsync();
            ViewBag.CategoryLastModified = await _context.Category.Include(c => c.ModifiedBy).OrderByDescending(c => c.ModifiedAt).FirstOrDefaultAsync();

            // Order
            ViewBag.OrderRecordCount = await _context.Order.CountAsync();
            ViewBag.OrderLastModified = await _context.Order.Include(o => o.ModifiedBy).OrderByDescending(o => o.ModifiedAt).FirstOrDefaultAsync();

            // User
            ViewBag.UserRecordCount = await _context.Users.CountAsync();

            return View();
        }
    }
}
