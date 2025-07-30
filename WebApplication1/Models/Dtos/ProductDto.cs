using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class ProductDto
	{
        public string Name { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }  // E.g., Consumable, Retail
        public string Unit { get; set; }  // E.g., bottle, pack
        public decimal Price { get; set; }
    }
}