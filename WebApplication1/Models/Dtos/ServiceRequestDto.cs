using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.DTOs
{
	public class ServiceRequestDto
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ServiceId { get; set; }
        public Guid? PreferredStaffId { get; set; }            // optional
        public List<Guid> SelectedProductIds { get; set; }     // optional or auto-filled
        public decimal Price { get; set; }                 // Add Price
        public int DurationMins { get; set; }              // Add DurationMins
    }
}