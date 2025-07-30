using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
    public class ServiceDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool Status { get; set; }
        public int DurationMins { get; set; }
        public Guid SalonId { get; set; }


    }
    public class SubServiceDto
    {
        public string SubServiceName { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Active, Inactive, etc.
        public int ServiceId { get; set; } // The ID of the parent service
    }

    public class StaffWithServicesDto
	{
        public Guid StaffId { get; set; }
        public string FullName { get; set; }
        public List<ServiceDto> Services { get; set; }
    }
}