using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Controllers.Filters;
using WMS.Models;

namespace WMS.Controllers.EditAttendance
{

    public class ManualAttendanceProcess
    {
        TAS2013Entities context = new TAS2013Entities();
        AttData _OldAttData = new AttData();
        AttData _NewAttData = new AttData();
        AttDataManEdit _ManualEditData = new AttDataManEdit();

        //Replace New TimeIn and Out with Old TimeIN and Out in Attendance Data
        public ManualAttendanceProcess(string EmpDate, string JobCardName, bool JobCardStatus, DateTime NewTimeIn, DateTime NewTimeOut, string NewDutyCode, int _UserID, TimeSpan _NewDutyTime, string _Remarks,short _ShiftMins)
        {
            _OldAttData = context.AttDatas.First(aa => aa.EmpDate == EmpDate);
            if (_OldAttData != null)
            {
                if (JobCardStatus == false)
                {
                    SaveOldAttData(_OldAttData, _UserID);
                    if (SaveNewAttData(NewTimeIn, NewTimeOut, NewDutyCode, _NewDutyTime, _Remarks,_ShiftMins))
                    {
                        _OldAttData.TimeIn = NewTimeIn;
                        _OldAttData.TimeOut = NewTimeOut;
                        _OldAttData.DutyCode = NewDutyCode;
                        _OldAttData.DutyTime = _NewDutyTime;
                        _OldAttData.ShifMin = _ShiftMins;
                        if (_OldAttData.Remarks != null)
                        {
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[Absent]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[Manual]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[LO]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[LI]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[EO]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[EI]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[N-OT]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[G-OT]", "");
                            _OldAttData.Remarks = _OldAttData.Remarks.Replace("[R-OT]", "");
                        }
                        else
                            _OldAttData.Remarks = "[Manual]" + _OldAttData.Remarks;
                       if (_OldAttData.StatusLeave == true)
                    {
                        _OldAttData.DutyCode = "L";

                        _OldAttData.StatusAB = false;
                        _OldAttData.StatusP = false;
                        _OldAttData.StatusOT = false;
                        _OldAttData.OTMin = null;
                        _OldAttData.EarlyIn = null;
                        _OldAttData.EarlyOut = null;
                        _OldAttData.LateIn = null;
                        _OldAttData.LateOut = null;
                        _OldAttData.WorkMin = null;
                        _OldAttData.GZOTMin = null;
                    }
                       else if (_OldAttData.StatusHL == true)
                       {
                           _OldAttData.DutyCode = "L";
                           _OldAttData.StatusAB = true;
                           _OldAttData.StatusP = false;
                           _OldAttData.StatusOT = false;
                           _OldAttData.OTMin = null;
                           _OldAttData.EarlyIn = null;
                           _OldAttData.EarlyOut = null;
                           _OldAttData.LateIn = null;
                           _OldAttData.LateOut = null;
                           _OldAttData.WorkMin = null;
                           _OldAttData.GZOTMin = null;
                       }
                       else
                       {
                           switch (_OldAttData.DutyCode)
                           {
                               case "D":
                                   _OldAttData.StatusAB = true;
                                   _OldAttData.StatusP = false;
                                   _OldAttData.StatusMN = true;
                                   _OldAttData.StatusDO = false;
                                   _OldAttData.StatusGZ = false;
                                   _OldAttData.StatusOT = false;
                                   _OldAttData.OTMin = null;
                                   _OldAttData.EarlyIn = null;
                                   _OldAttData.EarlyOut = null;
                                   _OldAttData.LateIn = null;
                                   _OldAttData.LateOut = null;
                                   _OldAttData.WorkMin = null;
                                   _OldAttData.GZOTMin = null;
                                   break;
                               case "G":
                                   _OldAttData.StatusAB = false;
                                   _OldAttData.StatusP = false;
                                   _OldAttData.StatusMN = true;
                                   _OldAttData.StatusDO = false;
                                   _OldAttData.StatusGZ = true;
                                   _OldAttData.StatusLeave = false;
                                   _OldAttData.StatusOT = false;
                                   _OldAttData.OTMin = null;
                                   _OldAttData.EarlyIn = null;
                                   _OldAttData.EarlyOut = null;
                                   _OldAttData.LateIn = null;
                                   _OldAttData.LateOut = null;
                                   _OldAttData.WorkMin = null;
                                   _OldAttData.GZOTMin = null;
                                   break;
                               case "R":
                                   _OldAttData.StatusAB = false;
                                   _OldAttData.StatusP = false;
                                   _OldAttData.StatusMN = true;
                                   _OldAttData.StatusDO = true;
                                   _OldAttData.StatusGZ = false;
                                   _OldAttData.StatusLeave = false;
                                   _OldAttData.StatusOT = false;
                                   _OldAttData.OTMin = null;
                                   _OldAttData.EarlyIn = null;
                                   _OldAttData.EarlyOut = null;
                                   _OldAttData.LateIn = null;
                                   _OldAttData.LateOut = null;
                                   _OldAttData.WorkMin = null;
                                   _OldAttData.GZOTMin = null;
                                   break;
                           }
                       }
                        ProcessDailyAttendance(_OldAttData);
                    }
                }
            }
        }

