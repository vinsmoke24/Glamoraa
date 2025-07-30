using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class AppointmentServiceProductDto
	{
        public Guid AppointmentId { get; set; }  // ID of the appointment
        public int SubServiceId { get; set; }  // ID of the subservice being used
        public Guid ProductId { get; set; }  // ID of the product being used
        public decimal Quantity { get; set; }
    }
}