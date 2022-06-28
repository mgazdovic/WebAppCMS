using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data.Interfaces;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Data.Repositories
{
    public class CMSRepository : ICMSRepository
    {
        private readonly ApplicationDbContext _context;

        public CMSRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Category
                .Include(c => c.ModifiedBy)
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var categories = await GetAllCategoriesAsync();
            return categories.FirstOrDefault(m => m.Id == id);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var toUpdate = await GetCategoryByIdAsync(category.Id);

            if (toUpdate != null)
            {
                toUpdate.Name = category.Name;
                toUpdate.ModifiedBy = category.ModifiedBy;
                toUpdate.ModifiedAt = DateTime.Now;
                _context.Update(toUpdate);
                await _context.SaveChangesAsync();
            }

            return toUpdate;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var toDelete = await GetCategoryByIdAsync(id);
            if (toDelete != null)
            {
                _context.Category.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Category> InsertCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.Now;
            category.ModifiedAt = DateTime.Now;

            var added = _context.Category.Add(category).Entity;
            await _context.SaveChangesAsync();
            return added;
        }

        public async Task<List<Category>> CategoryQueryFilterAsync(string s, string orderBy, int perPage)
        {
            var filter = await GetAllCategoriesAsync();

            if (!string.IsNullOrEmpty(s))
            {
                filter = filter
                    .Where(c => c.Name.Contains(s, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            filter = orderBy == "desc" ? filter.OrderByDescending(c => c.Id).ToList() : filter.OrderBy(c => c.Id).ToList();

            if (perPage > 0)
            {
                filter = filter.Take(perPage).ToList();
            }

            return filter;
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.ModifiedBy).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.Include(u => u.ModifiedBy).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = from prod in _context.Product.Include(p => p.OrderItems).Include(p => p.ModifiedBy)
                           join cat in _context.Category on prod.CategoryId equals cat.Id
                           select new Product()
                           {
                               Id = prod.Id,
                               CategoryId = prod.CategoryId,
                               CategoryName = cat.Name,
                               Name = prod.Name,
                               Description = prod.Description,
                               UnitPrice = prod.UnitPrice,
                               IsAvailable = prod.IsAvailable,
                               OrderItems = prod.OrderItems,
                               CreatedAt = prod.CreatedAt,
                               ModifiedAt = prod.ModifiedAt,
                               ModifiedBy = prod.ModifiedBy
                           };
            
            return await products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var products = await GetAllProductsAsync();
            return products.FirstOrDefault(m => m.Id == id);
        }

        public async Task<Product> InsertProductAsync(Product product)
        {
            product.CreatedAt = DateTime.Now;
            product.ModifiedAt = DateTime.Now;

            var added = _context.Product.Add(product).Entity;
            await _context.SaveChangesAsync();
            return added;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var toUpdate = await GetProductByIdAsync(product.Id);

            if (toUpdate != null)
            {
                toUpdate.Name = product.Name;
                toUpdate.CategoryId = product.CategoryId;
                toUpdate.Description = product.Description;
                toUpdate.IsAvailable = product.IsAvailable;
                toUpdate.UnitPrice = product.UnitPrice;

                toUpdate.ModifiedBy = product.ModifiedBy;
                toUpdate.ModifiedAt = DateTime.Now;

                _context.Update(toUpdate);
                await _context.SaveChangesAsync();
            }

            return toUpdate;
        }

        public async Task DeleteProductAsync(int id)
        {
            var toDelete = await GetProductByIdAsync(id);
            if (toDelete != null)
            {
                _context.Product.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> ProductQueryFilterAsync(string s, string orderBy, int perPage)
        {
            {
                var filter = await GetAllProductsAsync();

                if (!string.IsNullOrEmpty(s))
                {
                    filter = filter
                        .Where(p => p.Name.Contains(s, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
                }

                filter = orderBy == "desc" ? filter.OrderByDescending(c => c.Id).ToList() : filter.OrderBy(c => c.Id).ToList();

                if (perPage > 0)
                {
                    filter = filter.Take(perPage).ToList();
                }

                return filter;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = _context.Order
                .Include(o => o.ModifiedBy)
                .Include(o => o.OrderItems)
                .ThenInclude(item => item.ModifiedBy);

            foreach (var order in await orders.ToListAsync())
            {
                var user = await GetUserByIdAsync(order.UserId);
                order.UserName = user.UserName;
                
                foreach (var item in order.OrderItems)
                {
                    var product = await GetProductByIdAsync(item.ProductId);
                    item.ProductName = product.Name;
                    item.ProductUnitPrice = product.UnitPrice;
                }
            }

            return await orders.ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var orders = await GetAllOrdersAsync();
            return orders.FirstOrDefault(o => o.Id == id);
        }

        public async Task<Order> InsertOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.ModifiedAt = DateTime.Now;

            var added = _context.Order.Add(order).Entity;
            await _context.SaveChangesAsync();
            return added;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var toUpdate = await GetOrderByIdAsync(order.Id); 

            if (toUpdate != null)
            {
                toUpdate.UserId = order.UserId;
                toUpdate.State = order.State;
                toUpdate.PercentDiscount = order.PercentDiscount;
                toUpdate.PercentTax = order.PercentTax;
                toUpdate.DeliveryFee = order.DeliveryFee;
                toUpdate.DeliveryFirstName = order.DeliveryFirstName;
                toUpdate.DeliveryLastName = order.DeliveryLastName;
                toUpdate.DeliveryFullAddress = order.DeliveryFullAddress;
                toUpdate.Message = order.Message;
                toUpdate.DeliveryFullAddress = order.DeliveryFullAddress;

                toUpdate.ModifiedBy = order.ModifiedBy;
                toUpdate.ModifiedAt = DateTime.Now;

                _context.Update(toUpdate);
                await _context.SaveChangesAsync();
            }

            return toUpdate;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var toDelete = await GetOrderByIdAsync(id);
            if (toDelete != null)
            {
                _context.Order.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> OrderQueryFilterAsync(string s, string orderBy, int perPage)
        {
            {
                var filter = await GetAllOrdersAsync();

                if (!string.IsNullOrEmpty(s))
                {
                    filter = filter
                        .Where(o => o.UserName.Contains(s, StringComparison.CurrentCultureIgnoreCase))
                        .ToList();
                }

                filter = orderBy == "desc" ? filter.OrderByDescending(c => c.Id).ToList() : filter.OrderBy(c => c.Id).ToList();

                if (perPage > 0)
                {
                    filter = filter.Take(perPage).ToList();
                }

                return filter;
            }
        }

        public async Task<List<OrderItem>> GetAllOrderItemsAsync(int orderId)
        {
            var orderItems = from items in _context.OrderItem.Include(o => o.ModifiedBy)
                                join prod in _context.Product on items.ProductId equals prod.Id
                             where items.OrderId == orderId
                                select new OrderItem()
                                {
                                    Id = items.Id,
                                    OrderId = items.OrderId,
                                    ProductId = items.ProductId,
                                    ProductName = prod.Name,
                                    ProductUnitPrice = prod.UnitPrice,
                                    Quantity = items.Quantity,
                                    CreatedAt = items.CreatedAt,
                                    ModifiedAt = items.ModifiedAt,
                                    ModifiedBy = items.ModifiedBy,
                                };

            return await orderItems.ToListAsync();
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _context.OrderItem.Include(o => o.ModifiedBy).FirstOrDefaultAsync(item => item.Id == id);
            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);
            orderItem.ProductName = product.Name;
            orderItem.ProductUnitPrice = product.UnitPrice;

            return orderItem;
        }

        public async Task<OrderItem> InsertOrderItemAsync(OrderItem orderItem)
        {
            orderItem.CreatedAt = DateTime.Now;
            orderItem.ModifiedAt = DateTime.Now;

            var added = _context.OrderItem.Add(orderItem).Entity;
            await _context.SaveChangesAsync();
            return added;
        }

        public async Task<OrderItem> UpdateOrderItemAsync(OrderItem orderItem)
        {
            var toUpdate = await GetOrderItemByIdAsync(orderItem.Id);

            if (toUpdate != null)
            {
                toUpdate.OrderId = orderItem.OrderId;
                toUpdate.ProductId = orderItem.ProductId;
                toUpdate.ProductName = orderItem.ProductName;
                toUpdate.ProductUnitPrice = orderItem.ProductUnitPrice;
                toUpdate.Quantity = orderItem.Quantity;

                toUpdate.ModifiedBy = orderItem.ModifiedBy;
                toUpdate.ModifiedAt = DateTime.Now;

                _context.Update(toUpdate);
                await _context.SaveChangesAsync();
            }

            return toUpdate;
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var toDelete = await GetOrderItemByIdAsync(id);
            if (toDelete != null)
            {
                _context.OrderItem.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