        //Save Old and New Attendance Data in Manual Attendance Table
        private bool SaveNewAttData(DateTime _NewTimeIn, DateTime _NewTimeOut, string _NewDutyCode, TimeSpan _NewDutyTime, string _remarks,short _ShiftMins)
        {
            bool check = false;
            _ManualEditData.NewTimeIn = _NewTimeIn;
            _ManualEditData.NewTimeOut = _NewTimeOut;
            _ManualEditData.NewDutyCode = _NewDutyCode;
            _ManualEditData.EditDateTime = DateTime.Now;
            _ManualEditData.NewDutyTime = _NewDutyTime;
            _ManualEditData.NewRemarks = "[" + _remarks + "]";
            _ManualEditData.NewShiftMin = _ShiftMins;
            try
            {
                context.AttDataManEdits.Add(_ManualEditData);
                context.SaveChanges();
                check = true;
            }
            catch (Exception ex)
            {
                check = false;
            }
            return check;
        }

        private void SaveOldAttData(AttData _OldAttData, int Userid)
        {
            try
            {
                _ManualEditData.OldDutyCode = _OldAttData.DutyCode;
                _ManualEditData.OldTimeIn = _OldAttData.TimeIn;
                _ManualEditData.OldTimeOut = _OldAttData.TimeOut;
                _ManualEditData.EmpDate = _OldAttData.EmpDate;
                _ManualEditData.UserID = Userid;
                _ManualEditData.EditDateTime = DateTime.Now;
                _ManualEditData.EmpID = _OldAttData.EmpID;
                _ManualEditData.OldRemarks = _OldAttData.Remarks;
            }
            catch (Exception ex)
            {

            }
        }

