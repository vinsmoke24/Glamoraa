using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
    public class AppointmentDetailsDto
    {
        public Guid AppointmentId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }      // Add CustomerName (or full details)
        public DateTime AppointmentDate { get; set; }
        public string StartTime { get; set; }

        public string Status { get; set; }
        public List<BookedServiceDto> Services { get; set; }
        public virtual Customer Customer { get; set; }

        public string CustomerPhone { get; set; } // add
        public string StylistName { get; set; }   // add
        public string Type { get; set; }          // optional, if you track type
        public string ServiceName { get; set; }   // add for first service name

        public string AssignedStaffName { get; set; }
    }
}