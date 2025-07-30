using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class InvoiceItemServiceDto
	{
        public int AppointmentServiceId { get; set; }
        public Guid StaffId { get; set; }
        public decimal PriceAtBooking { get; set; }
        public decimal GstRatePercent { get; set; }
    }
}