        //Work Times calculation controller
        public void ProcessDailyAttendance(AttData _attData)
        {
            try
            {
                AttData attendanceRecord = _attData;
                Emp employee = attendanceRecord.Emp;
                Shift shift = employee.Shift;
                //If TimeIn and TimeOut are not null, then calculate other Atributes
                if (attendanceRecord.TimeIn != null && attendanceRecord.TimeOut != null)
                {
                    //If TimeIn = TimeOut then calculate according to DutyCode
                    if (attendanceRecord.TimeIn == attendanceRecord.TimeOut)
                    {
                        CalculateInEqualToOut(attendanceRecord);
                    }
                    else
                    {
                        if (attendanceRecord.DutyTime == new TimeSpan(0,0,0))
                        {
                            CalculateOpenShiftTimes(attendanceRecord, shift);
                        }
                        else
                        {
                            //if (attendanceRecord.TimeIn.Value.Date.Day == attendanceRecord.TimeOut.Value.Date.Day)
                            //{
                                CalculateShiftTimes(attendanceRecord, shift);
                            //}
                            //else
                            //{
                            //    CalculateOpenShiftTimes(attendanceRecord, shift);
                            //}
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }

            context.SaveChanges();
            context.Dispose();
        }

        TimeSpan OpenShiftThresholdStart = new TimeSpan(17, 00, 00);
        TimeSpan OpenShiftThresholdEnd = new TimeSpan(11, 00, 00);

        #region --Calculate Work Times--

        #region -- Calculate Work Times --

        public static void ACalculateShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                if (attendanceRecord.Remarks != null)
                {
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Manual]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[N-OT]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[G-OT]", "");
                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[R-OT]", "");
                }
                else
                    attendanceRecord.Remarks = "";
                if (attendanceRecord.StatusMN == true)
                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[Manual]";
                //Calculate WorkMin
                TimeSpan mins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                //Check if GZ holiday then place all WorkMin in GZOTMin
                if (attendanceRecord.StatusGZ == true)
                {
                    if (attendanceRecord.Emp.HasOT != false)
                    {
                        attendanceRecord.GZOTMin = (short)mins.TotalMinutes;
                        attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                        attendanceRecord.StatusGZOT = true;
                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[G-OT]";
                    }
                    else
                        attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                }
                //if Rest day then place all WorkMin in OTMin
                else if (attendanceRecord.StatusDO == true)
                {
                    if (attendanceRecord.Emp.HasOT != false)
                    {
                        attendanceRecord.OTMin = (short)mins.TotalMinutes;
                        attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                        attendanceRecord.StatusOT = true;
                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                    }
                    else
                    {
                        attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                    }
                }
                else
                {
                    /////////// to-do -----calculate Margins for those shifts which has break mins 
                    if (shift.HasBreak == true)
                    {
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                        attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                    }
                    else
                    {
                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                        attendanceRecord.StatusAB = false;
                        attendanceRecord.StatusP = true;
                        //Calculate Late IN, Compare margin with Shift Late In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay > attendanceRecord.DutyTime)
                        {
                            TimeSpan lateMinsSpan = (TimeSpan)(attendanceRecord.TimeIn.Value.TimeOfDay - attendanceRecord.DutyTime);
                            if (lateMinsSpan.TotalMinutes > shift.LateIn)
                            {
                                attendanceRecord.LateIn = (short)lateMinsSpan.TotalMinutes;
                                attendanceRecord.StatusLI = true;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LI]";
                            }
                            else
                            {
                                attendanceRecord.StatusLI = null;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLI = null;
                            attendanceRecord.LateIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                        }

                        //Calculate Early In, Compare margin with Shift Early In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay < attendanceRecord.DutyTime)
                        {
                            TimeSpan EarlyInMinsSpan = (TimeSpan)(attendanceRecord.DutyTime - attendanceRecord.TimeIn.Value.TimeOfDay);
                            if (EarlyInMinsSpan.TotalMinutes > shift.EarlyIn)
                            {
                                attendanceRecord.EarlyIn = (short)EarlyInMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEI = true;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EI]";
                            }
                            else
                            {
                                attendanceRecord.StatusEI = null;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEI = null;
                            attendanceRecord.EarlyIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                        }

                        // CalculateShiftEndTime = ShiftStart + DutyHours
                        DateTime shiftEnd = ProcessSupportFunc.CalculateShiftEndTimeWithAttData(attendanceRecord.AttDate.Value, attendanceRecord.DutyTime.Value, attendanceRecord.ShifMin);

                        //Calculate Early Out, Compare margin with Shift Early Out
                        if (attendanceRecord.TimeOut < shiftEnd)
                        {
                            TimeSpan EarlyOutMinsSpan = (TimeSpan)(shiftEnd - attendanceRecord.TimeOut);
                            if (EarlyOutMinsSpan.TotalMinutes > shift.EarlyOut)
                            {
                                attendanceRecord.EarlyOut = (short)EarlyOutMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEO = true;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                            }
                            else
                            {
                                attendanceRecord.StatusEO = null;
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEO = null;
                            attendanceRecord.EarlyOut = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                        }
                        //Calculate Late Out, Compare margin with Shift Late Out
                        if (attendanceRecord.TimeOut > shiftEnd)
                        {
                            TimeSpan LateOutMinsSpan = (TimeSpan)(attendanceRecord.TimeOut - shiftEnd);
                            if (LateOutMinsSpan.TotalMinutes > shift.LateOut)
                            {
                                attendanceRecord.LateOut = (short)LateOutMinsSpan.TotalMinutes;
                                // Late Out cannot have an early out, In case of poll at multiple times before and after shiftend
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.StatusLO = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LO]";
                            }
                            else
                            {
                                attendanceRecord.StatusLO = null;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLO = null;
                            attendanceRecord.LateOut = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                        }

                        //Subtract EarlyIn and LateOut from Work Minutes
                        //////-------to-do--------- Automate earlyin,lateout from shift setup
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                        if (attendanceRecord.EarlyIn != null && attendanceRecord.EarlyIn > shift.EarlyIn)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.EarlyIn);
                        }
                        if (attendanceRecord.LateOut != null && attendanceRecord.LateOut > shift.LateOut)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.LateOut);
                        }
                        if (attendanceRecord.LateOut != null || attendanceRecord.EarlyIn != null)

                            // round off work mins if overtime less than shift.OverTimeMin >
                            if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                            {
                                attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                            }
                        //Calculate OverTime = OT, Compare margin with Shift OverTime
                        //----to-do----- Handle from shift
                        //if (attendanceRecord.EarlyIn > shift.EarlyIn || attendanceRecord.LateOut > shift.LateOut)
                        //{
                        //    if (attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true)
                        //    {
                        //        short _EarlyIn;
                        //        short _LateOut;
                        //        if (attendanceRecord.EarlyIn == null)
                        //            _EarlyIn = 0;
                        //        else
                        //            _EarlyIn = 0;

                        //        if (attendanceRecord.LateOut == null)
                        //            _LateOut = 0;
                        //        else
                        //            _LateOut = (short)attendanceRecord.LateOut;

                        //        attendanceRecord.OTMin = (short)(_EarlyIn + _LateOut);
                        //        attendanceRecord.StatusOT = true;
                        //        attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                        //    }
                        //}
                        if ((attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true) && attendanceRecord.Emp.HasOT == true)
                        {
                            if (attendanceRecord.LateOut != null)
                            {
                                attendanceRecord.OTMin = attendanceRecord.LateOut;
                                attendanceRecord.StatusOT = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                            }
                        }
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                        //Mark Absent if less than 4 hours
                        if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                        {
                            int minMinutes = 0;
                            //currentday is present
                            if (shift.MinHrs > 100)
                            {
                                minMinutes = (int)attendanceRecord.ShifMin;
                                minMinutes = minMinutes / 2;
                            }
                            else
                                minMinutes = (int)shift.MinHrs;
                            if (attendanceRecord.StatusHL == true || attendanceRecord.Remarks.Contains("H-"))
                            {
                                attendanceRecord.EarlyOut = 0;
                                attendanceRecord.StatusEO = false;
                                attendanceRecord.LateIn = 0;
                                attendanceRecord.StatusLI = false;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                            }
                            else if (attendanceRecord.WorkMin < minMinutes)
                            {
                                attendanceRecord.StatusAB = true;
                                attendanceRecord.StatusP = false;
                                attendanceRecord.Remarks = "[Absent]";
                            }
                            else
                            {
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void ACalculateOpenShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                //Calculate WorkMin
                if (attendanceRecord != null)
                {
                    if (attendanceRecord.TimeOut != null && attendanceRecord.TimeIn != null)
                    {
                        if (attendanceRecord.Remarks != null)
                        {
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Manual]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[N-OT]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[G-OT]", "");
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[R-OT]", "");
                        }
                        else
                            attendanceRecord.Remarks = "";
                        if (attendanceRecord.StatusMN == true)
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[Manual]";
                        TimeSpan mins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                        //Check if GZ holiday then place all WorkMin in GZOTMin
                        if (attendanceRecord.StatusGZ == true)
                        {
                            attendanceRecord.GZOTMin = (short)mins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                            attendanceRecord.StatusGZOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[GZ-OT]";
                        }
                        else if (attendanceRecord.StatusDO == true)
                        {
                            attendanceRecord.OTMin = (short)mins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)mins.TotalMinutes;
                            attendanceRecord.StatusOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                            // RoundOff Overtime
                            if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                            {
                                if (attendanceRecord.OTMin > 0)
                                {
                                    float OTmins = (float)attendanceRecord.OTMin;
                                    float remainder = OTmins / 60;
                                    int intpart = (int)remainder;
                                    double fracpart = remainder - intpart;
                                    if (fracpart < 0.5)
                                    {
                                        attendanceRecord.OTMin = (short)(intpart * 60);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (shift.HasBreak == true)
                            {
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                                attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                            }
                            else
                            {
                                attendanceRecord.Remarks.Replace("[Absent]", "");
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                // CalculateShiftEndTime = ShiftStart + DutyHours
                                //TimeSpan shiftEnd = ProcessSupportFunc.CalculateShiftEndTime(shift, attendanceRecord.AttDate.Value.DayOfWeek);
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                                //Calculate OverTIme, 
                                if ((mins.TotalMinutes > (attendanceRecord.ShifMin + shift.OverTimeMin)) && attendanceRecord.Emp.HasOT == true)
                                {
                                    attendanceRecord.OTMin = (Int16)(Convert.ToInt16(mins.TotalMinutes) - attendanceRecord.ShifMin);
                                    attendanceRecord.WorkMin = (short)((mins.TotalMinutes) - attendanceRecord.OTMin);
                                    attendanceRecord.StatusOT = true;
                                    attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                                }
                                //Calculate Early Out
                                if (mins.TotalMinutes < (attendanceRecord.ShifMin - shift.EarlyOut))
                                {
                                    Int16 EarlyoutMin = (Int16)(attendanceRecord.ShifMin - Convert.ToInt16(mins.TotalMinutes));
                                    if (EarlyoutMin > shift.EarlyOut)
                                    {
                                        attendanceRecord.EarlyOut = EarlyoutMin;
                                        attendanceRecord.StatusEO = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusEO = null;
                                        attendanceRecord.EarlyOut = null;
                                        attendanceRecord.Remarks.Replace("[EO]", "");
                                    }
                                }
                                else
                                {
                                    attendanceRecord.StatusEO = null;
                                    attendanceRecord.EarlyOut = null;
                                    attendanceRecord.Remarks.Replace("[EO]", "");
                                }
                                // round off work mins if overtime less than shift.OverTimeMin >
                                if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                                {
                                    attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                                }
                                // RoundOff Overtime
                                if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                                {
                                    if (attendanceRecord.OTMin > 0)
                                    {
                                        float OTmins = (float)attendanceRecord.OTMin;
                                        float remainder = OTmins / 60;
                                        int intpart = (int)remainder;
                                        double fracpart = remainder - intpart;
                                        if (fracpart < 0.5)
                                        {
                                            attendanceRecord.OTMin = (short)(intpart * 60);
                                        }
                                    }
                                }
                                //Mark Absent if less than 4 hours
                                if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                                {
                                    int minMinutes = 0;
                                    //currentday is present
                                    if (shift.MinHrs > 100)
                                    {
                                        minMinutes = (int)attendanceRecord.ShifMin;
                                        minMinutes = minMinutes / 2;
                                    }
                                    else
                                        minMinutes = (int)shift.MinHrs;
                                    if (attendanceRecord.StatusHL == true || attendanceRecord.Remarks.Contains("H-"))
                                    {
                                        attendanceRecord.EarlyOut = 0;
                                        attendanceRecord.StatusEO = false;
                                        attendanceRecord.LateIn = 0;
                                        attendanceRecord.StatusLI = false;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                                    }
                                    else if (attendanceRecord.WorkMin < minMinutes)
                                    {
                                        attendanceRecord.StatusAB = true;
                                        attendanceRecord.StatusP = false;
                                        attendanceRecord.Remarks = "[Absent]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusAB = false;
                                        attendanceRecord.StatusP = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        #endregion
        public static void CalculateShiftTimes(AttData attendanceRecord, Shift shift)
        {
            //if (attendanceRecord.Emp.TypeID == 90)
            //{
            //    SMCalculateShiftTimes(attendanceRecord, shift);
            //}
            //else
            ACalculateShiftTimes(attendanceRecord, shift);
        }
        public static void CalculateOpenShiftTimes(AttData attendanceRecord, Shift shift)
        {
            if (attendanceRecord != null)
            {
                //if (attendanceRecord.Emp.TypeID == 90)
                //{
                //    SMCalculateOpenShiftTimes(attendanceRecord, shift);
                //}
                //else
                ACalculateOpenShiftTimes(attendanceRecord, shift);
            }
        }
        #region --Calculate Sugar Mill Employees
        public static void SMCalculateShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                attendanceRecord.Remarks = "";
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[N-OT]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[G-OT]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[R-OT]", "");
                //Calculate WorkMin
                TimeSpan TotalMins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                TimeSpan mins = (TimeSpan)(attendanceRecord.Tout0 - attendanceRecord.Tin0);
                //Check if GZ holiday then place all WorkMin in GZOTMin
                if (attendanceRecord.StatusGZ == true)
                {
                    if (attendanceRecord.Emp.HasOT != false)
                    {
                        attendanceRecord.GZOTMin = (short)TotalMins.TotalMinutes;
                        attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                        attendanceRecord.StatusGZOT = true;
                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[G-OT]";
                    }
                    else
                        attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                }
                //if Rest day then place all WorkMin in OTMin
                else if (attendanceRecord.StatusDO == true)
                {
                    if (attendanceRecord.Emp.HasOT != false)
                    {
                        attendanceRecord.OTMin = (short)TotalMins.TotalMinutes;
                        attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                        attendanceRecord.StatusOT = true;
                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                    }
                    else
                    {
                        attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                    }
                }
                else
                {
                    /////////// to-do -----calculate Margins for those shifts which has break mins 
                    if (shift.HasBreak == true)
                    {
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                        attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                    }
                    else
                    {
                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                        attendanceRecord.StatusAB = false;
                        attendanceRecord.StatusP = true;
                        //Calculate Late IN, Compare margin with Shift Late In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay > attendanceRecord.DutyTime)
                        {
                            TimeSpan lateMinsSpan = (TimeSpan)(attendanceRecord.TimeIn.Value.TimeOfDay - attendanceRecord.DutyTime);
                            if (lateMinsSpan.TotalMinutes > shift.LateIn)
                            {
                                attendanceRecord.LateIn = (short)lateMinsSpan.TotalMinutes;
                                attendanceRecord.StatusLI = true;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LI]";
                            }
                            else
                            {
                                attendanceRecord.StatusLI = null;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLI = null;
                            attendanceRecord.LateIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                        }

                        //Calculate Early In, Compare margin with Shift Early In
                        if (attendanceRecord.TimeIn.Value.TimeOfDay < attendanceRecord.DutyTime)
                        {
                            TimeSpan EarlyInMinsSpan = (TimeSpan)(attendanceRecord.DutyTime - attendanceRecord.TimeIn.Value.TimeOfDay);
                            if (EarlyInMinsSpan.TotalMinutes > shift.EarlyIn)
                            {
                                attendanceRecord.EarlyIn = (short)EarlyInMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEI = true;
                                attendanceRecord.LateIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EI]";
                            }
                            else
                            {
                                attendanceRecord.StatusEI = null;
                                attendanceRecord.EarlyIn = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEI = null;
                            attendanceRecord.EarlyIn = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                        }

                        // CalculateShiftEndTime = ShiftStart + DutyHours
                        DateTime shiftEnd = ProcessSupportFunc.CalculateShiftEndTimeWithAttData(attendanceRecord.AttDate.Value, attendanceRecord.DutyTime.Value, attendanceRecord.ShifMin);

                        //Calculate Early Out, Compare margin with Shift Early Out
                        if (attendanceRecord.TimeOut < shiftEnd)
                        {
                            TimeSpan EarlyOutMinsSpan = (TimeSpan)(shiftEnd - attendanceRecord.TimeOut);
                            if (EarlyOutMinsSpan.TotalMinutes > shift.EarlyOut)
                            {
                                attendanceRecord.EarlyOut = (short)EarlyOutMinsSpan.TotalMinutes;
                                attendanceRecord.StatusEO = true;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                            }
                            else
                            {
                                attendanceRecord.StatusEO = null;
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusEO = null;
                            attendanceRecord.EarlyOut = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                        }
                        //Calculate Late Out, Compare margin with Shift Late Out
                        if (attendanceRecord.TimeOut > shiftEnd)
                        {
                            TimeSpan LateOutMinsSpan = (TimeSpan)(attendanceRecord.TimeOut - shiftEnd);
                            if (LateOutMinsSpan.TotalMinutes > shift.LateOut)
                            {
                                attendanceRecord.LateOut = (short)LateOutMinsSpan.TotalMinutes;
                                // Late Out cannot have an early out, In case of poll at multiple times before and after shiftend
                                attendanceRecord.EarlyOut = null;
                                attendanceRecord.StatusLO = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[LO]";
                            }
                            else
                            {
                                attendanceRecord.StatusLO = null;
                                attendanceRecord.LateOut = null;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                            }
                        }
                        else
                        {
                            attendanceRecord.StatusLO = null;
                            attendanceRecord.LateOut = null;
                            attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                        }

                        //Subtract EarlyIn and LateOut from Work Minutes
                        //////-------to-do--------- Automate earlyin,lateout from shift setup
                        attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                        if (attendanceRecord.EarlyIn != null && attendanceRecord.EarlyIn > shift.EarlyIn)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.EarlyIn);
                        }
                        if (attendanceRecord.LateOut != null && attendanceRecord.LateOut > shift.LateOut)
                        {
                            attendanceRecord.WorkMin = (short)(attendanceRecord.WorkMin - attendanceRecord.LateOut);
                        }
                        if (attendanceRecord.LateOut != null || attendanceRecord.EarlyIn != null)

                            // round off work mins if overtime less than shift.OverTimeMin >
                            if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                            {
                                attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                            }
                        //Calculate OverTime = OT, Compare margin with Shift OverTime
                        //----to-do----- Handle from shift
                        //if (attendanceRecord.EarlyIn > shift.EarlyIn || attendanceRecord.LateOut > shift.LateOut)
                        //{
                        //    if (attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true)
                        //    {
                        //        short _EarlyIn;
                        //        short _LateOut;
                        //        if (attendanceRecord.EarlyIn == null)
                        //            _EarlyIn = 0;
                        //        else
                        //            _EarlyIn = 0;

                        //        if (attendanceRecord.LateOut == null)
                        //            _LateOut = 0;
                        //        else
                        //            _LateOut = (short)attendanceRecord.LateOut;

                        //        attendanceRecord.OTMin = (short)(_EarlyIn + _LateOut);
                        //        attendanceRecord.StatusOT = true;
                        //        attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                        //    }
                        //}
                        if ((attendanceRecord.StatusGZ != true || attendanceRecord.StatusDO != true) && attendanceRecord.Emp.HasOT == true)
                        {
                            //if (attendanceRecord.LateOut != null)
                            //{
                            //    attendanceRecord.OTMin = attendanceRecord.LateOut;
                            //    attendanceRecord.StatusOT = true;
                            //    attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                            //}
                            if (attendanceRecord.Tout1 != null && attendanceRecord.Tin1 != null)
                            {
                                TimeSpan ts = (TimeSpan)(attendanceRecord.Tout1 - attendanceRecord.Tin1);
                                attendanceRecord.OTMin = (short)ts.TotalMinutes;
                                attendanceRecord.StatusOT = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                            }
                        }
                        // RoundOff Overtime
                        if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                        {
                            if (attendanceRecord.OTMin > 0)
                            {
                                float OTmins = (float)attendanceRecord.OTMin;
                                float remainder = OTmins / 60;
                                int intpart = (int)remainder;
                                double fracpart = remainder - intpart;
                                if (fracpart < 0.5)
                                {
                                    attendanceRecord.OTMin = (short)(intpart * 60);
                                }
                            }
                        }
                        //Mark Absent if less than 4 hours
                        if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                        {
                            int minMinutes = 0;
                            //currentday is present
                            if (shift.MinHrs > 100)
                            {
                                minMinutes = (int)attendanceRecord.ShifMin;
                                minMinutes = minMinutes / 2;
                            }
                            else
                                minMinutes = (int)shift.MinHrs;
                            if (attendanceRecord.WorkMin < minMinutes)
                            {
                                attendanceRecord.StatusAB = true;
                                attendanceRecord.StatusP = false;
                                attendanceRecord.Remarks = attendanceRecord.Remarks = "[Absent]";
                            }
                            else
                            {
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void SMCalculateOpenShiftTimes(AttData attendanceRecord, Shift shift)
        {
            try
            {
                attendanceRecord.Remarks = "";
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LO]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[LI]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EI]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[N-OT]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[G-OT]", "");
                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[R-OT]", "");
                //Calculate WorkMin
                if (attendanceRecord != null)
                {
                    if (attendanceRecord.TimeOut != null && attendanceRecord.TimeIn != null)
                    {
                        attendanceRecord.Remarks = "";
                        TimeSpan TotalMins = (TimeSpan)(attendanceRecord.TimeOut - attendanceRecord.TimeIn);
                        TimeSpan mins = (TimeSpan)(attendanceRecord.Tout0 - attendanceRecord.Tin0);
                        //Check if GZ holiday then place all WorkMin in GZOTMin
                        if (attendanceRecord.StatusGZ == true)
                        {
                            attendanceRecord.GZOTMin = (short)TotalMins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                            attendanceRecord.StatusGZOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[GZ-OT]";
                        }
                        else if (attendanceRecord.StatusDO == true)
                        {
                            attendanceRecord.OTMin = (short)TotalMins.TotalMinutes;
                            attendanceRecord.WorkMin = (short)TotalMins.TotalMinutes;
                            attendanceRecord.StatusOT = true;
                            attendanceRecord.Remarks = attendanceRecord.Remarks + "[R-OT]";
                            // RoundOff Overtime
                            if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                            {
                                if (attendanceRecord.OTMin > 0)
                                {
                                    float OTmins = (float)attendanceRecord.OTMin;
                                    float remainder = OTmins / 60;
                                    int intpart = (int)remainder;
                                    double fracpart = remainder - intpart;
                                    if (fracpart < 0.5)
                                    {
                                        attendanceRecord.OTMin = (short)(intpart * 60);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (shift.HasBreak == true)
                            {
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes - shift.BreakMin);
                                attendanceRecord.ShifMin = (short)(ProcessSupportFunc.CalculateShiftMinutes(shift, attendanceRecord.AttDate.Value.DayOfWeek) - (short)shift.BreakMin);
                            }
                            else
                            {
                                attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                                attendanceRecord.StatusAB = false;
                                attendanceRecord.StatusP = true;
                                // CalculateShiftEndTime = ShiftStart + DutyHours
                                //TimeSpan shiftEnd = ProcessSupportFunc.CalculateShiftEndTime(shift, attendanceRecord.AttDate.Value.DayOfWeek);
                                attendanceRecord.WorkMin = (short)(mins.TotalMinutes);
                                //Calculate OverTIme, 
                                if ((mins.TotalMinutes > (attendanceRecord.ShifMin + shift.OverTimeMin)) && attendanceRecord.Emp.HasOT == true)
                                {
                                    //attendanceRecord.OTMin = (Int16)(Convert.ToInt16(mins.TotalMinutes) - attendanceRecord.ShifMin);
                                    //attendanceRecord.WorkMin = (short)((mins.TotalMinutes) - attendanceRecord.OTMin);
                                    //attendanceRecord.StatusOT = true;
                                    //attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                                    if (attendanceRecord.Tout1 != null && attendanceRecord.Tin1 != null)
                                    {
                                        TimeSpan ts = (TimeSpan)(attendanceRecord.Tout1 - attendanceRecord.Tin1);
                                        attendanceRecord.OTMin = (short)ts.TotalMinutes;
                                        attendanceRecord.StatusOT = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[N-OT]";
                                    }
                                }
                                //Calculate Early Out
                                if (mins.TotalMinutes < (attendanceRecord.ShifMin - shift.EarlyOut))
                                {
                                    Int16 EarlyoutMin = (Int16)(attendanceRecord.ShifMin - Convert.ToInt16(mins.TotalMinutes));
                                    if (EarlyoutMin > shift.EarlyOut)
                                    {
                                        attendanceRecord.EarlyOut = EarlyoutMin;
                                        attendanceRecord.StatusEO = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[EO]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusEO = null;
                                        attendanceRecord.EarlyOut = null;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                                    }
                                }
                                else
                                {
                                    attendanceRecord.StatusEO = null;
                                    attendanceRecord.EarlyOut = null;
                                    attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[EO]", "");
                                }
                                // round off work mins if overtime less than shift.OverTimeMin >
                                if (attendanceRecord.WorkMin > attendanceRecord.ShifMin && (attendanceRecord.WorkMin <= (attendanceRecord.ShifMin + shift.OverTimeMin)))
                                {
                                    attendanceRecord.WorkMin = attendanceRecord.ShifMin;
                                }
                                // RoundOff Overtime
                                if ((attendanceRecord.Emp.EmpType.CatID == 2 || attendanceRecord.Emp.EmpType.CatID == 4) && attendanceRecord.Emp.CompanyID == 1)
                                {
                                    if (attendanceRecord.OTMin > 0)
                                    {
                                        float OTmins = (float)attendanceRecord.OTMin;
                                        float remainder = OTmins / 60;
                                        int intpart = (int)remainder;
                                        double fracpart = remainder - intpart;
                                        if (fracpart < 0.5)
                                        {
                                            attendanceRecord.OTMin = (short)(intpart * 60);
                                        }
                                    }
                                }
                                //Mark Absent if less than 4 hours
                                if (attendanceRecord.AttDate.Value.DayOfWeek != DayOfWeek.Friday && attendanceRecord.StatusDO != true && attendanceRecord.StatusGZ != true)
                                {
                                    int minMinutes = 0;
                                    //currentday is present
                                    if (shift.MinHrs > 100)
                                    {
                                        minMinutes = (int)attendanceRecord.ShifMin;
                                        minMinutes = minMinutes / 2;
                                    }
                                    else
                                        minMinutes = (int)shift.MinHrs;
                                    if (attendanceRecord.WorkMin < minMinutes)
                                    {
                                        attendanceRecord.StatusAB = true;
                                        attendanceRecord.StatusP = false;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks + "[Absent]";
                                    }
                                    else
                                    {
                                        attendanceRecord.StatusAB = false;
                                        attendanceRecord.StatusP = true;
                                        attendanceRecord.Remarks = attendanceRecord.Remarks.Replace("[Absent]", "");
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
        #endregion
        private void CalculateInEqualToOut(AttData attendanceRecord)
        {
            attendanceRecord.TimeIn = null;
            attendanceRecord.TimeOut = null;
            if (attendanceRecord.StatusLeave == true)
            {
                attendanceRecord.DutyCode = "L";
            }
            else if (attendanceRecord.StatusHL == true)
            {
                attendanceRecord.DutyCode = "L";
                attendanceRecord.StatusAB = true;
                attendanceRecord.StatusGZ = false;
                attendanceRecord.WorkMin = 0;
                attendanceRecord.EarlyIn = 0;
                attendanceRecord.EarlyOut = 0;
                attendanceRecord.LateIn = 0;
                attendanceRecord.LateOut = 0;
                attendanceRecord.OTMin = 0;
                attendanceRecord.GZOTMin = 0;
                attendanceRecord.StatusGZOT = false;
                attendanceRecord.StatusDO = false;
                attendanceRecord.StatusP = false;
                attendanceRecord.Remarks = "[Absent][Manual]";
            }
            else
            {
                switch (attendanceRecord.DutyCode)
                {
                    case "G":
                        attendanceRecord.StatusAB = false;
                        attendanceRecord.StatusGZ = true;
                        attendanceRecord.WorkMin = 0;
                        attendanceRecord.EarlyIn = 0;
                        attendanceRecord.EarlyOut = 0;
                        attendanceRecord.LateIn = 0;
                        attendanceRecord.LateOut = 0;
                        attendanceRecord.OTMin = 0;
                        attendanceRecord.GZOTMin = 0;
                        attendanceRecord.StatusGZOT = false;
                        attendanceRecord.Remarks = "[GZ][Manual]";
                        break;
                    case "R":
                        attendanceRecord.StatusAB = false;
                        attendanceRecord.StatusGZ = false;
                        attendanceRecord.WorkMin = 0;
                        attendanceRecord.EarlyIn = 0;
                        attendanceRecord.EarlyOut = 0;
                        attendanceRecord.LateIn = 0;
                        attendanceRecord.LateOut = 0;
                        attendanceRecord.OTMin = 0;
                        attendanceRecord.GZOTMin = 0;
                        attendanceRecord.StatusGZOT = false;
                        attendanceRecord.StatusDO = true;
                        attendanceRecord.Remarks = "[DO][Manual]";
                        break;
                    case "D":
                        attendanceRecord.StatusAB = true;
                        attendanceRecord.StatusGZ = false;
                        attendanceRecord.WorkMin = 0;
                        attendanceRecord.EarlyIn = 0;
                        attendanceRecord.EarlyOut = 0;
                        attendanceRecord.LateIn = 0;
                        attendanceRecord.LateOut = 0;
                        attendanceRecord.OTMin = 0;
                        attendanceRecord.GZOTMin = 0;
                        attendanceRecord.StatusGZOT = false;
                        attendanceRecord.StatusDO = false;
                        attendanceRecord.StatusP = false;
                        attendanceRecord.Remarks = "[Absent][Manual]";
                        break;
                }
            }

        }
        #region --Helper Functions--
        private string ReturnDayOfWeek(DayOfWeek dayOfWeek)
        {
            string _DayName = "";
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    _DayName = "Monday";
                    break;
                case DayOfWeek.Tuesday:
                    _DayName = "Tuesday";
                    break;
                case DayOfWeek.Wednesday:
                    _DayName = "Wednesday";
                    break;
                case DayOfWeek.Thursday:
                    _DayName = "Thursday";
                    break;
                case DayOfWeek.Friday:
                    _DayName = "Friday";
                    break;
                case DayOfWeek.Saturday:
                    _DayName = "Saturday";
                    break;
                case DayOfWeek.Sunday:
                    _DayName = "Sunday";
                    break;
            }
            return _DayName;
        }

        private TimeSpan CalculateShiftEndTime(Shift shift, DayOfWeek dayOfWeek)
        {
            Int16 workMins = 0;
            try
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        workMins = shift.MonMin;
                        break;
                    case DayOfWeek.Tuesday:
                        workMins = shift.TueMin;
                        break;
                    case DayOfWeek.Wednesday:
                        workMins = shift.WedMin;
                        break;
                    case DayOfWeek.Thursday:
                        workMins = shift.ThuMin;
                        break;
                    case DayOfWeek.Friday:
                        workMins = shift.FriMin;
                        break;
                    case DayOfWeek.Saturday:
                        workMins = shift.SatMin;
                        break;
                    case DayOfWeek.Sunday:
                        workMins = shift.SunMin;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return shift.StartTime + (new TimeSpan(0, workMins, 0));
        }
        private TimeSpan CalculateShiftEndTime(Shift shift,short ShiftMins)
        {

            return shift.StartTime + (new TimeSpan(0, ShiftMins, 0));
        }
        private DateTime CalculateShiftEndTime(Shift shift, DateTime _AttDate, TimeSpan _DutyTime)
        {
            Int16 workMins = 0;
            try
            {
                switch (_AttDate.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        workMins = shift.MonMin;
                        break;
                    case DayOfWeek.Tuesday:
                        workMins = shift.TueMin;
                        break;
                    case DayOfWeek.Wednesday:
                        workMins = shift.WedMin;
                        break;
                    case DayOfWeek.Thursday:
                        workMins = shift.ThuMin;
                        break;
                    case DayOfWeek.Friday:
                        workMins = shift.FriMin;
                        break;
                    case DayOfWeek.Saturday:
                        workMins = shift.SatMin;
                        break;
                    case DayOfWeek.Sunday:
                        workMins = shift.SunMin;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            DateTime _datetime = new DateTime();
            TimeSpan _Time = new TimeSpan(0, workMins, 0);
            _datetime = _AttDate.Date.Add(_DutyTime);
            _datetime = _datetime.Add(_Time);
            return _datetime;
        }

        private DateTime CalculateShiftEndTime(Shift shift, DateTime _AttDate, TimeSpan _DutyTime,short ShiftMins)
        {
            DateTime _datetime = new DateTime();
            TimeSpan _Time = new TimeSpan(0, ShiftMins, 0);
            _datetime = _AttDate.Date.Add(_DutyTime);
            _datetime = _datetime.Add(_Time);
            return _datetime;
        }
        private Int16 CalculateShiftMinutes(Shift shift, DayOfWeek dayOfWeek)
        {
            Int16 workMins = 0;
            try
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        workMins = shift.MonMin;
                        break;
                    case DayOfWeek.Tuesday:
                        workMins = shift.TueMin;
                        break;
                    case DayOfWeek.Wednesday:
                        workMins = shift.WedMin;
                        break;
                    case DayOfWeek.Thursday:
                        workMins = shift.ThuMin;
                        break;
                    case DayOfWeek.Friday:
                        workMins = shift.FriMin;
                        break;
                    case DayOfWeek.Saturday:
                        workMins = shift.SatMin;
                        break;
                    case DayOfWeek.Sunday:
                        workMins = shift.SunMin;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return workMins;
        }

        #endregion

    }
}