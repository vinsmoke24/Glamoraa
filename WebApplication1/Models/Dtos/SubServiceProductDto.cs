using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class SubServiceProductDto
	{
        public Guid ProductId { get; set; } 
        public decimal QuantityRequired { get; set; } 

    }
}