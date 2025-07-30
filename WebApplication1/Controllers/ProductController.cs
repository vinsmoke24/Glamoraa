using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/admin/products")]
    public class ProductController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET /api/admin/products
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllProducts()
        {
            var products = db.Products.Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Category,
                p.Type,
                p.Unit,
                p.Price
            }).ToList();

            return Ok(products);
        }

        // POST /api/admin/products
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddProduct([FromBody] ProductDto productDto)
        {
            if (productDto == null)
                return BadRequest("Product data cannot be empty.");

            var newProduct = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = productDto.Name,
                Category = productDto.Category,
                Type = productDto.Type,
                Unit = productDto.Unit,
                Price = productDto.Price,
                SalonId = new Guid("00000000-0000-0000-0000-000000000021")  // you should ideally fetch current salonId
            };

            db.Products.Add(newProduct);
            db.SaveChanges();

            var initialStock = new ProductStock
            {
                ProductId = newProduct.ProductId,
                Quantity = 100,
                UpdatedAt = DateTime.Now,
                Operation = "Added",
                Notes = "Initial stock added"
            };

            db.ProductStocks.Add(initialStock);
            db.SaveChanges();

            return Ok(new { Message = "Product added successfully." });
        }

        // PUT /api/admin/products/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateProduct(Guid id, [FromBody] ProductDto productDto)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                return NotFound();

            product.Name = productDto.Name;
            product.Category = productDto.Category;
            product.Type = productDto.Type;
            product.Unit = productDto.Unit;
            product.Price = productDto.Price;

            db.SaveChanges();

            return Ok(new { Message = "Product updated successfully." });
        }

        #region Product Stock Management

        // POST /api/admin/products/{productId}/stock
        [HttpPost]
        [Route("{productId}/stock")]
        public IHttpActionResult ManageStock(Guid productId, [FromBody] ProductStockDto productStockDto)
        {
            if (productStockDto == null)
                return BadRequest("Stock data cannot be empty.");

            var stockOperation = new ProductStock
            {
                ProductId = productId,
                Quantity = productStockDto.Quantity,
                Operation = productStockDto.Operation,
                UpdatedAt = DateTime.Now,
                Notes = productStockDto.Notes
            };

            db.ProductStocks.Add(stockOperation);
            db.SaveChanges();

            return Ok(new { Message = "Stock operation completed successfully." });
        }

        #endregion
    }
}
