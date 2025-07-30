using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Dtos
{
	public class StaffAttendanceDto
	{

        public int AttendanceId { get; set; }
        public Guid StaffId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } // Present, Absent, Late, Leave
        public string Notes { get; set; }
    }
}