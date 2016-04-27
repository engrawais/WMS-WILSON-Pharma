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
    public class EmailFormController : Controller
    {
        private TAS2013Entities db = new TAS2013Entities();

        // GET: /EmailForm/
        public ActionResult Index()
        {
            var emailentryforms = db.EmailEntryForms.Include(e => e.Category).Include(e => e.Company).Include(e => e.Department).Include(e => e.Location).Include(e => e.Section);
            return View(emailentryforms.ToList());
        }

        // GET: /EmailForm/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            return View(emailentryform);
        }

        // GET: /EmailForm/Create
        public ActionResult Create()
        {
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName");
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName");
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName");
            return View();
        }

        // POST: /EmailForm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,EmailAddress,CCAddress,CompanyID,DepartmentID,SectionID,Criteria,ReportCurrentDate,LocationID,CatID,HasCat,HasLoc")] EmailEntryForm emailentryform)
        {
            if (ModelState.IsValid)
            {
                db.EmailEntryForms.Add(emailentryform);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            return View(emailentryform);
        }

        // GET: /EmailForm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            return View(emailentryform);
        }

        // POST: /EmailForm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,EmailAddress,CCAddress,CompanyID,DepartmentID,SectionID,Criteria,ReportCurrentDate,LocationID,CatID,HasCat,HasLoc")] EmailEntryForm emailentryform)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailentryform).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", emailentryform.CatID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompID", "CompName", emailentryform.CompanyID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DeptID", "DeptName", emailentryform.DepartmentID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocID", "LocName", emailentryform.LocationID);
            ViewBag.SectionID = new SelectList(db.Sections, "SectionID", "SectionName", emailentryform.SectionID);
            return View(emailentryform);
        }

        // GET: /EmailForm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            if (emailentryform == null)
            {
                return HttpNotFound();
            }
            return View(emailentryform);
        }

        // POST: /EmailForm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailEntryForm emailentryform = db.EmailEntryForms.Find(id);
            db.EmailEntryForms.Remove(emailentryform);
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
        public ActionResult GetDepartment(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Departments.Where(aa=>aa.CompanyID==Code).OrderBy(s=>s.DeptName);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "DeptID",
                                "DeptName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
        public ActionResult GetSection(string ID)
        {
            short Code = Convert.ToInt16(ID);
            var secs = db.Sections.Where(aa => aa.CompanyID == Code).OrderBy(s => s.SectionName);
            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(
                                secs.ToArray(),
                                "SectionID",
                                "SectionName")
                           , JsonRequestBehavior.AllowGet);

            return RedirectToAction("Index");
        }
        
    }
}
