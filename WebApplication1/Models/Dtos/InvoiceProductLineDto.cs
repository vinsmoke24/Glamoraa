using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class InvoiceProductLineDto
	{
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal GstRatePercent { get; set; }
        public decimal GstAmount => UnitPrice * Quantity * GstRatePercent / 100m;
    }
}