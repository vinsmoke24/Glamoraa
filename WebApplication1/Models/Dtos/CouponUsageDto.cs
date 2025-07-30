using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class CouponUsageDto
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime UsedOn { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}