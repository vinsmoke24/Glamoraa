using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
    public class StaffDto
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Specializations { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public Guid SalonId { get; set; }

    }
}