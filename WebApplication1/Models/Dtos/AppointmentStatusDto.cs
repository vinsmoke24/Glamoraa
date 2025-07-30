using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class AppointmentStatusDto
	{
        public Guid AppointmentId { get; set; }
        public string Status { get; set; } // e.g., Pending, Completed, Cancelled
        public DateTime UpdatedAt { get; set; }
    }
}