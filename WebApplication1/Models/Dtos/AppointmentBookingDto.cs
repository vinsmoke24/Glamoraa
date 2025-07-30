using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class AppointmentBookingDto
	{
        public Guid CustomerId { get; set; }
        public DateTime BookingDate { get; set; }          // e.g., 2025-05-14
        public DateTime PreferredTime { get; set; }

        public string Status { get; set; }  // e.g., "WalkinConfirmed", "OnlinePending"
                                            // e.g., 14:00
        public List<ServiceRequestDto> SelectedServices { get; set; }
        public decimal? TokenAmount { get; set; }              // ₹100 advance

    }
    public class SelectedServiceDto
    {
        public Guid ServiceId { get; set; }
        public Guid PreferredStaffId { get; set; }
        public decimal Price { get; set; }
        public int DurationMins { get; set; }
        public List<Guid> SelectedProductIds { get; set; }
    }
}