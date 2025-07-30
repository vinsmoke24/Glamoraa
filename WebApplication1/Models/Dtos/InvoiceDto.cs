using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class InvoiceDto
	{
        public Guid InvoiceId { get; set; }
        public Guid AppointmentId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } // Paid/Unpaid
        public DateTime InvoiceDate { get; set; }
    }
}