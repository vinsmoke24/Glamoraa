using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class ProductStockDto
	{
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }  // Quantity added or deducted
        public string Operation { get; set; }  // E.g., 'Added' or 'Used'
        public string Notes { get; set; }  // Optional notes about the operation
    }
}