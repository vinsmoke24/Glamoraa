using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class BookedServiceDto
	{
        public string ServiceName { get; set; }
        public string AssignedStaffName { get; set; }
        public decimal PriceAtBooking { get; set; }
        public int EstimatedDurationMins { get; set; }
        public List<BookedProductDto> ProductsUsed { get; set; }  // optional
    }
}