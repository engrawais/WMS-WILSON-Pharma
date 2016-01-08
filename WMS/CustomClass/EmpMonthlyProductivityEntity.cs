using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMS.Models;
using WMS.Reports;


namespace WMS.CustomClass
{
  
   public class EmpMonthlyProductivityEntity
    {
       public string EmpName { get; set; }
       public string SectionName { get; set; }
       public string Designation { get; set; }
       public string EmpNo { get; set; }
       public string Company { get; set; }
       public double AW1 { get; set; }
       public double AW2 { get; set; }
       public double AW3 { get; set; }
       public double AW4 { get; set; }
       public double AW5 { get; set; }
       public double AW6 { get; set; }
       public double AW7 { get; set; }
       public double AW8 { get; set; }
       public double AW9 { get; set; }
       public double AW10 { get; set; }
       public double AW11 { get; set; }
        public double AW12{ get; set; }
        public double AW13 { get; set; }
        public double AW14 { get; set; }
        public double AW15 { get; set; }
        public double AW16 { get; set; }
        public double AW17 { get; set; }
        public double AW18 { get; set; }
        public double AW19 { get; set; }
        public double AW20 { get; set; }
        public double AW21 { get; set; }
        public double AW22 { get; set; }
        public double AW23 { get; set; }
        public double AW24 { get; set; }
        public double AW25{ get; set; }
        public double AW26 { get; set; }
        public double AW27 { get; set; }
        public double AW28 { get; set; }
        public double AW29 { get; set; }
        public double AW30 { get; set; }
        public double AW31 { get; set; }
        public double EW1 { get; set; }
        public double EW2 { get; set; }
        public double EW3 { get; set; }
        public double EW4 { get; set; }
        public double EW5 { get; set; }
        public double EW6 { get; set; }
        public double EW7 { get; set; }
        public double EW8 { get; set; }
        public double EW9{ get; set; }
        public double EW10 { get; set; }
        public double EW11 { get; set; }
        public double EW12 { get; set; }
        public double EW13 { get; set; }
        public double EW14 { get; set; }
        public double EW15 { get; set; }

        public double EW16 { get; set; }
        public double EW17 { get; set; }
        public double EW18 { get; set; }
        public double EW19 { get; set; }
        public double EW20 { get; set; }
        public double EW21 { get; set; }
        public double EW22 { get; set; }
        public double EW23 { get; set; }
        public double EW24 { get; set; }
        public double EW25 { get; set; }
        public double EW26{ get; set; }
        public double EW27 { get; set; }
        public double EW28{ get; set; }
        public double EW29{ get; set; }
        public double EW30{ get; set; }
        public double EW31{ get; set; }
        public double TotalExpectedWork{ get; set; }
        public double TotalActualWork{ get; set; }


        
   
        public static List<EmpMonthlyProductivityEntity> ProcessAttendence(List<ViewAttData> finalOutput, string _dateFrom, string _dateTo)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<EmpMonthlyProductivityEntity> listOfEmps = new List<EmpMonthlyProductivityEntity>();
            var result = finalOutput.OrderBy(m => m.EmpName ).Select(m => m.EmpID).Distinct();
          
            foreach (var emp in result)
            {
                EmpMonthlyProductivityEntity mp = new EmpMonthlyProductivityEntity();
                Emp employee = db.Emps.Where(aa => aa.EmpID == emp).FirstOrDefault();
                mp.EmpName = employee.EmpName;
                mp.Designation =employee.Designation.DesignationName;
                mp.EmpNo = employee.EmpNo;
                mp.Company = employee.Company.CompName;
                mp.SectionName =employee.Section.SectionName;
                mp.TotalActualWork = 0;
                mp.TotalExpectedWork = 0;
              
                List<ActualExpected> Mins = new List<ActualExpected>();
                Boolean found = false;
                    DateTime from = Convert.ToDateTime(_dateFrom);
                    DateTime to = Convert.ToDateTime(_dateTo);
                    while (from <= to)
                    {
                        foreach (ViewAttData att in finalOutput)
                        {
                           
                            
                                 if (att.EmpID == emp && att.AttDate == from)
                                        {

                                            found = true;
                                              from =   from.AddDays(1);
                                                ActualExpected ae = new ActualExpected();
                                                if (att.WorkMin != null)
                                                {
                                                    ae.actual = (double)att.WorkMin;
                                                    mp.TotalActualWork = mp.TotalActualWork + ae.actual;
                                                }
                                                else
                                                    ae.actual = 0.0;
                                                if (att.ShifMin != null || att.StatusDO == false || att.StatusGZ == false)
                                                {
                                                    ae.expected = (double)att.ShifMin;
                                                    mp.TotalExpectedWork = mp.TotalExpectedWork + ae.expected;
                                                }
                                                else
                                                    ae.expected = 0.0;
                                                Mins.Add(ae);

                                                break;

                                         
                                        
                                         }
                        }
                        if (!found)
                        {
                            from = from.AddDays(1);
                        }
                        else
                        {
                            found = false;
                        }



                      
                    }
                    mp = DecompressActualAndExpectedMinForRDLC(mp, Mins);
                    listOfEmps.Add(mp);
            }


