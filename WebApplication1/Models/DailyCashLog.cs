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
    
    public partial class DailyCashLog
    {
        public int Id { get; set; }
        public System.Guid SalonId { get; set; }
        public Nullable<System.Guid> BranchId { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<decimal> OpeningCash { get; set; }
        public Nullable<decimal> ClosingCash { get; set; }
        public string Notes { get; set; }
        public Nullable<System.Guid> RecordedBy { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Salon Salon { get; set; }
        public virtual User User { get; set; }
    }
}
