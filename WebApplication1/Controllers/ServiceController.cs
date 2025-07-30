using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using System.Data.Entity;
using WebApplication1.Models.Dtos;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/admin/services")]
    public class ServiceController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET /api/admin/services
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllServices()
        {
            var services = db.Services.Select(s => new
            {
                s.ServiceId,
                s.ServiceName,
                s.Description,
                s.Status,
                SubServices = db.SubServices.Where(sub => sub.ServiceId == s.ServiceId).Select(sub => new
                {
                    sub.SubServiceId,
                    sub.SubServiceName,
                    sub.Price,
                    sub.DurationMinutes,
                    sub.Description,
                    sub.Status
                }).ToList()
            }).ToList();

            return Ok(services);
        }

        // POST /api/admin/services
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddService([FromBody] ServiceDto serviceDto)
        {
            if (serviceDto == null)
                return BadRequest("Service data cannot be empty.");


            Guid currentSaloonId = new Guid("00000000-0000-0000-0000-000000000021");

            var CurrentSaloonId = currentSaloonId;  // NEW - fetch current salon

            

            var newService = new Service
            {
                ServiceId = Guid.NewGuid(),
                ServiceName = serviceDto.ServiceName,
                Description = serviceDto.Description,
                Status = serviceDto.Status,
                SalonId= CurrentSaloonId
            };

            db.Services.Add(newService);
            db.SaveChanges();

            return Ok(new
            {
                Message = "Service added successfully.",
                ServiceId = newService.ServiceId
            });
        }

        // PUT /api/admin/services/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateService(Guid id, [FromBody] ServiceDto serviceDto)
        {
            var service = db.Services.FirstOrDefault(s => s.ServiceId == id);
            if (service == null)
                return NotFound();

            service.ServiceName = serviceDto.ServiceName;
            service.Description = serviceDto.Description;
            service.Status = serviceDto.Status;

            db.SaveChanges();

            return Ok(new { Message = "Service updated successfully." });
        }

        // DELETE /api/admin/services/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteService(Guid id)
        {
            var service = db.Services.FirstOrDefault(s => s.ServiceId == id);
            if (service == null)
                return NotFound();

            service.Status = false;

            db.SaveChanges();

            return Ok(new { Message = "Service deactivated successfully." });
        }

        #region SubService CRUD Operations

        // GET /api/admin/services/{serviceId}/subservices
        [HttpGet]
        [Route("{serviceId}/subservices")]
        public IHttpActionResult GetSubServices(Guid serviceId)
        {
            var subServices = db.SubServices.Where(sub => sub.ServiceId == serviceId).Select(sub => new
            {
                sub.SubServiceId,
                sub.SubServiceName,
                sub.Price,
                sub.DurationMinutes,
                sub.Description,
                sub.Status
            }).ToList();

            return Ok(subServices);
        }

        // POST /api/admin/services/{serviceId}/subservices
        [HttpPost]
        [Route("{serviceId}/subservices")]
        public IHttpActionResult AddSubService(Guid serviceId, [FromBody] SubServiceDto subServiceDto)
        {
            if (subServiceDto == null)
                return BadRequest("SubService data cannot be empty.");

            // Ensure that the service exists
            var service = db.Services.FirstOrDefault(s => s.ServiceId == serviceId);
            if (service == null)
                return NotFound();

            bool? status = null;
            if (!string.IsNullOrEmpty(subServiceDto.Status))
            {
                status = subServiceDto.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            }

            var newSubService = new SubService
            {
                ServiceId = serviceId,
                SubServiceName = subServiceDto.SubServiceName,
                Price = subServiceDto.Price,
                DurationMinutes = subServiceDto.DurationMinutes,
                Description = subServiceDto.Description,
                Status = status
            };

            db.SubServices.Add(newSubService);
            db.SaveChanges();

            // Return the created subservice with its details
            return Ok(new
            {
                Message = "SubService added successfully.",
                SubServiceId = newSubService.SubServiceId,
                SubServiceName = newSubService.SubServiceName,
                Price = newSubService.Price,
                DurationMinutes = newSubService.DurationMinutes,
                Status = (bool)newSubService.Status ? "Active" : "Inactive"
            });
        }


        // PUT /api/admin/services/{serviceId}/subservices/{subServiceId}
        [HttpPut]
        [Route("{serviceId}/subservices/{subServiceId}")]
        public IHttpActionResult UpdateSubService(Guid serviceId, int subServiceId, [FromBody] SubServiceDto subServiceDto)
        {
            var subService = db.SubServices.FirstOrDefault(sub => sub.SubServiceId == subServiceId && sub.ServiceId == serviceId);
            if (subService == null)
                return NotFound();

            bool? status = null;
            if (!string.IsNullOrEmpty(subServiceDto.Status))
            {
                status = subServiceDto.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            }

            subService.SubServiceName = subServiceDto.SubServiceName;
            subService.Price = subServiceDto.Price;
            subService.DurationMinutes = subServiceDto.DurationMinutes;
            subService.Description = subServiceDto.Description;
            subService.Status = status;

            db.SaveChanges();

            return Ok(new { Message = "SubService updated successfully." });
        }

        // DELETE /api/admin/services/{serviceId}/subservices/{subServiceId}
        [HttpDelete]
        [Route("{serviceId}/subservices/{subServiceId}")]
        public IHttpActionResult DeleteSubService(Guid serviceId, int subServiceId)
        {
            var subService = db.SubServices.FirstOrDefault(sub => sub.SubServiceId == subServiceId && sub.ServiceId == serviceId);
            if (subService == null)
                return NotFound();

            try
            {
                db.SubServices.Remove(subService);
                db.SaveChanges();
                return Ok(new { Message = "SubService deleted from database." });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Deletion failed. SubService might be linked to other records. " + ex.Message));
            }
        }

        // POST /api/admin/subservices/{subServiceId}/products
        [HttpPost]
        [Route("{subServiceId}/products")]
        public IHttpActionResult AssignProductsToSubService(int subServiceId, [FromBody] SubServiceProductDto[] products)
        {
            var subService = db.SubServices.FirstOrDefault(ss => ss.SubServiceId == subServiceId);
            if (subService == null)
                return NotFound();

            foreach (var product in products)
            {
                var existingEntry = db.SubServiceProducts
                    .FirstOrDefault(ssp => ssp.SubServiceId == subServiceId && ssp.ProductId == product.ProductId);

                if (existingEntry != null)
                {
                    existingEntry.QuantityRequired = product.QuantityRequired;
                }
                else
                {
                    var newEntry = new SubServiceProduct
                    {
                        SubServiceId = subServiceId,
                        ProductId = product.ProductId,
                        QuantityRequired = product.QuantityRequired
                    };

                    db.SubServiceProducts.Add(newEntry);
                }
            }

            db.SaveChanges();

            return Ok(new { Message = "Products assigned to subservice successfully." });
        }

        #endregion

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
