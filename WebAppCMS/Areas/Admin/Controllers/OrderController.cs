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
using WebAppCMS.Data.Interfaces;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Supervisor")]
    public class OrderController : Controller
    {
        private readonly ICMSRepository _repo;

        public OrderController(ICMSRepository repo)
        {
            _repo = repo;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(string filter, int? page, int? perPage)
        {
            int pageInput = 1;
            if (page.HasValue)
            {
                pageInput = page.Value;
            }
            ViewBag.page = pageInput;

            string filterInput = "";
            if (!String.IsNullOrEmpty(filter))
            {
                filterInput = filter;
            }
            ViewBag.filter = filter;

            int perPageInput = 1000;
            if (perPage.HasValue && perPage > 0)
            {
                perPageInput = perPage.Value;
            }
            ViewBag.perPage = perPage;

            var records = await _repo.OrderQueryFilterAsync(filterInput, perPageInput, pageInput, false);
            if (records != null)
            {
                var allRecords = await _repo.OrderQueryFilterAsync(filterInput, 0, 0, false);
                if (allRecords != null)
                {
                    int recordCount = allRecords.Count;

                    int totalPageCount = recordCount / perPageInput;
                    if (recordCount % perPageInput > 0)
                    {
                        totalPageCount++;
                    }

                    ViewBag.totalPageCount = totalPageCount;
                }
            }

            ViewBag.totalCount = await _repo.GetOrderCountAsync();
            return View(records.OrderBy(o => o.Id));
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int orderId = id.Value;
            var order = await _repo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Order/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await GetUserSelectList();

            return View();
        }

        // POST: Admin/Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DeliveryFirstName,DeliveryLastName,DeliveryFullAddress,Message")] Order order)
        {            
            if (ModelState.IsValid)
            {
                order.State = Order.OrderState.New;
                order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                await _repo.InsertOrderAsync(order);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await GetUserSelectList();
            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int orderId = id.Value;
            var order = await _repo.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.States = GetOrderStateSelectList();
            ViewBag.Users = await GetUserSelectList();
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                try
                {
                    await _repo.UpdateOrderAsync(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await OrderExists(order.Id);
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

            ViewBag.States = GetOrderStateSelectList();
            ViewBag.Users = await GetUserSelectList();
            return View(order);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int orderId = id.Value;

            var order = await _repo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        #region OrderItem
        // GET
        [Route("Admin/Order/{orderId}/AddItem")]
        public async Task<IActionResult> AddItem(int orderId)
        {
            ViewBag.Products = await GetProductSelectList();
            ViewBag.OrderId = orderId;

            return View();
        }

        // POST
        [HttpPost]
        [Route("Admin/Order/{orderId}/AddItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("Id,ProductId,Quantity")] OrderItem orderItem, int orderId)
        {
            if (orderItem.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity has to be positive");
            }

            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();

                orderItem.OrderId = orderId;

                // Find OrderItem with Product
                var existingOrderItem = (await _repo.GetAllOrderItemsAsync(orderId)).FirstOrDefault(item => item.ProductId == orderItem.ProductId);
                
                if (existingOrderItem == null)
                {
                    // New product
                    orderItem.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                    await _repo.InsertOrderItemAsync(orderItem);
                }
                else
                {
                    // Existing product
                    var newQuantity = existingOrderItem.Quantity + orderItem.Quantity;
                    existingOrderItem.Quantity = newQuantity;
                    existingOrderItem.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

                    await _repo.UpdateOrderItemAsync(existingOrderItem);
                }

                // Order
                var order = await _repo.GetOrderByIdAsync(orderId);
                order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());
                await _repo.UpdateOrderAsync(order);

                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            ViewBag.Products = await GetProductSelectList();
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

            int orderItemId = id.Value;
            var orderItem = await _repo.GetOrderItemByIdAsync(orderItemId);

            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: Admin/OrderItem/Delete/5
        [HttpPost]
        [Route("Admin/OrderItem/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItemConfirmed(int id)
        {
            var orderItem = await _repo.GetOrderItemByIdAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            // Remove OrderItem
            await _repo.DeleteOrderItemAsync(id);

            // Update Order Modified fields
            var order = await _repo.GetOrderByIdAsync(orderItem.OrderId);
            order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());

            await _repo.UpdateOrderAsync(order);

            return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
        }

        // POST: Admin/OrderItem/AddOne/5
        [HttpPost]
        [Route("Admin/OrderItem/{id}/AddOne")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOne(int id)
        {
            // Update OrderItem
            var orderItem = await _repo.GetOrderItemByIdAsync(id);
            orderItem.Quantity++;
            await _repo.UpdateOrderItemAsync(orderItem);

            // Update Order Modified fields
            var order = await _repo.GetOrderByIdAsync(orderItem.OrderId);
            order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());
            await _repo.UpdateOrderAsync(order);

            return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
        }

        // POST: Admin/OrderItem/RemoveOne/5
        [HttpPost]
        [Route("Admin/OrderItem/RemoveOne")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveOne(int id)
        {
            // OrderItem
            var orderItem = await _repo.GetOrderItemByIdAsync(id);
            var newQuantity = orderItem.Quantity - 1;

            if (newQuantity == 0)
            {
                return RedirectToAction(nameof(DeleteItem), new { id = orderItem.Id });
            }
            else
            {
                orderItem.Quantity = newQuantity;
                await _repo.UpdateOrderItemAsync(orderItem);

                // Update Order Modified fields
                var order = await _repo.GetOrderByIdAsync(orderItem.OrderId);
                order.ModifiedBy = await _repo.GetUserByIdAsync(GetCurrentUserId());
                await _repo.UpdateOrderAsync(order);

                return RedirectToAction(nameof(Details), new { id = orderItem.OrderId });
            }
        }

        #endregion

        private async Task<bool> OrderExists(int id)
        {
            var existingOrder = await _repo.GetOrderByIdAsync(id);
            return existingOrder != null;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
        }

        private async Task<List<SelectListItem>> GetUserSelectList()
        {
            var users = (await _repo.GetAllUsersAsync())
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

        private async Task<List<SelectListItem>> GetProductSelectList()
        {
            var products = (await _repo.GetAllProductsAsync())
                .Select
                (
                    item => new SelectListItem() { Text = $"{item.Name} (Price: {item.UnitPrice})", Value = item.Id.ToString() }
                ).ToList();

            return products;
        }
    }
}
