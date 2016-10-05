using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Models;
using PagedList;
using System.IO;
using System.Web.Helpers;
using WMS.Controllers.Filters;
using WMS.HelperClass;
using WMS.CustomClass;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
namespace WMS.Controllers
{
    
    public class AttProcessorsController : Controller
    {
        private TAS2013Entities context = new TAS2013Entities();

        //
        // GET: /AttProcessors/

        public ViewResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TagSortParm = String.IsNullOrEmpty(sortOrder) ? "tag_desc" : "";
            ViewBag.FromSortParm = sortOrder == "from" ? "from_desc" : "from";
            ViewBag.ToSortParm = sortOrder == "to" ? "to_desc" : "to";
            ViewBag.WhenToSortParm = sortOrder == "whento" ? "whento_desc" : "whento";
            ViewBag.LocationSortParm = sortOrder == "location" ? "location_desc" : "location";
            ViewBag.CompanySortParm = sortOrder == "company" ? "company_desc" : "company";
            ViewBag.CatSortParm = sortOrder == "cat" ? "cat_desc" : "cat";

            if (searchString != null)
                page = 1;
             else
               searchString = currentFilter;
            DateTime dtE = DateTime.Now - new TimeSpan(4,0,0);
            List<AttProcessorScheduler> attprocess = context.AttProcessorSchedulers.Where(aa=>aa.CreatedDate>=dtE || aa.ProcessingDone==false).OrderByDescending(aa=>aa.CreatedDate).ToList();
            switch (sortOrder)
            {
                case "tag_desc": attprocess = attprocess.OrderByDescending(s => s.PeriodTag).ToList();                   break;
                case "from_desc":
                    attprocess = attprocess.OrderByDescending(s => s.DateFrom).ToList();
                    break;
                case "from":
                   attprocess = attprocess.OrderBy(s => s.DateFrom).ToList();
                    break;
                case "to_desc":
                    attprocess = attprocess.OrderByDescending(s => s.DateTo).ToList();
                    break;
                case "to":
                    attprocess = attprocess.OrderBy(s => s.DateTo).ToList();
                    break;
                case "whento_desc":
                    attprocess = attprocess.OrderByDescending(s => s.WhenToProcess).ToList();
                    break;
                case "whento":
                    attprocess = attprocess.OrderBy(s => s.WhenToProcess).ToList();
                    break;
               
                default:
                    attprocess = attprocess.OrderBy(s => s.PeriodTag).ToList();
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(attprocess.ToPagedList(pageNumber, pageSize));
           
        }

        //
        // GET: /AttProcessors/Details/5

        public ViewResult Details(int id)
        {
            AttProcessorScheduler attprocessor = context.AttProcessorSchedulers.Single(x => x.AttProcesserSchedulerID == id);
            return View(attprocessor);
        }

        //
        // GET: /AttProcessors/Create

