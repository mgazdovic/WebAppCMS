using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data.Models;

namespace WebAppCMS.Data.Interfaces
{
    public interface ICMSRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();

        Task<Category> GetCategoryByIdAsync(int id);

        Task<Category> InsertCategoryAsync(Category category);

        Task<Category> UpdateCategoryAsync(Category category);

        Task DeleteCategoryAsync(int id);

        Task<List<Category>> CategoryQueryFilterAsync(string s, string orderBy, int perPage);

        Task<int> GetCategoryCountAsync();

        Task<Category> GetLastModifiedCategoryAsync();

        Task<List<AppUser>> GetAllUsersAsync();

        Task<AppUser> GetUserByIdAsync(string id);

        Task<int> GetAppUserCountAsync();

        Task<AppUser> GetLastModifiedAppUserAsync();

        Task<List<Product>> GetAllProductsAsync();

        Task<Product> GetProductByIdAsync(int id);

        Task<Product> InsertProductAsync(Product product);

        Task<Product> UpdateProductAsync(Product product);

        Task DeleteProductAsync(int id);

        Task<List<Product>> ProductQueryFilterAsync(string s, string orderBy, int perPage);

        Task<int> GetProductCountAsync();

        Task<Product> GetLastModifiedProductAsync();

        Task<List<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(int id);

        Task<Order> InsertOrderAsync(Order order);

        Task<Order> UpdateOrderAsync(Order order);

        Task DeleteOrderAsync(int id);

        Task<List<Order>> OrderQueryFilterAsync(string s, string orderBy, int perPage);

        Task<List<OrderItem>> GetAllOrderItemsAsync(int orderId);

        Task<OrderItem> GetOrderItemByIdAsync(int id);

        Task<OrderItem> InsertOrderItemAsync(OrderItem orderItem);

        Task<OrderItem> UpdateOrderItemAsync(OrderItem orderItem);

        Task DeleteOrderItemAsync(int id);

        Task<int> GetOrderCountAsync();

        Task<Order> GetLastModifiedOrderAsync();

    }
}
