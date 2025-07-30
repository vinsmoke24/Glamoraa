using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class BookedProductDto
	{
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    }
}