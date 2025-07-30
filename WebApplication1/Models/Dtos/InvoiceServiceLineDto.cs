using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class InvoiceServiceLineDto
	{
        public int AppointmentServiceId { get; set; }
        public string ServiceName { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; }
        public decimal PriceAtBooking { get; set; }
        public decimal GstRatePercent { get; set; }
        public decimal GstAmount => PriceAtBooking * GstRatePercent / 100m;
    }
}