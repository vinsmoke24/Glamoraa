using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
    public class AvailabilityOverrideDto
    {
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public string Notes { get; set; }
    }

    public class StaffAvailabilityDto
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; }
        public List<AvailabilityOverrideDto> Overrides { get; set; }
    }

}