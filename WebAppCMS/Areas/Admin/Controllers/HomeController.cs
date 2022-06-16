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
    [Authorize(Roles="Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Product
            ViewBag.ProductRecordCount = _context.Product.Count();
            ViewBag.ProductLastModified = _context.Product.Include(p => p.ModifiedBy).OrderByDescending(p => p.ModifiedAt).FirstOrDefault();

            // Category 
            ViewBag.CategoryRecordCount = _context.Category.Count();
            ViewBag.CategoryLastModified = _context.Category.Include(c => c.ModifiedBy).OrderByDescending(c => c.ModifiedAt).FirstOrDefault();

            //// Order
            //ViewBag.OrderRecordCount = _context.Order.Count();
            //ViewBag.OrderLastModified = _context.Order.Include(o => o.ModifiedBy).OrderByDescending(o => o.ModifiedAt).FirstOrDefault();

            // User
            ViewBag.UserRecordCount = _context.Users.Count();

            return View();
        }
    }
}
