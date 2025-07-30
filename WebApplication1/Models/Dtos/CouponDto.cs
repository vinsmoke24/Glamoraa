using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class CouponDto
    {
        public int CouponId { get; set; }
        public Guid SalonId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}