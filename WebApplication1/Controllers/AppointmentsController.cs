using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;           // Your EF models
using WebApplication1.Models.DTOs;      // Your DTOs
using System.Data.Entity;                // Assuming EF 6

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/appointments")]
    public class AppointmentsController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // 1. Create (Online or Walk-in)
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> CreateAppointment(AppointmentBookingDto dto)
        {
            if (dto == null || dto.SelectedServices == null || !dto.SelectedServices.Any())
                return BadRequest("Invalid appointment booking data.");

            var customer = await db.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                return BadRequest("Customer not found.");

            var allProductIds = dto.SelectedServices
                .Where(s => s.SelectedProductIds != null)
                .SelectMany(s => s.SelectedProductIds)
                .Distinct()
                .ToList();

            var productPricesMap = await db.Products
                .Where(p => allProductIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p.Price);

            decimal fullAmount = dto.SelectedServices.Sum(s => s.Price) +
                dto.SelectedServices
                    .Where(s => s.SelectedProductIds != null)
                    .Sum(s => s.SelectedProductIds.Sum(pId => productPricesMap.ContainsKey(pId) ? productPricesMap[pId] : 0));

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                BookingDate = DateTime.UtcNow,
                PreferredDate = dto.BookingDate.Date,
                PreferredTime = dto.PreferredTime.TimeOfDay,
                Status = dto.Status ?? "OnlinePending",
                FullAmount = fullAmount,
                AdvancePaid = dto.TokenAmount ?? 0,
                SalonId = customer.SalonId,
                BranchId = customer.BranchId
            };

            db.Appointments.Add(appointment);

            foreach (var serviceReq in dto.SelectedServices)
            {
                db.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.AppointmentId,
                    ServiceId = serviceReq.ServiceId,
                    AssignedStaffId = serviceReq.PreferredStaffId,  // Assign the staff ID here
                    PriceAtBooking = serviceReq.Price,
                    EstimatedDurationMins = serviceReq.DurationMins
                });
            }

            await db.SaveChangesAsync();
            return CreatedAtRoute("GetAppointmentById", new { id = appointment.AppointmentId }, appointment.AppointmentId);
        }


        // 2. GET appointment by ID
        [HttpGet, Route("{id:guid}", Name = "GetAppointmentById")]
        public async Task<IHttpActionResult> GetAppointmentById(Guid id)
        {
            var appointment = await db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.AppointmentServices.Select(s => s.Service))
                .SingleOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            return Ok(new AppointmentDetailsDto
            {
                AppointmentId = appointment.AppointmentId,
                CustomerId = appointment.CustomerId,
                CustomerName = appointment.Customer?.FullName,
                AppointmentDate = (DateTime)appointment.PreferredDate,
                StartTime = appointment.PreferredTime.HasValue ? appointment.PreferredTime.Value.ToString(@"hh\:mm") : null,
                Status = appointment.Status,
                Services = appointment.AppointmentServices.Select(s => new BookedServiceDto
                {
                    ServiceName = s.Service?.ServiceName,
                    PriceAtBooking = (decimal)s.PriceAtBooking,
                    EstimatedDurationMins = (int)s.EstimatedDurationMins
                }).ToList()
            });
        }

        // 3. Get all appointments (with filters)
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAppointments(
            [FromUri] string status = null,
            [FromUri] Guid? customerId = null,
            [FromUri] DateTime? fromDate = null,
            [FromUri] DateTime? toDate = null,
            [FromUri] Guid? staffId = null,
            [FromUri] string type = null,
            [FromUri] Guid? serviceId = null)
        {
            var query = db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.AppointmentServices.Select(s => s.Service))
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (customerId.HasValue)
                query = query.Where(a => a.CustomerId == customerId.Value);

            if (fromDate.HasValue)
                query = query.Where(a => a.PreferredDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.PreferredDate <= toDate.Value.Date);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(a => a.Status == (type.ToLower() == "walk-in" ? "Confirmed" : "Pending"));

            if (staffId.HasValue)
                query = query.Where(a => a.AppointmentServices.Any(s => s.AssignedStaffId == staffId));

            if (serviceId.HasValue)
                query = query.Where(a => a.AppointmentServices.Any(s => s.ServiceId == serviceId));

            var list = await query.OrderByDescending(a => a.PreferredDate).ToListAsync();

            return Ok(list.Select(a => new AppointmentDetailsDto
            {
                AppointmentId = a.AppointmentId,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer?.FullName,
                AppointmentDate = (DateTime)a.PreferredDate,
                StartTime = a.PreferredTime.HasValue ? a.PreferredTime.Value.ToString(@"hh\:mm") : null,
                Status = a.Status,
                Services = a.AppointmentServices.Select(s => new BookedServiceDto
                {
                    ServiceName = s.Service?.ServiceName,
                    PriceAtBooking = (decimal)s.PriceAtBooking,
                    EstimatedDurationMins = (int)s.EstimatedDurationMins
                }).ToList()
            }));
        }

        // 4. Get all for single date
        [HttpGet, Route("day")]
        public async Task<IHttpActionResult> GetAppointmentsForDate([FromUri] DateTime date)
        {
            var day = date.Date;

            var list = await db.Appointments
                .Where(a => DbFunctions.TruncateTime(a.PreferredDate) == day)
                .Include(a => a.Customer)
                .Include(a => a.AppointmentServices.Select(s => s.Service))
                .ToListAsync();

            return Ok(list.Select(a => new AppointmentDetailsDto
            {
                AppointmentId = a.AppointmentId,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer?.FullName,
                CustomerPhone = a.Customer?.PhoneNumber,
                AppointmentDate = (DateTime)a.PreferredDate,
                StartTime = a.PreferredTime.HasValue ? a.PreferredTime.Value.ToString(@"hh\:mm") : null,
                ServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.ServiceName ?? "N/A",     // ✅ add
                Type = a.Status?.ToLower().Contains("walk") == true ? "Walk-in" : "Online",   // ✅ optional type

                Status = a.Status,
                Services = a.AppointmentServices.Select(s => new BookedServiceDto
                {
                    ServiceName = s.Service?.ServiceName ?? "N/A",
                    PriceAtBooking = (decimal)s.PriceAtBooking,
                    EstimatedDurationMins = (int)s.EstimatedDurationMins,
                    AssignedStaffName = s.Staff?.FullName ?? "N/A"
                }).ToList()
            }));
        }

        // 5. PATCH status update
        [HttpPatch, Route("{id:guid}/status")]
        public async Task<IHttpActionResult> ChangeAppointmentStatus(Guid id, AppointmentStatusDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Status))
                return BadRequest("Status must be provided.");

            var appointment = await db.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.Status = dto.Status;
           // appointment.AppointmentDate = dto.UpdatedAt;
            await db.SaveChangesAsync();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        // 6. PUT update/reschedule
        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> UpdateAppointment(Guid id, AppointmentBookingDto dto)
        {
            var appointment = await db.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.PreferredDate = dto.BookingDate.Date;
            appointment.PreferredTime = dto.PreferredTime.TimeOfDay;
            appointment.AdvancePaid = dto.TokenAmount ?? appointment.AdvancePaid;

            db.AppointmentServices.RemoveRange(db.AppointmentServices.Where(s => s.AppointmentId == id));

            foreach (var s in dto.SelectedServices)
            {
                db.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.AppointmentId,
                    ServiceId = s.ServiceId,
                    AssignedStaffId = s.PreferredStaffId,
                    PriceAtBooking = s.Price,
                    EstimatedDurationMins = s.DurationMins
                });
            }

            await db.SaveChangesAsync();
            return Ok();
        }

        // 7. DELETE appointment
        [HttpDelete, Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteAppointment(Guid id)
        {
            var appointment = await db.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            db.Appointments.Remove(appointment);
            await db.SaveChangesAsync();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }

}
