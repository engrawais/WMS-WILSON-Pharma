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
    
    public partial class EmergencyView
    {
        public int EmpID { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public Nullable<bool> IsSafe { get; set; }
        public Nullable<System.DateTime> LastEntryDateTime { get; set; }
        public Nullable<bool> Presence { get; set; }
        public string RdrName { get; set; }
        public Nullable<short> ReaderID { get; set; }
        public string DesignationName { get; set; }
        public string GradeName { get; set; }
        public string TypeName { get; set; }
        public string ShiftName { get; set; }
        public string SectionName { get; set; }
        public string DeptName { get; set; }
    }
}
