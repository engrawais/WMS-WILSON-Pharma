using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;

namespace WMS.Controllers.EditAttendance
{
    public class CreateAttendance
    {
        public void CreateAttendanceForEmp(DateTime dateTime , List<Emp> emps)
        {
            if (dateTime<= DateTime.Today)
            {
                using (var ctx = new TAS2013Entities())
                {
                    List<RosterDetail> _NewRoster = new List<RosterDetail>();
                    _NewRoster = ctx.RosterDetails.Where(aa => aa.RosterDate == dateTime).ToList();
                    List<LvData> _LvData = new List<LvData>();
                    _LvData = ctx.LvDatas.Where(aa => aa.AttDate == dateTime).ToList();
                    List<LvShort> _lvShort = new List<LvShort>();
                    _lvShort = ctx.LvShorts.Where(aa => aa.DutyDate == dateTime).ToList();
                    List<AttData> _AttData = ctx.AttDatas.Where(aa => aa.AttDate == dateTime).ToList();
                    foreach (var emp in emps)
                    {
                        string empDate = emp.EmpID + dateTime.ToString("yyMMdd");
                        if (_AttData.Where(aa => aa.EmpDate == empDate).Count() == 0)
                        {
                            try
                            {
                                /////////////////////////////////////////////////////
                                //  Mark Everyone Absent while creating Attendance //
                                /////////////////////////////////////////////////////
                                //Set DUTYCODE = D, StatusAB = true, and Remarks = [Absent]
                                AttData att = new AttData();
                                att.AttDate = dateTime.Date;
                                att.DutyCode = "D";
                                att.StatusAB = true;
                                att.Remarks = "[Absent]";
                                if (emp.Shift != null)
                                    att.DutyTime = emp.Shift.StartTime;
                                else
                                    att.DutyTime = new TimeSpan(07, 45, 00);
                                att.EmpID = emp.EmpID;
                                att.EmpNo = emp.EmpNo;
                                att.EmpDate = emp.EmpID + dateTime.ToString("yyMMdd");
                                att.ShifMin = ProcessSupportFunc.CalculateShiftMinutes(emp.Shift, dateTime.DayOfWeek);
                                //////////////////////////
                                //  Check for Rest Day //
                                ////////////////////////
                                //Set DutyCode = R, StatusAB=false, StatusDO = true, and Remarks=[DO]
                                //Check for 1st Day Off of Shift
                                if (emp.Shift.DaysName.Name == ProcessSupportFunc.ReturnDayOfWeek(dateTime.DayOfWeek))
                                {
                                    att.DutyCode = "R";
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                }
                                //Check for 2nd Day Off of shift
                                if (emp.Shift.DaysName1.Name == ProcessSupportFunc.ReturnDayOfWeek(dateTime.DayOfWeek))
                                {
                                    att.DutyCode = "R";
                                    att.StatusAB = false;
                                    att.StatusDO = true;
                                    att.Remarks = "[DO]";
                                }
                                //////////////////////////
                                //  Check for Roster   //
                                ////////////////////////
                                ////New Roster
                                string empCdate = "Emp" + emp.EmpID.ToString() + dateTime.ToString("yyMMdd");
                                string sectionCdate = "Section" + emp.SecID.ToString() + dateTime.ToString("yyMMdd");
                                string crewCdate = "Crew" + emp.CrewID.ToString() + dateTime.ToString("yyMMdd");
                                string crewCdateAlternate = "C" + emp.CrewID.ToString() + dateTime.ToString("yyMMdd");
                                string shiftCdate = "Shift" + emp.ShiftID.ToString() + dateTime.ToString("yyMMdd");

                                if (_NewRoster.Where(aa => aa.CriteriaValueDate == empCdate).Count() > 0)
                                {
                                    var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == empCdate);
                                    if (roster.WorkMin == 0)
                                    {
                                        att.StatusAB = false;
                                        att.StatusDO = true;
                                        att.Remarks = "[DO]";
                                        att.DutyCode = "R";
                                        att.ShifMin = 0;
                                    }
                                    else
                                    {
                                        att.ShifMin = roster.WorkMin;
                                        att.DutyCode = "D";
                                        att.DutyTime = roster.DutyTime;
                                    }
                                }
                                else if (_NewRoster.Where(aa => aa.CriteriaValueDate == sectionCdate).Count() > 0)
                                {
                                    var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == sectionCdate);
                                    if (roster.WorkMin == 0)
                                    {
                                        att.StatusAB = false;
                                        att.StatusDO = true;
                                        att.Remarks = "[DO]";
                                        att.DutyCode = "R";
                                        att.ShifMin = 0;
                                    }
                                    else
                                    {
                                        att.ShifMin = roster.WorkMin;
                                        att.DutyCode = "D";
                                        att.DutyTime = roster.DutyTime;
                                    }
                                }
                                else if (_NewRoster.Where(aa => aa.CriteriaValueDate == crewCdate).Count() > 0)
                                {
                                    var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == crewCdate);
                                    if (roster.WorkMin == 0)
                                    {
                                        att.StatusAB = false;
                                        att.StatusDO = true;
                                        att.Remarks = "[DO]";
                                        att.DutyCode = "R";
                                        att.ShifMin = 0;
                                    }
                                    else
                                    {
                                        att.ShifMin = roster.WorkMin;
                                        att.DutyCode = "D";
                                        att.DutyTime = roster.DutyTime;
                                    }
                                }
                                else if (_NewRoster.Where(aa => aa.CriteriaValueDate == shiftCdate).Count() > 0)
                                {
                                    var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == shiftCdate);
                                    if (roster.WorkMin == 0)
                                    {
                                        att.StatusAB = false;
                                        att.StatusDO = true;
                                        att.Remarks = "[DO]";
                                        att.DutyCode = "R";
                                        att.ShifMin = 0;
                                    }
                                    else
                                    {
                                        att.ShifMin = roster.WorkMin;
                                        att.DutyCode = "D";
                                        att.DutyTime = roster.DutyTime;
                                    }
                                }
                                else if (_NewRoster.Where(aa => aa.CriteriaValueDate == crewCdate || aa.CriteriaValueDate
                                        == crewCdateAlternate).Count() > 0)
                                {
                                    var roster = _NewRoster.FirstOrDefault(aa => aa.CriteriaValueDate == crewCdate || aa.CriteriaValueDate
                                        == crewCdateAlternate);
                                    if (roster.WorkMin == 0)
                                    {
                                        att.StatusAB = false;
                                        att.StatusDO = true;
                                        att.Remarks = "[DO]";
                                        att.DutyCode = "R";
                                        att.ShifMin = 0;
                                    }
                                    else
                                    {
                                        att.ShifMin = roster.WorkMin;
                                        att.DutyCode = "D";
                                        att.DutyTime = roster.DutyTime;
                                    }
                                }
                                //////////////////////////
                                //  Check for GZ Day //
                                ////////////////////////
                                //Set DutyCode = R, StatusAB=false, StatusGZ = true, and Remarks=[GZ]
                                if (emp.Shift.GZDays == true)
                                {
                                    foreach (var holiday in ctx.Holidays)
                                    {
                                        if (ctx.Holidays.Where(hol => hol.HolDate.Month == att.AttDate.Value.Month && hol.HolDate.Day == att.AttDate.Value.Day).Count() > 0)
                                        {
                                            att.DutyCode = "G";
                                            att.StatusAB = false;
                                            att.StatusGZ = true;
                                            att.Remarks = "[GZ]";
                                            att.ShifMin = 0;
                                        }
                                    }
                                }
                                ////////////////////////////
                                //  Check for Short Leave//
                                //////////////////////////
                                foreach (var sLeave in _lvShort.Where(aa => aa.EmpDate == att.EmpDate))
                                {
                                    if (_lvShort.Where(lv => lv.EmpDate == att.EmpDate).Count() > 0)
                                    {
                                        att.StatusSL = true;
                                        att.StatusAB = null;
                                        att.DutyCode = "L";
                                        att.Remarks = "[Short Leave]";
                                    }
                                }

                                //////////////////////////
                                //   Check for Leave   //
                                ////////////////////////
                                //Set DutyCode = R, StatusAB=false, StatusGZ = true, and Remarks=[GZ]
                                foreach (var Leave in _LvData)
                                {
                                    var _Leave = _LvData.Where(lv => lv.EmpDate == att.EmpDate && lv.HalfLeave != true);
                                    if (_Leave.Count() > 0)
                                    {
                                        att.StatusLeave = true;
                                        att.StatusAB = false;
                                        att.DutyCode = "L";
                                        att.StatusDO = false;
                                        if (Leave.LvCode == "A")
                                            att.Remarks = "[CL]";
                                        else if (Leave.LvCode == "B")
                                            att.Remarks = "[AL]";
                                        else if (Leave.LvCode == "C")
                                            att.Remarks = "[SL]";
                                        else
                                            att.Remarks = "[" + _Leave.FirstOrDefault().LvType.LvDesc + "]";
                                    }
                                }

                                /////////////////////////
                                //Check for Half Leave///
                                ////////////////////////
                                var _HalfLeave = _LvData.Where(lv => lv.EmpDate == att.EmpDate && lv.HalfLeave == true);
                                if (_HalfLeave.Count() > 0)
                                {
                                    att.StatusLeave = true;
                                    att.StatusAB = false;
                                    att.DutyCode = "L";
                                    att.StatusHL = true;
                                    att.StatusDO = false;
                                    if (_HalfLeave.FirstOrDefault().LvCode == "A")
                                        att.Remarks = "[H-CL]";
                                    else if (_HalfLeave.FirstOrDefault().LvCode == "B")
                                        att.Remarks = "[S-AL]";
                                    else if (_HalfLeave.FirstOrDefault().LvCode == "C")
                                        att.Remarks = "[H-SL]";
                                    else if (_HalfLeave.FirstOrDefault().LvCode == "E")
                                        att.Remarks = "[H-CP]";
                                    else
                                        att.Remarks = "[Half Leave]";
                                }
                                ctx.AttDatas.Add(att);
                                ctx.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                } 
            }
        }




    }
}