            return listOfEmps;
            





        }

        private static EmpMonthlyProductivityEntity DecompressActualAndExpectedMinForRDLC(EmpMonthlyProductivityEntity mp, List<ActualExpected> Mins)
        {
            int count = 0;
            foreach (ActualExpected mins in Mins)
            {
                switch (count)
                
                {
                    case 0: mp.AW1 = mins.actual; mp.EW1 = mins.expected;
                        break;
                    case 1: mp.AW2 = mins.actual; mp.EW2 = mins.expected;
                        break;
                    case 2: mp.AW3 = mins.actual; mp.EW3 = mins.expected;
                        break;
                    case 3: mp.AW4 = mins.actual; mp.EW4 = mins.expected;
                        break;
                    case 4: mp.AW5 = mins.actual; mp.EW5 = mins.expected;
                        break;
                    case 5: mp.AW6 = mins.actual; mp.EW6 = mins.expected;
                        break;
                    case 6: mp.AW7 = mins.actual; mp.EW7 = mins.expected;
                        break;
                    case 7: mp.AW8 = mins.actual; mp.EW8 = mins.expected; break;
                    case 8: mp.AW9 = mins.actual; mp.EW9= mins.expected;
                        break;
                    case 9: mp.AW10 = mins.actual; mp.EW10 = mins.expected; break;
                    case 10: mp.AW11 = mins.actual; mp.EW11 = mins.expected;
                        break;
                    case 11: mp.AW12 = mins.actual; mp.EW12 = mins.expected; break;
                    case 12: mp.AW13 = mins.actual; mp.EW13 = mins.expected; break;
                    case 13: mp.AW14 = mins.actual; mp.EW14 = mins.expected; break;
                    case 14: mp.AW15 = mins.actual; mp.EW15 = mins.expected; break;
                    case 15: mp.AW16 = mins.actual; mp.EW16 = mins.expected; break;
                    case 16: mp.AW17 = mins.actual; mp.EW17= mins.expected; break;
                    case 17: mp.AW18 = mins.actual; mp.EW18 = mins.expected; break;
                    case 18: mp.AW19 = mins.actual; mp.EW19 = mins.expected; break;
                    case 19: mp.AW20 = mins.actual; mp.EW20 = mins.expected; break;
                    case 20: mp.AW21 = mins.actual; mp.EW21 = mins.expected; break;
                    case 21: mp.AW22 = mins.actual; mp.EW22 = mins.expected; break;
                    case 22: mp.AW23 = mins.actual; mp.EW23 = mins.expected; break;
                    case 23: mp.AW24 = mins.actual; mp.EW24 = mins.expected; break;
                    case 24: mp.AW25 = mins.actual; mp.EW25 = mins.expected; break;
                    case 25: mp.AW26 = mins.actual; mp.EW26 = mins.expected; break;

                    case 26: mp.AW27 = mins.actual; mp.EW27 = mins.expected; break;
                    case 27: mp.AW28 = mins.actual; mp.EW28 = mins.expected; break;
                    case 28: mp.AW29 = mins.actual; mp.EW29 = mins.expected; break;
                    case 29: mp.AW30 = mins.actual; mp.EW30 = mins.expected; break;
                    case 30: mp.AW31 = mins.actual; mp.EW31 = mins.expected; break;
                 

                
                
                }
                count++;
            }
            return mp;
        }
    }

    class ActualExpected
    {

       public double actual;
       public double expected;
    }

  
}
