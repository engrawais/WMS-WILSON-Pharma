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
    
    public partial class EmergencyAuto
    {
        public short ID { get; set; }
        public Nullable<int> EmpID { get; set; }
        public System.DateTime T1 { get; set; }
        public Nullable<int> EmpID2 { get; set; }
        public Nullable<System.DateTime> T2 { get; set; }
        public Nullable<int> EmpID3 { get; set; }
        public Nullable<System.DateTime> T3 { get; set; }
        public Nullable<int> EmpID4 { get; set; }
        public Nullable<System.DateTime> T4 { get; set; }
        public Nullable<bool> IsEmergency { get; set; }
        public Nullable<bool> EmailSend { get; set; }
    }
}
