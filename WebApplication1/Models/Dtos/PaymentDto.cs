using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class PaymentDto
	{
        public int PaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMode { get; set; }        // e.g., Cash, Card, UPI
        public DateTime PaymentDate { get; set; }
        public bool IsTokenAdvance { get; set; }      // Add this property
    }
}