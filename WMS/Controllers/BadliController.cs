using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using PagedList;

namespace WMS.Controllers
{
    public class BadliController : Controller
    {
        //
        // GET: /Badli/
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            List<VMBadliRecord> brecords = new List<VMBadliRecord>();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            brecords = GetBadliValue();
            brecords = brecords.OrderByDescending(aa => aa.BadliID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {

                brecords = brecords.Where(s => s.EmpName.ToUpper().Contains(searchString.ToUpper())
                    || s.EmpNo == searchString || s.BEmpNo == searchString || s.BEmpName.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(brecords.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s => s.CompName), "CompID", "CompName", LoggedInUser.CompanyID);
            return View();
        }

        [HttpPost]
        public ActionResult Create(string EmpID)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            string EmpNo = Request.Form["EmpNo"];
            string BEmpNo = Request.Form["BEmpNo"];
            string CompanyID = Request.Form["CompanyID"];
            int CompID= Convert.ToInt32(CompanyID);
            string Date = Request.Form["Date"];
            BadliRecordEmp br = new BadliRecordEmp();
            Emp emp = db.Emps.First(aa => aa.EmpNo == EmpNo && aa.CompanyID == CompID);
            Emp BEmp = db.Emps.First(aa => aa.EmpNo == BEmpNo && aa.CompanyID == CompID);
            br.BadliEmpID = BEmp.EmpID;
            br.CreatedDate = DateTime.Now;
            br.Date = Convert.ToDateTime(Date);
            br.EmpID = emp.EmpID;
            br.UserID = LoggedInUser.UserID;
            br.EmpDateBadli = br.EmpID + br.Date.Value.ToString("yyMMdd");
            db.BadliRecordEmps.Add(br);
            db.SaveChanges();
            AddBadliAttData(br.EmpDateBadli, (int)br.EmpID, (DateTime)br.Date);
            ViewBag.CompanyID = new SelectList(db.Companies.OrderBy(s => s.CompName), "CompID", "CompName", LoggedInUser.CompanyID);
            return View();
        }
        public ActionResult Delete(int? id)
        {
            BadliRecordEmp be = new BadliRecordEmp();
            be = db.BadliRecordEmps.First(aa => aa.BadliID == id);
            db.BadliRecordEmps.Remove(be);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private bool AddBadliAttData(string _empDate, int _empID, DateTime _Date)
        {
            bool check = false;
            try
            {
                //Normal Duty
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        _attdata.Remarks = "";
                        _attdata.Remarks = _attdata.Remarks.Replace("[Badli][Manual]", "");
                        _attdata.DutyCode = "D";
                        _attdata.StatusAB = false;
                        _attdata.StatusLeave = false;
                        _attdata.StatusP = true;
                        _attdata.Remarks = _attdata.Remarks + "[Badli][Manual]";
                        _attdata.StatusMN = true;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return check;
        }
        TAS2013Entities db = new TAS2013Entities();
        private List<VMBadliRecord> GetBadliValue()
        {
            List<VMBadliRecord> brs = new List<VMBadliRecord>();
            List<BadliRecordEmp> badli = new List<BadliRecordEmp>();
            badli = db.BadliRecordEmps.ToList();
            List<Emp> emps = new List<Emp>();
            emps = db.Emps.ToList();
            foreach (var item in badli)
            {
                Emp OEmp = emps.First(aa => aa.EmpID == item.EmpID);
                Emp BEmp = emps.First(aa => aa.EmpID == item.BadliEmpID);
                VMBadliRecord br = new VMBadliRecord();
                br.BadliID = item.BadliID;
                br.BDesignation = BEmp.Designation.DesignationName;
                br.BEmpID = (int)item.BadliEmpID;
                br.BEmpName = BEmp.EmpName;
                br.BEmpNo = BEmp.EmpNo;
                br.Dated = (DateTime)item.Date;
                br.Designation = OEmp.Designation.DesignationName;
                br.EmpID = (int)item.EmpID;
                br.EmpName = OEmp.EmpName;
                br.EmpNo = OEmp.EmpNo;
                brs.Add(br);
            }
            return brs;
        }
	}
    public class VMBadliRecord
    {
        public int BadliID { get; set; }
        public int EmpID { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public DateTime Dated { get; set; }
        public int BEmpID { get; set; }
        public string BEmpNo { get; set; }
        public string BEmpName { get; set; }
        public string BDesignation { get; set; }
    }
}