//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Appointment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Appointment()
        {
            this.AppointmentServices = new HashSet<AppointmentService>();
            this.Feedbacks = new HashSet<Feedback>();
            this.Invoices = new HashSet<Invoice>();
            this.Payments = new HashSet<Payment>();
        }
    
        public System.Guid AppointmentId { get; set; }
        public System.Guid SalonId { get; set; }
        public Nullable<System.Guid> BranchId { get; set; }
        public System.Guid CustomerId { get; set; }
        public System.Guid ServiceId { get; set; }
        public int SubServiceId { get; set; }
        public Nullable<System.Guid> StaffId { get; set; }
        public Nullable<System.DateTime> PreferredDate { get; set; }
        public Nullable<System.TimeSpan> PreferredTime { get; set; }
        public string SpecialRequests { get; set; }
        public Nullable<decimal> FullAmount { get; set; }
        public Nullable<decimal> AdvancePaid { get; set; }
        public string CouponCode { get; set; }
        public Nullable<System.DateTime> BookingDate { get; set; }
        public string Status { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Salon Salon { get; set; }
        public virtual Service Service { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual SubService SubService { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
