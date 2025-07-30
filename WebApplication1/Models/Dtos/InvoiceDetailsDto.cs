using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class InvoiceDetailsDto
	{
        public Guid InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TokenPaid { get; set; }
        public decimal TotalAmount { get; set; }
        public string CouponCode { get; set; }

        public List<InvoiceServiceLineDto> ServiceLines { get; set; }
        public List<InvoiceProductLineDto> ProductLines { get; set; }
    }
}