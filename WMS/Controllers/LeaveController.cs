﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;

namespace WMS.Controllers
{
    public class LeaveController
    {
        #region -- Add Leaves--
        List<LvQuota> _empQuota = new List<LvQuota>();
        public string AddLeavesToLvApplication()
        {
            string error = "";

            return error;
        }

        //Check Duplication of Leave for a date
        public bool CheckDuplicateLeave(LvApplication lvappl)
        {
            List<LvApplication> _Lv = new List<LvApplication>();
            DateTime _DTime = new DateTime();
            DateTime _DTimeLV = new DateTime();
            using (var context = new TAS2013Entities())
            {
                _Lv = context.LvApplications.Where(aa => aa.EmpID == lvappl.EmpID).ToList();
                foreach (var item in _Lv)
                {
                    _DTime = item.FromDate;
                    _DTimeLV = lvappl.FromDate;
                    while (_DTime <= item.ToDate)
                    {
                        while (_DTimeLV <= lvappl.ToDate)
                        {
                            if (_DTime == _DTimeLV)
                                return false;
                            _DTimeLV = _DTimeLV.AddDays(1);
                        }
                        _DTime = _DTime.AddDays(1);
                    }
                }
            }
            return true;
        }

        public bool CheckLeaveBalance(LvApplication _lvapp)
        {
            bool balance = false;
            decimal RemainingLeaves;
            using (var ctx = new TAS2013Entities())
            {
                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                string empLvType = _lvapp.EmpID.ToString() + _lvapp.LvType+ DateTime.Today.Year.ToString();;
                if (ctx.LvTypes.Where(aa => aa.LvType1 == _lvapp.LvType).FirstOrDefault().UpdateBalance == true)
                {
                    _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                    if (_lvConsumed.Count > 0)
                    {
                        if (_lvConsumed.FirstOrDefault().YearRemaining != null)
                        {
                            RemainingLeaves = (decimal)_lvConsumed.FirstOrDefault().YearRemaining;
                            if ((RemainingLeaves - Convert.ToDecimal(_lvapp.NoOfDays)) >= 0)
                            {
                                balance = true;
                            }
                            else
                                balance = false;
                        }
                        else
                        {
                            balance = false;
                            _lvConsumed.FirstOrDefault().YearRemaining = 0;
                            _lvConsumed.FirstOrDefault().TotalForYear = 0;
                            ctx.SaveChanges();

                        }
                    }
                    else
                        balance = false;
                }
                else
                    balance = true;
            }

            return balance;

        }

        public bool AddLeaveToLeaveAttData(LvApplication lvappl)
        {
            try
            {
                DateTime datetime = new DateTime();
                datetime = lvappl.FromDate;
                for (int i = 0; i < lvappl.NoOfDays; i++)
                {
                    string _EmpDate = lvappl.EmpID + datetime.Date.ToString("yyMMdd");
                    using (var context = new TAS2013Entities())
                    {
                        if (context.AttProcesses.Where(aa => aa.ProcessDate == datetime).Count() > 0)
                        {
                            AttData _EmpAttData = new AttData();
                            _EmpAttData = context.AttDatas.First(aa => aa.EmpDate == _EmpDate);
                            _EmpAttData.TimeIn = null;
                            _EmpAttData.TimeOut = null;
                            _EmpAttData.WorkMin = null;
                            _EmpAttData.LateIn = null;
                            _EmpAttData.LateOut = null;
                            _EmpAttData.EarlyIn = null;
                            _EmpAttData.EarlyOut = null;
                            _EmpAttData.StatusAB= false;
                            _EmpAttData.StatusLeave = true;
                            _EmpAttData.StatusEI = null;
                            _EmpAttData.StatusEO = null;
                            _EmpAttData.StatusLI = null;
                            _EmpAttData.StatusLI= null;
                            _EmpAttData.StatusLO = null;
                            _EmpAttData.StatusDO = null;
                                _EmpAttData.StatusGZ = null;
                            _EmpAttData.StatusGZOT = null;
                            _EmpAttData.StatusMN = null;
                            _EmpAttData.StatusOD = null;
                            if (lvappl.LvType == "A")//Casual Leave
                                _EmpAttData.Remarks = "[CL]";
                            if (lvappl.LvType == "B")//Anual Leave
                                _EmpAttData.Remarks = "[AL]";
                            if (lvappl.LvType == "C")//Sick Leave
                                _EmpAttData.Remarks = "[SL]";
                            _EmpAttData.StatusAB = false;
                            _EmpAttData.StatusLeave = true;
                            context.SaveChanges();
                        }
                    }
                    datetime = datetime.AddDays(1);
                }
            }
            catch (Exception  ex)
            {
                return false;
            }
            return true;

        }

