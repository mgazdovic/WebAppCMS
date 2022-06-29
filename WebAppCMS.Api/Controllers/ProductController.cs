using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCMS.Data.DTOs;
using WebAppCMS.Data.Interfaces;

namespace WebAppCMS.Api.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ICMSRepository _repo;

        public ProductController(ICMSRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
        {
            var products = (await _repo.GetAllProductsAsync());

            if (products == null || products.Count == 0) return NotFound(new { Message = "No products found." });

            var productsDTO = products
                .Select(p => new ProductDTO() {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.CategoryName,
                    UnitPrice = p.UnitPrice,
                    IsAvailable = p.IsAvailable
                });

            return Ok(productsDTO);
        }

        // GET api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product == null) return NotFound();

            var productDTO = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.CategoryName,
                UnitPrice = product.UnitPrice,
                Description = product.Description
            };

            return Ok(productDTO);
        }
    }
}
