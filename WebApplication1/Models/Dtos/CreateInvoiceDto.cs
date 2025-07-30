using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class CreateInvoiceDto
	{
        public Guid CustomerId { get; set; }
        public Guid? AppointmentId { get; set; }
        public List<InvoiceItemServiceDto> Services { get; set; }
        public List<InvoiceItemProductDto> Products { get; set; }
        public string CouponCode { get; set; }   // optional
        public decimal TokenPaid { get; set; }   // optional
        public Guid CreatedBy { get; set; }   // receptionist/user ID
    }
}