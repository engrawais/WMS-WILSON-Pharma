//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailEntryForm
    {
        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public string CCAddress { get; set; }
        public string CriteriaComLoc { get; set; }
        public Nullable<short> CompanyID { get; set; }
        public Nullable<short> DepartmentID { get; set; }
        public Nullable<short> SectionID { get; set; }
        public string CriteriaDepSec { get; set; }
        public Nullable<bool> ReportCurrentDate { get; set; }
        public Nullable<short> LocationID { get; set; }
        public Nullable<short> CatID { get; set; }
        public Nullable<bool> HasCat { get; set; }
        public Nullable<System.TimeSpan> EmailTime { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
        public virtual Location Location { get; set; }
        public virtual Section Section { get; set; }
    }
}
