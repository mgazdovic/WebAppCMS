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
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index()
        {
            var orders = _context.Order.Include(o => o.OrderItems).Include(o => o.ModifiedBy);

            foreach (var order in orders)
            {
                IncludeUserFields(order);

                foreach (var item in order.OrderItems)
                {
                    IncludeProductFields(item);
                }
            }

            return View(await orders.OrderBy(o => o.Id).ToListAsync());
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.Include(o => o.ModifiedBy).Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var item in order.OrderItems)
            {
                IncludeProductFields(item);
            }

            IncludeUserFields(order);
            return View(order);
        }

        // GET: Admin/Order/Create
        public IActionResult Create()
        {
            ViewBag.Users = GetUserSelectList();

            return View();
        }

        // POST: Admin/Order/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,UserId,DeliveryFirstName,DeliveryLastName,DeliveryFullAddress,Message")] Order order)
        {            
            if (ModelState.IsValid)
            {
                order.State = Order.OrderState.New;
                order.CreatedAt = DateTime.Now;
                order.ModifiedAt = DateTime.Now;
                order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = GetUserSelectList();
            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.States = GetOrderStateSelectList();
            ViewBag.Users = GetUserSelectList();
            IncludeUserFields(order);
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,State,PercentDiscount,PercentTax,DeliveryFee,DeliveryFirstName,DeliveryLastName,DeliveryFullAddress,Message,CreatedAt")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            var emptyOrder = (order.OrderItems == null || order.OrderItems.Count == 0);
            var orderInConfirmedState = (order.State == Order.OrderState.Paid || order.State == Order.OrderState.Delivered);
            if (emptyOrder && orderInConfirmedState)
            {
                ModelState.AddModelError("State", $"Empty order (no items) cannot have state '{order.State}'");
            }

            if (order.PercentDiscount < 0 || order.PercentDiscount > 100)
            {
                ModelState.AddModelError("PercentDiscount", "Invalid percentage");
            }

            if (order.PercentTax < 0)
            {
                ModelState.AddModelError("PercentTax", "Tax (%) must be a positive number");
            }

            if (ModelState.IsValid)
            {
                order.ModifiedAt = DateTime.Now;
                order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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

            ViewBag.States = GetOrderStateSelectList();
            ViewBag.Users = GetUserSelectList();
            IncludeUserFields(order);
            return View(order);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.ModifiedBy)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var item in order.OrderItems)
            {
                IncludeProductFields(item);
            }

            IncludeUserFields(order);
            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = _context.Order.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == id);
            foreach (var item in order.OrderItems)
            {
                _context.OrderItem.Remove(item);
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region OrderItem
        // GET
        [Route("Admin/Order/{orderId}/AddItem")]
        public IActionResult AddItem(int orderId)
        {
            ViewBag.Products = GetProductSelectList();
            ViewBag.OrderId = orderId;

            return View();
        }

        // POST
        [HttpPost]
        [Route("Admin/Order/{orderId}/AddItem")]
        public async Task<IActionResult> AddItem([Bind("Id,ProductId,Quantity")] OrderItem orderItem, int orderId)
        {
            if (orderItem.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity has to be positive");
            }

            if (ModelState.IsValid)
            {
                var timestamp = DateTime.Now;
                var userId = GetCurrentUserId();

                orderItem.OrderId = orderId;

                // Find OrderItem with Product
                var existingOrderItem = await _context.OrderItem.FirstOrDefaultAsync(item => item.ProductId == orderItem.ProductId && item.OrderId == orderId);
                if (existingOrderItem == null)
                {
                    // New product
                    orderItem.CreatedAt = timestamp;
                    orderItem.ModifiedAt = timestamp;
                    orderItem.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == userId);
                    _context.Add(orderItem);
                }
                else
                {
                    // Existing product
                    var newQuantity = existingOrderItem.Quantity + orderItem.Quantity;
                    existingOrderItem.Quantity = newQuantity;
                    existingOrderItem.ModifiedAt = timestamp;
                    existingOrderItem.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == userId);
                    _context.Update(existingOrderItem);
                }

                // Order
                var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderId);
                order.ModifiedAt = timestamp;
                order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == userId);
                _context.Update(order);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            ViewBag.Products = GetProductSelectList();
            ViewBag.OrderId = orderId;

            return View(orderItem);
        }

        // GET
        [Route("Admin/OrderItem/Delete")]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.Include(item => item.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            IncludeProductFields(orderItem);

            return View(orderItem);
        }

        // POST: Admin/OrderItem/Delete/5
        [HttpPost]
        [Route("Admin/OrderItem/Delete")]
        public async Task<IActionResult> DeleteItemConfirmed(int id)
        {
            // Remove OrderItem
            var orderItem = await _context.OrderItem.FindAsync(id);
            _context.OrderItem.Remove(orderItem);

            // Update Order Modified fields
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
            order.ModifiedAt = DateTime.Now;
            order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());
            _context.Update(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
        }

        // POST: Admin/OrderItem/AddOne/5
        [HttpPost]
        [Route("Admin/OrderItem/{id}/AddOne")]
        public async Task<IActionResult> AddOne(int id)
        {
            // Update OrderItem
            var orderItem = await _context.OrderItem.FindAsync(id);
            orderItem.Quantity++;
            _context.Update(orderItem);

            // Update Order Modified fields
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
            order.ModifiedAt = DateTime.Now;
            order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());
            _context.Update(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
        }

        // POST: Admin/OrderItem/RemoveOne/5
        [HttpPost]
        [Route("Admin/OrderItem/RemoveOne")]
        public async Task<IActionResult> RemoveOne(int id)
        {
            // OrderItem
            var orderItem = await _context.OrderItem.FindAsync(id);
            var newQuantity = orderItem.Quantity - 1;

            if (newQuantity == 0)
            {
                _context.OrderItem.Remove(orderItem);
            }
            else
            {
                orderItem.Quantity = newQuantity;
                _context.Update(orderItem);
            }

            // Update Order Modified fields
            var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
            order.ModifiedAt = DateTime.Now;
            order.ModifiedBy = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId());
            _context.Update(order);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
        }

        #endregion

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }

        private List<SelectListItem> GetUserSelectList()
        {
            var users = _context.Users
                .Select
                (
                    item => new SelectListItem() { Text = item.UserName, Value = item.Id.ToString() }
                ).ToList();

            return users;
        }

        private List<SelectListItem> GetOrderStateSelectList()
        {
            var states = new List<SelectListItem>();
            foreach (Order.OrderState item in Enum.GetValues(typeof(Order.OrderState)))
            {
                states.Add(new SelectListItem() { Text = item.ToString(), Value = ((int)item).ToString() });
            }

            return states;
        }

        private List<SelectListItem> GetProductSelectList()
        {
            var products = _context.Product
                .Select
                (
                    item => new SelectListItem() { Text = $"{item.Name} (Price: {item.UnitPrice})", Value = item.Id.ToString() }
                ).ToList();

            return products;
        }

        private void IncludeUserFields(Order order)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == order.UserId);
            order.UserName = user.UserName;
        }

        private void IncludeProductFields(OrderItem orderItem)
        {
            var product = _context.Product.FirstOrDefault(p => p.Id == orderItem.ProductId);
            orderItem.ProductName = product.Name;
            orderItem.ProductUnitPrice = product.UnitPrice;
        }
    }
}
