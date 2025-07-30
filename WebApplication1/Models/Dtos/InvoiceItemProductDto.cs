using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class InvoiceItemProductDto
	{
        public Guid ProductId { get; set; }
        public Guid StaffId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal GstRatePercent { get; set; }
    }
}