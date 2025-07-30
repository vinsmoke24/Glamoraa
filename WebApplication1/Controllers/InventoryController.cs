using System;
using System.Linq;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/admin/inventory")]
    public class InventoryController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // POST /api/admin/inventory/track-usage
        [HttpPost]
        [Route("track-usage")]
        public IHttpActionResult TrackProductUsage([FromBody] AppointmentServiceProductDto[] appointmentServiceProducts)
        {
            if (appointmentServiceProducts == null || !appointmentServiceProducts.Any())
                return BadRequest("No product data provided for tracking.");

            foreach (var asp in appointmentServiceProducts)
            {
                // Fetch the subservice products used in the appointment
                var subServiceProducts = db.SubServiceProducts
                    .Where(ssp => ssp.SubServiceId == asp.SubServiceId)
                    .ToList();

                foreach (var ssp in subServiceProducts)
                {
                    // Fetch the product being used
                    var product = db.Products.FirstOrDefault(p => p.ProductId == ssp.ProductId);
                    if (product == null)
                        return BadRequest($"Product with ID {ssp.ProductId} not found.");

                    // Ensure there's sufficient stock
                    var productStock = db.ProductStocks
                        .Where(ps => ps.ProductId == ssp.ProductId)
                        .OrderByDescending(ps => ps.UpdatedAt)
                        .FirstOrDefault();

                    if (productStock == null || productStock.Quantity < ssp.QuantityRequired)
                        return BadRequest($"Insufficient stock for product {product.Name}.");

                    // Deduct stock for this product
                    var newStock = new ProductStock
                    {
                        ProductId = ssp.ProductId,
                        Quantity = (int)-ssp.QuantityRequired, // Deduct the quantity used
                        UpdatedAt = DateTime.Now,
                        Operation = "Used", // Mark as used
                        Notes = $"Used in SubServiceId {asp.SubServiceId}, AppointmentId {asp.AppointmentId}"
                    };

                    db.ProductStocks.Add(newStock);
                }
            }

            // Save all changes
            db.SaveChanges();

            return Ok(new { Message = "Product usage tracked successfully." });
        }
    }
}
