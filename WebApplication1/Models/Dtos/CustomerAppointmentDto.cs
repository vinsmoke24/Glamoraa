using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class CustomerAppointmentDto
	{
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> ServicesBooked { get; set; }
    }
}