        public ActionResult Create()
        {
            TAS2013Entities db = new TAS2013Entities();
            User LoggedInUser = Session["LoggedUser"] as User;
            QueryBuilder qb = new QueryBuilder();
            String query = qb.QueryForCompanyViewLinq(LoggedInUser);
            ViewBag.PeriodTag = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "Daily", Value = "D"},
                new SelectListItem { Selected = false, Text = "Monthly", Value = "M"},
                new SelectListItem { Selected = false, Text = "Summary", Value = "S"},

            }, "Value" , "Text",1);
            ViewBag.CriteriaID = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "Company", Value = "C"},
                new SelectListItem { Selected = false, Text = "Location", Value = "L"},
                new SelectListItem { Selected = false, Text = "Employee", Value = "E"},

            }, "Value", "Text", 1);
            ViewBag.ProcessCats = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "Yes", Value = "1"},
                new SelectListItem { Selected = false, Text = "No", Value = "0"},

            }, "Value", "Text", 1);
            if (LoggedInUser.RoleID == 1)
            {
                ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
                ViewBag.CompanyIDForEmp = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);          
                query = qb.QueryForLocationTableSegerationForLinq(LoggedInUser);
                ViewBag.LocationID = new SelectList(CustomFunction.GetLocations(db.Locations.ToList(), db.UserLocations.Where(aa => aa.UserID == LoggedInUser.UserID).ToList()), "LocID", "LocName");           
              }
            else
            {
                ViewBag.CompanyID = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
                ViewBag.CompanyIDForEmp = new SelectList(CustomFunction.GetCompanies(db.Companies.ToList(), LoggedInUser), "CompID", "CompName", LoggedInUser.CompanyID);
                query = qb.QueryForLocationTableSegerationForLinq(LoggedInUser);
                ViewBag.LocationID = new SelectList(CustomFunction.GetLocations(db.Locations.ToList(), db.UserLocations.Where(aa => aa.UserID == LoggedInUser.UserID).ToList()), "LocID", "LocName");
            }

            
            
            ViewBag.CatID = new SelectList(db.Categories.OrderBy(s=>s.CatName), "CatID", "CatName");
             return View();
        } 

        //
        // POST: /AttProcessors/Create

        [HttpPost]
        public ActionResult Create(AttProcessorScheduler attprocessor)
        {
            string d = Request.Form["CriteriaID"].ToString();
            switch (d)
            {
                case "C":
                    attprocessor.Criteria = "C";
                    break;
                case "L": attprocessor.Criteria = "L"; break;
                case "A": attprocessor.Criteria = "A"; break;
                case "E":
                    {
                        attprocessor.Criteria = "E";
                        attprocessor.ProcessCat = false;
                        string ee = Request.Form["EmpNo"].ToString();
                         int cc = Convert.ToInt16(Request.Form["CompanyIDForEmp"].ToString());
                         List<Emp> empss = new List<Emp>();
                         empss = context.Emps.Where(aa => aa.EmpNo == ee && aa.CompanyID == cc).ToList();
                         if (empss.Count() > 0)
                         {
                             attprocessor.EmpID = empss.First().EmpID;
                             attprocessor.EmpNo = empss.First().EmpNo;
                         }
                    }
                    break;
            }
            int a = Convert.ToInt16(Request.Form["ProcessCats"].ToString());
            if (a == 1)
                attprocessor.ProcessCat = true;
            else
                attprocessor.ProcessCat = false;
            attprocessor.ProcessingDone = false;
            attprocessor.WhenToProcess = DateTime.Today;
            attprocessor.CreatedDate = DateTime.Now;
            int _userID = Convert.ToInt32(Session["LogedUserID"].ToString());
            if (ModelState.IsValid)
            {
                attprocessor.UserID = _userID;
                context.AttProcessorSchedulers.Add(attprocessor);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }
            return View(attprocessor);
        }
        
        //
        // GET: /AttProcessors/Edit/5
 
        public ActionResult Edit(int id)
        {
            AttProcessorScheduler attprocessor = context.AttProcessorSchedulers.Single(x => x.AttProcesserSchedulerID == id);
            return View(attprocessor);
        }

        //
        // POST: /AttProcessors/Edit/5

        [HttpPost]
        public ActionResult Edit(AttProcessorScheduler attprocessor)
        {
            if (ModelState.IsValid)
            {
                context.Entry(attprocessor).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attprocessor);
        }

        //
        // GET: /AttProcessors/Delete/5
 
        public ActionResult Delete(int id)
        {
            AttProcessorScheduler attprocessor = context.AttProcessorSchedulers.Single(x => x.AttProcesserSchedulerID == id);
            return View(attprocessor);
        }

        //
        // POST: /AttProcessors/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            AttProcessorScheduler attprocessor = context.AttProcessorSchedulers.Single(x => x.AttProcesserSchedulerID == id);
            context.AttProcessorSchedulers.Remove(attprocessor);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetEmpInfo(string ID)
        {
            string[] words = ID.Split('w');
            int companyID = Convert.ToInt16(words[1]);
            string EmpNo = words[0];
            List<Emp> emp = context.Emps.Where(aa => aa.CompanyID == companyID && aa.EmpNo == EmpNo).ToList();
            if (emp.Count > 0)
            {
                if (HttpContext.Request.IsAjaxRequest())
                    return Json(emp.FirstOrDefault().EmpName + "@" + emp.FirstOrDefault().Designation.DesignationName + "@" + emp.FirstOrDefault().Section.SectionName
                           , JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (HttpContext.Request.IsAjaxRequest())
                    return Json("NotFound"
                           , JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}