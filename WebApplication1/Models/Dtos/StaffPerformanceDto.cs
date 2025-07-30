using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class StaffPerformanceDto
	{
        public Guid StaffId { get; set; }
        public string FullName { get; set; }
        public decimal RevenueGenerated { get; set; }
        public int ServicesHandled { get; set; }
        public double AverageRating { get; set; } // If ratings are collected
    }
}