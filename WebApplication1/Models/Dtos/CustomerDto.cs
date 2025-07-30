using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class CustomerDto
	{
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public bool IsMember { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public decimal AmountSpent { get; set; }
        public string Status { get; set; } 
        public int? MembershipId { get; set; }
    }
}