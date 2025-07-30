using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/staff")]
    public class StaffController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET: api/staff
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllStaff()
        {
            var staffList = db.Staffs.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Specializations = s.Specializations,
                IsActive = (bool)s.IsActive,
                DateOfJoining = s.DateOfJoining
            }).ToList();

            return Ok(staffList);
        }

        // GET: api/staff/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetStaffById(Guid id)
        {
            var staff = db.Staffs.Find(id);
            if (staff == null) return NotFound();

            return Ok(new StaffDto
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                Specializations = staff.Specializations,
                IsActive = (bool)staff.IsActive,
                DateOfJoining = staff.DateOfJoining
            });
        }

        // POST: api/staff
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateStaff([FromBody] StaffDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid currentSaloonId = new Guid("00000000-0000-0000-0000-000000000021");
            var staff = new Staff
            {
                StaffId = Guid.NewGuid(),
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Specializations = dto.Specializations,
                IsActive = true,
                DateOfJoining = DateTime.Now,
                SalonId = currentSaloonId  // ✅ THIS LINE REQUIRED!
            };


            db.Staffs.Add(staff);
            db.SaveChanges();

            return Ok(staff.StaffId);
        }

        // PUT: api/staff/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult UpdateStaff(Guid id, [FromBody] StaffDto dto)
        {
            var staff = db.Staffs.Find(id);
            if (staff == null) return NotFound();

            staff.FullName = dto.FullName;
            staff.PhoneNumber = dto.PhoneNumber;
            staff.Email = dto.Email;
            staff.Specializations = dto.Specializations;
            staff.IsActive = dto.IsActive;

            db.SaveChanges();
            return Ok("Updated");
        }

        // DELETE: api/staff/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult DeleteStaff(Guid id)
        {
            // Step 1: Find the staff record by ID
            var staff = db.Staffs.Find(id);
            if (staff == null)
            {
                // If the staff is not found, return a NotFound status with a message
                return NotFound();
            }

            // Step 2: Remove related records if performing hard delete
            var staffAttendances = db.StaffAttendances.Where(sa => sa.StaffId == id).ToList();
            var staffServices = db.StaffServices.Where(ss => ss.StaffId == id).ToList();
            var staffSubServices = db.StaffSubServices.Where(ss => ss.StaffId == id).ToList();

            // If you are doing hard delete, remove related records
            db.StaffAttendances.RemoveRange(staffAttendances);
            db.StaffServices.RemoveRange(staffServices);
            db.StaffSubServices.RemoveRange(staffSubServices);

            // Step 3: Remove the staff record
            db.Staffs.Remove(staff);

            // Step 4: Save changes to the database to commit the deletion
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any errors during deletion and provide detailed feedback
                return InternalServerError(new Exception("Failed to delete staff. Error: " + ex.Message));
            }

            // Step 5: Return success response
            return Ok("Staff deleted successfully");
        }


        // POST: api/staff/{id}/attendance/checkin
        [HttpPost]
        [Route("{id:guid}/attendance/checkin")]
        public IHttpActionResult CheckIn(Guid id)
        {
            var today = DateTime.Today;
            var record = db.StaffAttendances.FirstOrDefault(x => x.StaffId == id && x.AttendanceDate == today);

            if (record != null && record.CheckInTime != null)
                return BadRequest("Already checked in today.");

            if (record == null)
            {
                db.StaffAttendances.Add(new StaffAttendance
                {
                    StaffId = id,
                    AttendanceDate = today,
                    CheckInTime = DateTime.Now,
                    Status = "Present"
                });
            }
            else
            {
                record.CheckInTime = DateTime.Now;
                record.Status = "Present";
            }

            db.SaveChanges();
            return Ok("Check-in successful");
        }

        // POST: api/staff/{id}/attendance/checkout
        [HttpPost]
        [Route("{id:guid}/attendance/checkout")]
        public IHttpActionResult CheckOut(Guid id)
        {
            var today = DateTime.Today;
            var record = db.StaffAttendances.FirstOrDefault(x => x.StaffId == id && x.AttendanceDate == today);

            if (record == null || record.CheckInTime == null)
                return BadRequest("Check-in required before checkout.");

            if (record.CheckOutTime != null)
                return BadRequest("Already checked out.");

            record.CheckOutTime = DateTime.Now;
            db.SaveChanges();

            return Ok("Check-out successful");
        }

        // GET: api/staff/{id}/attendance/logs
        [HttpGet]
        [Route("{id:guid}/attendance/logs")]
        public IHttpActionResult GetAttendanceLogs(Guid id)
        {
            var records = db.StaffAttendances
                .Where(x => x.StaffId == id)
                .OrderByDescending(x => x.AttendanceDate)
                .Select(x => new StaffAttendanceDto
                {
                    AttendanceId = x.AttendanceId,
                    StaffId = x.StaffId,
                    Date = x.AttendanceDate,
                    CheckInTime = x.CheckInTime,
                    CheckOutTime = x.CheckOutTime,
                    Status = x.Status,
                    Notes = x.Notes
                }).ToList();

            return Ok(records);
        }

        // POST: api/staff/{id}/services
        [HttpPost]
        [Route("{id:guid}/services")]
        public IHttpActionResult AssignServicesToStaff(Guid id, [FromBody] List<Guid> serviceIds)
        {
            var existing = db.StaffServices.Where(ss => ss.StaffId == id);
            db.StaffServices.RemoveRange(existing);

            foreach (var sid in serviceIds)
            {
                db.StaffServices.Add(new StaffService
                {
                    StaffId = id,
                    ServiceId = sid
                });
            }

            db.SaveChanges();
            return Ok("Services updated");
        }

        // POST: api/staff/{id}/subservices
        [HttpPost]
        [Route("{id:guid}/subservices")]
        public IHttpActionResult AssignSubServicesToStaff(Guid id, [FromBody] List<int> subServiceIds)
        {
            var existing = db.StaffSubServices.Where(ss => ss.StaffId == id);
            db.StaffSubServices.RemoveRange(existing);

            foreach (var sid in subServiceIds)
            {
                db.StaffSubServices.Add(new StaffSubService
                {
                    StaffId = id,
                    SubServiceId = sid
                });
            }

            db.SaveChanges();
            return Ok("Sub-services updated");
        }
    }
}