        public bool AddLeaveToLeaveData(LvApplication lvappl)
        {
            DateTime datetime = new DateTime();
            datetime = lvappl.FromDate;
            for (int i = 0; i < lvappl.NoOfDays; i++)
            {
                string _EmpDate = lvappl.EmpID + datetime.Date.ToString("yyMMdd");
                LvData _LVData = new LvData();
                _LVData.EmpID = lvappl.EmpID;
                _LVData.EmpDate = _EmpDate;
                _LVData.Remarks = lvappl.LvReason;
                _LVData.LvID = lvappl.LvID;
                _LVData.AttDate = datetime.Date;
                _LVData.LvCode = lvappl.LvType;
                _LVData.CompanyID = lvappl.CompanyID;
                try
                {
                    using (var context = new TAS2013Entities())
                    {
                        context.LvDatas.Add(_LVData);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
                datetime = datetime.AddDays(1);
                // Balance Leaves from Emp Table
            }
            BalanceLeaves(lvappl);
            return true;
        }

        public void BalanceLeaves(LvApplication lvappl)
        {
            using (var ctx = new TAS2013Entities())
            {
                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                string empLvType = lvappl.EmpID.ToString() + lvappl.LvType+DateTime.Today.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                if (ctx.LvTypes.Where(aa => aa.LvType1 == lvappl.LvType).FirstOrDefault().UpdateBalance == true)
                {
                    float _NoOfDays = lvappl.NoOfDays;

                    if (_lvConsumed.Count > 0)
                    {
                        _lvConsumed.FirstOrDefault().YearRemaining = (float)(_lvConsumed.FirstOrDefault().YearRemaining - _NoOfDays);
                        _lvConsumed.FirstOrDefault().GrandTotalRemaining = (float)(_lvConsumed.FirstOrDefault().GrandTotalRemaining - _NoOfDays);
                        if (lvappl.IsHalf == true)
                        {
                            AddHalfLeaveBalancceMonthQuota(_lvConsumed, lvappl);
                        }
                        else
                        {
                            AddBalancceMonthQuota(_lvConsumed, lvappl);
                        }
                        ctx.SaveChanges();
                    }
                }
                ctx.Dispose();
            }
        }

        private void AddBalancceMonthQuota(List<LvConsumed> _lvConsumed, LvApplication lvappl)
        {
            DateTime _dtStart = lvappl.FromDate;
            DateTime _dtEnd = lvappl.ToDate;
            while(_dtStart<=_dtEnd)
            {
                switch (_dtStart.Date.Month)
                {
                    case 1:
                        _lvConsumed.FirstOrDefault().JanConsumed = _lvConsumed.FirstOrDefault().JanConsumed + 1;
                        break;
                    case 2:
                        _lvConsumed.FirstOrDefault().FebConsumed = _lvConsumed.FirstOrDefault().FebConsumed + 1;
                        break;
                    case 3:
                        _lvConsumed.FirstOrDefault().MarchConsumed = _lvConsumed.FirstOrDefault().MarchConsumed + 1;
                        break;
                    case 4:
                        _lvConsumed.FirstOrDefault().AprConsumed = _lvConsumed.FirstOrDefault().AprConsumed + 1;
                        break;
                    case 5:
                        _lvConsumed.FirstOrDefault().MayConsumed = _lvConsumed.FirstOrDefault().MayConsumed + 1;
                        break;
                    case 6:
                        _lvConsumed.FirstOrDefault().JuneConsumed = _lvConsumed.FirstOrDefault().JuneConsumed + 1;
                        break;
                    case 7:
                        _lvConsumed.FirstOrDefault().JulyConsumed = _lvConsumed.FirstOrDefault().JulyConsumed + 1;
                        break;
                    case 8:
                        _lvConsumed.FirstOrDefault().AugustConsumed = _lvConsumed.FirstOrDefault().AugustConsumed + 1;
                        break;
                    case 9:
                        _lvConsumed.FirstOrDefault().SepConsumed = _lvConsumed.FirstOrDefault().SepConsumed + 1;
                        break;
                    case 10:
                        _lvConsumed.FirstOrDefault().OctConsumed = _lvConsumed.FirstOrDefault().OctConsumed + 1;
                        break;
                    case 11:
                        _lvConsumed.FirstOrDefault().NovConsumed = _lvConsumed.FirstOrDefault().NovConsumed + 1;
                        break;
                    case 12:
                        _lvConsumed.FirstOrDefault().DecConsumed = _lvConsumed.FirstOrDefault().DecConsumed + 1;
                        break;

                }
                _dtStart = _dtStart.AddDays(1);
            }
        }

        private void AddHalfLeaveBalancceMonthQuota(List<LvConsumed> _lvConsumed, LvApplication lvappl)
        {
            DateTime _dtStart = lvappl.FromDate;
            DateTime _dtEnd = lvappl.ToDate;
            while (_dtStart <= _dtEnd)
            {
                switch (_dtStart.Date.Month)
                {
                    case 1:
                        _lvConsumed.FirstOrDefault().JanConsumed = _lvConsumed.FirstOrDefault().JanConsumed + 0.5;
                        break;
                    case 2:
                        _lvConsumed.FirstOrDefault().FebConsumed = _lvConsumed.FirstOrDefault().FebConsumed + 0.5;
                        break;
                    case 3:
                        _lvConsumed.FirstOrDefault().MarchConsumed = _lvConsumed.FirstOrDefault().MarchConsumed + 0.5;
                        break;
                    case 4:
                        _lvConsumed.FirstOrDefault().AprConsumed = _lvConsumed.FirstOrDefault().AprConsumed + 0.5;
                        break;
                    case 5:
                        _lvConsumed.FirstOrDefault().MayConsumed = _lvConsumed.FirstOrDefault().MayConsumed + 0.5;
                        break;
                    case 6:
                        _lvConsumed.FirstOrDefault().JuneConsumed = _lvConsumed.FirstOrDefault().JuneConsumed + 0.5;
                        break;
                    case 7:
                        _lvConsumed.FirstOrDefault().JulyConsumed = _lvConsumed.FirstOrDefault().JulyConsumed + 0.5;
                        break;
                    case 8:
                        _lvConsumed.FirstOrDefault().AugustConsumed = _lvConsumed.FirstOrDefault().AugustConsumed + 0.5;
                        break;
                    case 9:
                        _lvConsumed.FirstOrDefault().SepConsumed = _lvConsumed.FirstOrDefault().SepConsumed + 0.5;
                        break;
                    case 10:
                        _lvConsumed.FirstOrDefault().OctConsumed = _lvConsumed.FirstOrDefault().OctConsumed + 0.5;
                        break;
                    case 11:
                        _lvConsumed.FirstOrDefault().NovConsumed = _lvConsumed.FirstOrDefault().NovConsumed + 0.5;
                        break;
                    case 12:
                        _lvConsumed.FirstOrDefault().DecConsumed = _lvConsumed.FirstOrDefault().DecConsumed + 0.5;
                        break;

                }
                _dtStart = _dtStart.AddDays(1);
            }
        }

        private void SubtractBalancceMonthQuota(List<LvConsumed> _lvConsumed, LvApplication lvappl)
        {
            DateTime _dtStart = lvappl.FromDate;
            DateTime _dtEnd = lvappl.ToDate;
            while (_dtStart <= _dtEnd)
            {
                switch (_dtStart.Date.Month)
                {
                    case 1:
                        _lvConsumed.FirstOrDefault().JanConsumed = _lvConsumed.FirstOrDefault().JanConsumed - 1;
                        break;
                    case 2:
                        _lvConsumed.FirstOrDefault().FebConsumed = _lvConsumed.FirstOrDefault().FebConsumed - 1;
                        break;
                    case 3:
                        _lvConsumed.FirstOrDefault().MarchConsumed = _lvConsumed.FirstOrDefault().MarchConsumed - 1;
                        break;
                    case 4:
                        _lvConsumed.FirstOrDefault().AprConsumed = _lvConsumed.FirstOrDefault().AprConsumed - 1;
                        break;
                    case 5:
                        _lvConsumed.FirstOrDefault().MayConsumed = _lvConsumed.FirstOrDefault().MayConsumed - 1;
                        break;
                    case 6:
                        _lvConsumed.FirstOrDefault().JuneConsumed = _lvConsumed.FirstOrDefault().JuneConsumed - 1;
                        break;
                    case 7:
                        _lvConsumed.FirstOrDefault().JulyConsumed = _lvConsumed.FirstOrDefault().JulyConsumed - 1;
                        break;
                    case 8:
                        _lvConsumed.FirstOrDefault().AugustConsumed = _lvConsumed.FirstOrDefault().AugustConsumed - 1;
                        break;
                    case 9:
                        _lvConsumed.FirstOrDefault().SepConsumed = _lvConsumed.FirstOrDefault().SepConsumed - 1;
                        break;
                    case 10:
                        _lvConsumed.FirstOrDefault().OctConsumed = _lvConsumed.FirstOrDefault().OctConsumed - 1;
                        break;
                    case 11:
                        _lvConsumed.FirstOrDefault().NovConsumed = _lvConsumed.FirstOrDefault().NovConsumed - 1;
                        break;
                    case 12:
                        _lvConsumed.FirstOrDefault().DecConsumed = _lvConsumed.FirstOrDefault().DecConsumed - 1;
                        break;

                }
                _dtStart = _dtStart.AddDays(1);
            }
        }
        private void SubtractHalfLeavesBalancceMonthQuota(List<LvConsumed> _lvConsumed, LvApplication lvappl)
        {
            DateTime _dtStart = lvappl.FromDate;
            DateTime _dtEnd = lvappl.ToDate;
            while (_dtStart <= _dtEnd)
            {
                switch (_dtStart.Date.Month)
                {
                    case 1:
                        _lvConsumed.FirstOrDefault().JanConsumed = _lvConsumed.FirstOrDefault().JanConsumed - 0.5;
                        break;
                    case 2:
                        _lvConsumed.FirstOrDefault().FebConsumed = _lvConsumed.FirstOrDefault().FebConsumed - 0.5;
                        break;
                    case 3:
                        _lvConsumed.FirstOrDefault().MarchConsumed = _lvConsumed.FirstOrDefault().MarchConsumed - 0.5;
                        break;
                    case 4:
                        _lvConsumed.FirstOrDefault().AprConsumed = _lvConsumed.FirstOrDefault().AprConsumed - 0.5;
                        break;
                    case 5:
                        _lvConsumed.FirstOrDefault().MayConsumed = _lvConsumed.FirstOrDefault().MayConsumed - 0.5;
                        break;
                    case 6:
                        _lvConsumed.FirstOrDefault().JuneConsumed = _lvConsumed.FirstOrDefault().JuneConsumed - 0.5;
                        break;
                    case 7:
                        _lvConsumed.FirstOrDefault().JulyConsumed = _lvConsumed.FirstOrDefault().JulyConsumed - 0.5;
                        break;
                    case 8:
                        _lvConsumed.FirstOrDefault().AugustConsumed = _lvConsumed.FirstOrDefault().AugustConsumed - 0.5;
                        break;
                    case 9:
                        _lvConsumed.FirstOrDefault().SepConsumed = _lvConsumed.FirstOrDefault().SepConsumed - 0.5;
                        break;
                    case 10:
                        _lvConsumed.FirstOrDefault().OctConsumed = _lvConsumed.FirstOrDefault().OctConsumed - 0.5;
                        break;
                    case 11:
                        _lvConsumed.FirstOrDefault().NovConsumed = _lvConsumed.FirstOrDefault().NovConsumed - 0.5;
                        break;
                    case 12:
                        _lvConsumed.FirstOrDefault().DecConsumed = _lvConsumed.FirstOrDefault().DecConsumed - 0.5;
                        break;

                }
                _dtStart = _dtStart.AddDays(1);
            }
        }

        //public void BalanceLeaves(LvApplication lvappl)
        //{
        //    _empQuota.Clear();
        //    using (var context = new TAS2013Entities())
        //    {
        //        _empQuota = context.LvQuotas.Where(aa => aa.EmpID == lvappl.EmpID).ToList();
        //        float _NoOfDays = lvappl.NoOfDays;
        //        if (_empQuota.Count > 0)
        //        {
        //            switch (lvappl.LvType)
        //            {
        //                case "A":
        //                    _empQuota.FirstOrDefault().A = (float)(_empQuota.FirstOrDefault().A - _NoOfDays);
        //                    break;
        //                case "B":
        //                    _empQuota.FirstOrDefault().B = (float)(_empQuota.FirstOrDefault().B - _NoOfDays);
        //                    break;
        //                case "C":
        //                    _empQuota.FirstOrDefault().C = (float)(_empQuota.FirstOrDefault().C - _NoOfDays);
        //                    break;
        //                case "D":
        //                    _empQuota.FirstOrDefault().D = (float)(_empQuota.FirstOrDefault().D - _NoOfDays);
        //                    break;
        //                case "E":
        //                    _empQuota.FirstOrDefault().E = (float)(_empQuota.FirstOrDefault().E - _NoOfDays);
        //                    break;
        //                case "F":
        //                    _empQuota.FirstOrDefault().F = (float)(_empQuota.FirstOrDefault().F - _NoOfDays);
        //                    break;
        //                case "G":
        //                    _empQuota.FirstOrDefault().G = (float)(_empQuota.FirstOrDefault().G - _NoOfDays);
        //                    break;
        //                case "H":
        //                    _empQuota.FirstOrDefault().H = (float)(_empQuota.FirstOrDefault().H - _NoOfDays);
        //                    break;
        //                case "I":
        //                    _empQuota.FirstOrDefault().I = (float)(_empQuota.FirstOrDefault().I - _NoOfDays);
        //                    break;
        //                case "J":
        //                    _empQuota.FirstOrDefault().J = (float)(_empQuota.FirstOrDefault().J - _NoOfDays);
        //                    break;
        //                case "K":
        //                    _empQuota.FirstOrDefault().K = (float)(_empQuota.FirstOrDefault().K - _NoOfDays);
        //                    break;
        //                case "L":
        //                    _empQuota.FirstOrDefault().L = (float)(_empQuota.FirstOrDefault().L - _NoOfDays);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //}

        #endregion

        #region -- Add Half Leave--

        public void AddHalfLeaveToLeaveData(LvApplication lvappl)
        {
            DateTime datetime = new DateTime();
            datetime = lvappl.FromDate;
            string _EmpDate = lvappl.EmpID + datetime.Date.ToString("yyMMdd");
            LvData _LVData = new LvData();
            _LVData.EmpID = lvappl.EmpID;
            _LVData.EmpDate = _EmpDate;
            _LVData.Remarks = lvappl.LvReason;
            _LVData.HalfLeave = true;
            _LVData.LvID = lvappl.LvID;
            _LVData.AttDate = datetime.Date;
            _LVData.LvCode = lvappl.LvType;
            _LVData.FirstHalf = lvappl.FirstHalf;
            try
            {
                using (var db = new TAS2013Entities())
                {
                    db.LvDatas.Add(_LVData);
                    db.SaveChanges(); 
                }
            }
            catch (Exception ex)
            {

            }
            // Balance Leaves from Emp Table
            BalanceLeaves(lvappl);
        }

        public void AddHalfLeaveToAttData(LvApplication lvappl)
        {
            DateTime datetime = new DateTime();
            datetime = lvappl.FromDate;
            string _EmpDate = lvappl.EmpID + datetime.Date.ToString("yyMMdd");
            using (var db = new TAS2013Entities())
            {
                if (db.AttProcesses.Where(aa => aa.ProcessDate == datetime).Count() > 0)
                {
                    AttData _EmpAttData = new AttData();
                    _EmpAttData = db.AttDatas.First(aa => aa.EmpDate == _EmpDate);
                    if (lvappl.LvType == "A")//Casual Leave
                        _EmpAttData.Remarks = _EmpAttData.Remarks+"[H-CL]";
                    if (lvappl.LvType == "B")//Anual Leave
                        _EmpAttData.Remarks = _EmpAttData.Remarks+"[H-AL]";
                    if (lvappl.LvType == "C")//Sick Leave
                        _EmpAttData.Remarks = _EmpAttData.Remarks+"[H-SL]";

                    if (_EmpAttData.Remarks.Contains("[Absent]"))
                        _EmpAttData.Remarks.Replace("[Abesnt]", "");
                    if (_EmpAttData.Remarks.Contains("[EO]"))
                        _EmpAttData.Remarks.Replace("[EO]", "-");
                    if (_EmpAttData.Remarks.Contains("[LI]"))
                        _EmpAttData.Remarks.Replace("[LI]", "");
                    _EmpAttData.StatusLeave = true;
                    if (lvappl.FirstHalf == true)
                    {
                        _EmpAttData.LateIn = 0;
                        _EmpAttData.StatusLI = false;
                    }
                    else
                    {
                        _EmpAttData.StatusEO = false;
                        _EmpAttData.EarlyOut = 0;
                    }
                    //_EmpAttData.statushl


                    db.SaveChanges();
                } 
            }
        }

        public bool CheckHalfLeaveBalance(LvApplication lvapplication)
        {
            bool check = false;
            float RemainingLeaves;
            List<LvConsumed> _lvConsumed = new List<LvConsumed>();
            using (var ctx = new TAS2013Entities())
            {
                string empLvType = lvapplication.EmpID.ToString() + lvapplication.LvType+DateTime.Today.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                if (_lvConsumed.Count > 0)
                {
                    RemainingLeaves = (float)_lvConsumed.FirstOrDefault().YearRemaining;
                     if ((RemainingLeaves - (float)lvapplication.NoOfDays) >= 0)
                    {
                        check =true;
                    }
                    else
                        check= false;
                }
            }
            return check;

        }

        //public bool CheckHalfLeaveBalance(LvApplication lvapplication)
        //{
        //    _empQuota.Clear();
        //    int _EmpID;
        //    float RemainingLeaves;
        //    _EmpID = lvapplication.EmpID;
        //    using (var context = new TAS2013Entities())
        //    {
        //        _empQuota = context.LvQuotas.Where(aa => aa.EmpID == _EmpID).ToList();
        //        if (_empQuota.Count > 0)
        //        {
        //            switch (lvapplication.LvType)
        //            {
        //                case "A":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().A;
        //                    break;
        //                case "B":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().B;
        //                    break;
        //                case "C":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().C;
        //                    break;
        //                case "D":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().D;
        //                    break;
        //                case "E":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().E;
        //                    break;
        //                case "F":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().F;
        //                    break;
        //                case "G":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().G;
        //                    break;
        //                case "H":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().H;
        //                    break;
        //                case "I":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().I;
        //                    break;
        //                case "J":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().J;
        //                    break;
        //                case "K":
        //                    RemainingLeaves = (float)_empQuota.FirstOrDefault().K;
        //                    break;
        //                default:
        //                    RemainingLeaves = 1;
        //                    break;
        //            }
        //            if ((RemainingLeaves - (float)lvapplication.NoOfDays) >= 0)
        //            {
        //                return true;
        //            }
        //            else
        //                return false;
        //        }
        //        else
        //            return false;

        //    }
        //}

        #endregion

        #region -- Delete Leaves --
        public void DeleteFromLVData(LvApplication lvappl)
        {
            int _EmpID = lvappl.EmpID;
            DateTime Date = lvappl.FromDate;
            while (Date <= lvappl.ToDate)
            {
                using (var context = new TAS2013Entities())
                {
                    var _id = context.LvDatas.Where(aa => aa.EmpID == _EmpID && aa.AttDate == Date.Date).FirstOrDefault().EmpDate;
                    if (_id != null)
                    {
                        LvData lvvdata = context.LvDatas.Find(_id);
                        //lvvdata.Active = false;
                        context.LvDatas.Remove(lvvdata);
                        context.SaveChanges();
                    }
                }
                Date = Date.AddDays(1);
            }
        }

        public void DeleteLeaveFromAttData(LvApplication lvappl)
        {
            try
            {
                int _EmpID = lvappl.EmpID;
                DateTime Date = lvappl.FromDate;
                while (Date <= lvappl.ToDate)
                {
                    using (var context = new TAS2013Entities())
                    {
                        if (context.AttProcesses.Where(aa => aa.ProcessDate == Date.Date).Count() > 0)
                        {
                            string _empdate = _EmpID.ToString() + Date.Date.ToString("yyMMdd");
                            var _AttData = context.AttDatas.Where(aa => aa.EmpDate == _empdate);
                            if (_AttData != null)
                            {
                                _AttData.FirstOrDefault().StatusLeave = false;
                                _AttData.FirstOrDefault().Remarks.Replace("[SL]", "");
                                _AttData.FirstOrDefault().Remarks.Replace("[CL]", "");
                                _AttData.FirstOrDefault().Remarks.Replace("[AL]", "");
                                if (_AttData.FirstOrDefault().TimeIn == null && _AttData.FirstOrDefault().TimeOut == null && _AttData.FirstOrDefault().DutyCode == "D")
                                {
                                    _AttData.FirstOrDefault().Remarks = "[Absent]";
                                    _AttData.FirstOrDefault().StatusAB = true;
                                }
                                //context.LvDatas.Remove(lvvdata);
                                context.SaveChanges();
                            }
                        }
                    }
                    Date = Date.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void UpdateLeaveBalance(LvApplication lvappl)
        {
            float LvDays = (float)lvappl.NoOfDays;
            List<LvConsumed> _lvConsumed = new List<LvConsumed>();
            using (var ctx = new TAS2013Entities())
            {
                string empLvType = lvappl.EmpID.ToString() + lvappl.LvType + DateTime.Today.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                if (_lvConsumed.Count > 0)
                {
                    _lvConsumed.FirstOrDefault().YearRemaining = (float)(_lvConsumed.FirstOrDefault().YearRemaining + LvDays);
                    _lvConsumed.FirstOrDefault().GrandTotalRemaining = (float)(_lvConsumed.FirstOrDefault().GrandTotalRemaining + LvDays);
                    SubtractBalancceMonthQuota(_lvConsumed, lvappl);
                    ctx.SaveChanges();
                }
                ctx.Dispose();
            }
        }

        #endregion

        #region -- Delete Half Leaves --
        public void DeleteHLFromLVData(LvApplication lvappl)
        {
            int _EmpID = lvappl.EmpID;
            DateTime Date = lvappl.FromDate;
            while (Date <= lvappl.ToDate)
            {
                using (var context = new TAS2013Entities())
                {
                    var _id = context.LvDatas.Where(aa => aa.EmpID == _EmpID && aa.AttDate == Date.Date).FirstOrDefault().EmpDate;
                    if (_id != null)
                    {
                        LvData lvvdata = context.LvDatas.Find(_id);
                        //lvvdata.Active = false;
                        context.LvDatas.Remove(lvvdata);
                        context.SaveChanges();
                    }
                }
                Date = Date.AddDays(1);
            }
        }

        public void DeleteHLFromAttData(LvApplication lvappl)
        {
            try
            {
                int _EmpID = lvappl.EmpID;
                DateTime Date = lvappl.FromDate;
                while (Date <= lvappl.ToDate)
                {
                    using (var context = new TAS2013Entities())
                    {
                        if (context.AttProcesses.Where(aa => aa.ProcessDate == Date.Date).Count() > 0)
                        {
                            string _empdate = _EmpID.ToString() + Date.Date.ToString("yyMMdd");
                            var _AttData = context.AttDatas.Where(aa => aa.EmpDate == _empdate);
                            if (_AttData != null)
                            {
                                _AttData.FirstOrDefault().StatusLeave = false;
                                _AttData.FirstOrDefault().Remarks.Replace("[H-SL]", "");
                                _AttData.FirstOrDefault().Remarks.Replace("[H-CL]", "");
                                _AttData.FirstOrDefault().Remarks.Replace("[H-AL]", "");
                                if (_AttData.FirstOrDefault().TimeIn == null && _AttData.FirstOrDefault().TimeOut == null && _AttData.FirstOrDefault().DutyCode == "D")
                                {
                                    _AttData.FirstOrDefault().Remarks = "[Absent]";
                                    _AttData.FirstOrDefault().StatusAB = true;
                                }
                                context.SaveChanges();
                            }
                        }
                    }
                    Date = Date.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void UpdateHLeaveBalance(LvApplication lvappl)
        {
            float LvDays = (float)lvappl.NoOfDays;
            List<LvConsumed> _lvConsumed = new List<LvConsumed>();
            using (var ctx = new TAS2013Entities())
            {
                string empLvType = lvappl.EmpID.ToString() + lvappl.LvType + DateTime.Today.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                if (_lvConsumed.Count > 0)
                {
                    _lvConsumed.FirstOrDefault().YearRemaining = (float)(_lvConsumed.FirstOrDefault().YearRemaining + LvDays);
                    _lvConsumed.FirstOrDefault().GrandTotalRemaining = (float)(_lvConsumed.FirstOrDefault().GrandTotalRemaining + LvDays);
                    SubtractHalfLeavesBalancceMonthQuota(_lvConsumed, lvappl);
                    ctx.SaveChanges();
                }
                ctx.Dispose();
            }
        }

        #endregion

        #region -- Add Short Leave --
        public void AddShortLeaveToAttData(LvShort lvshort)
        {
            DateTime datetime = new DateTime();
            using (var db = new TAS2013Entities())
            {
                if (db.AttProcesses.Where(aa => aa.ProcessDate == datetime).Count() > 0)
                {
                    AttData _EmpAttData = new AttData();
                    _EmpAttData = db.AttDatas.First(aa => aa.EmpDate == lvshort.EmpDate);
                    _EmpAttData.StatusAB = false;
                    _EmpAttData.StatusSL = true;
                    _EmpAttData.Remarks = _EmpAttData.Remarks + "[Short Leave]";
                    db.SaveChanges();
                }
            }
        }
        #endregion

        public bool HasLeaveQuota(int empID, string lvType)
        {

            bool check = false;
            using (var ctx = new TAS2013Entities())
            {
                LvType LeaveType = ctx.LvTypes.First(aa => aa.LvType1 == lvType);
                if (LeaveType.UpdateBalance == true)
                {
                    List<LvConsumed> lv = new List<LvConsumed>();
                    string empLvType = empID.ToString() + lvType + DateTime.Today.Year.ToString();
                    lv = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                    if (lv.Count > 0)
                        check = true;
                }
                else
                    check = true;
            }
            return check;
        }
    }
}