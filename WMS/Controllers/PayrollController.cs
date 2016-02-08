using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;

namespace WMS.Controllers
{
    public class PayrollController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /Payroll/
        public ActionResult Index()
        {
            return View(db.PayRollPrimaries.ToList());
        }

        // GET: /Payroll/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayRollPrimary payrollprimary = db.PayRollPrimaries.Find(id);
            if (payrollprimary == null)
            {
                return HttpNotFound();
            }
            return View(payrollprimary);
        }

        // GET: /Payroll/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Payroll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="EmpMonthYear,CompanyID,MonthYear,StartDate,EndDate,EmpNo,TotalDays,EarnedDays,RestDays,AbsentDays,SickLeaveDays,AnnualLeaveDays,CasualDays,OtherLeaveDays,LateInMins,OverTimeMins,EmpID,AnnualLVEncashmentDays,BadliPNo,BadliCompanyID,BadliDays,PreparedBy,PreparedDate,Approved,CreatedDate,IsEdited,TransferedToOracle,TransferedDate")] PayRollPrimary payrollprimary)
        {
            if (ModelState.IsValid)
            {
                db.PayRollPrimaries.Add(payrollprimary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(payrollprimary);
        }

        // GET: /Payroll/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayRollPrimary payrollprimary = db.PayRollPrimaries.Find(id);
            if (payrollprimary == null)
            {
                return HttpNotFound();
            }
            return View(payrollprimary);
        }

        // POST: /Payroll/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="EmpMonthYear,CompanyID,MonthYear,StartDate,EndDate,EmpNo,TotalDays,EarnedDays,RestDays,AbsentDays,SickLeaveDays,AnnualLeaveDays,CasualDays,OtherLeaveDays,LateInMins,OverTimeMins,EmpID,AnnualLVEncashmentDays,BadliPNo,BadliCompanyID,BadliDays,PreparedBy,PreparedDate,Approved,CreatedDate,IsEdited,TransferedToOracle,TransferedDate")] PayRollPrimary payrollprimary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payrollprimary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payrollprimary);
        }

        // GET: /Payroll/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayRollPrimary payrollprimary = db.PayRollPrimaries.Find(id);
            if (payrollprimary == null)
            {
                return HttpNotFound();
            }
            return View(payrollprimary);
        }

        // POST: /Payroll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PayRollPrimary payrollprimary = db.PayRollPrimaries.Find(id);
            db.PayRollPrimaries.Remove(payrollprimary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
