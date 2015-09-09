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
    
    public partial class Department
    {
        public Department()
        {
            this.DailySumDepts = new HashSet<DailySumDept>();
            this.Sections = new HashSet<Section>();
        }
    
        public short DeptID { get; set; }
        public string DeptName { get; set; }
        public Nullable<short> DivID { get; set; }
        public Nullable<short> CompanyID { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual ICollection<DailySumDept> DailySumDepts { get; set; }
        public virtual Division Division { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
    }
}