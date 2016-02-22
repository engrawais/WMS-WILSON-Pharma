using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.CustomClass;
using WMS.HelperClass;
using WMS.Models;
using WMSLibrary;

namespace WMS.Reports
{
    public partial class ReportContainer : System.Web.UI.Page
    {
        String title = "";
        string _dateFrom = "";
        List<EmpPhoto> companyimage = new List<EmpPhoto>();
        FiltersModel fm;
        protected void Page_Load(object sender, EventArgs e)
        {
            String reportName = Request.QueryString["reportname"];
            String type = Request.QueryString["type"];
            if (!Page.IsPostBack)
            {
                List<string> list = Session["ReportSession"] as List<string>;
                fm = new FiltersModel();
                fm = Session["FiltersModel"] as FiltersModel;
                CreateDataTable();
                CreateFlexyMonthlyDataTable();
                CreateEmpSumTimeDatatable();
                User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
                QueryBuilder qb = new QueryBuilder();
                string query = qb.MakeCustomizeQuery(LoggedInUser);
                _dateFrom = list[0];
                string _dateTo = list[1];
                companyimage = GetCompanyImages(fm);
                string PathString = "";
                string consolidatedMonth = "";
                switch (reportName)
                {
                    case "monthly_productivity":
                        monthlyProductivityProcess(_dateFrom,_dateTo,query);

                           break;
                    case "badli_report":
                        DataTable badlidt = qb.GetValuesfromDB("select * from ViewBadli where (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        List<ViewBadli> _BadliList = badlidt.ToList<ViewBadli>();
                        List<ViewBadli> _TempBadliList = new List<ViewBadli>();
                        title = "Badli Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/BadliReport.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/BadliReport.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempBadliList, _BadliList), _dateFrom + " TO " + _dateTo);
                        break;
                    //please modify the first case 
                    case "summarized_monthly_report":
                        List<TASReportDataSet.SummarizedMonthlyReportDataTable> AttDeptdummy = new List<TASReportDataSet.SummarizedMonthlyReportDataTable>().ToList();
                        title = "Department Attendace Summary";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/SummarizedMonthlyReport.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/SummarizedMonthlyReport.rdlc";

                        LoadReport(PathString, AttDeptdummy, _dateFrom + " TO " + _dateTo);

                        break;
                    case "Employee_Att_Summary_New_report": DataTable dt4  = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        List<ViewAttData> ListOfAttDate = new List<ViewAttData>();
                        List<ViewAttData> TempList = new List<ViewAttData>();
                        title = "Employee Attendace Summary New";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummaryNew.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummaryNew.rdlc";
                        ListOfAttDate = dt4.ToList<ViewAttData>();
                        TempList = new List<ViewAttData>();
                        
                        LoadReport(PathString, ReportsFilterImplementation(fm, TempList, ListOfAttDate), _dateFrom + " TO " + _dateTo);
                        

                        break;
                    case "department_attendance_summary": HRReportsMaker hrm = new HRReportsMaker();
                        List<AttDeptSummary> AttDept = hrm.GetListForAttDepartmentsSummary(Session["FiltersModel"] as FiltersModel, _dateFrom, _dateTo);
                        title = "Department Attendace Summary";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/AttDepartmentSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/AttDepartmentSummary.rdlc";

                        LoadReport(PathString, AttDept, _dateFrom + " TO " + _dateTo);

                        break;
                    #region --- Employee Record ---
                    case "emp_record": DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        List<EmpView> _ViewList = dt.ToList<EmpView>();
                        List<EmpView> _TempViewList = new List<EmpView>();
                        title = "Employee Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record_active": dt = qb.GetValuesfromDB("select * from EmpView " + query+ " and Status=1 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Active Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record_inactive": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=0 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Inactive Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_detail_excel": DataTable dt1 = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        List<EmpView> _ViewList1 = dt1.ToList<EmpView>();
                        List<EmpView> _TempViewList1 = new List<EmpView>();
                        title = "Employee Detail Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmployeeDetail.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmployeeDetail.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion

                    #region -- Daily Report --
                    case "leave_application": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and (FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        List<ViewLvApplication> _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        List<ViewLvApplication> _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);

                        break;
                    case "lv_applicationCPL": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and ( LvType='E' and FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRCPLLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRCPLLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);
                        break;
                    case "lv_applicationLWOP": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and ( LvType='F' and FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLWOPLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLWOPLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);

                        break;
                    case "detailed_att": DataTable dt2 = qb.GetValuesfromDB("select * from ViewDetailAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )");
                        List<ViewDetailAttData> _ViewList2 = dt2.ToList<ViewDetailAttData>();
                        List<ViewDetailAttData> _TempViewList2 = new List<ViewDetailAttData>();
                        title = "Detailed Attendence";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRdetailed.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRdetailed.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList2, _ViewList2), _dateFrom + " TO " + _dateTo);

                        break;
                    case "consolidated_att": DataTable dt3 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )");
                        List<ViewAttData> _ViewList3 = dt3.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList3 = new List<ViewAttData>();
                        title = "Consolidated Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAttendance.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAttendance.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList3, _ViewList3), _dateFrom + " TO " + _dateTo);

                        break;
                    case "present": DataTable datatable = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusP = 1 ");
                        List<ViewAttData> _ViewList4 = datatable.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList4 = new List<ViewAttData>();
                        title = "Present Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList4, _ViewList4), _dateFrom + " TO " + _dateTo);

                        break;
                    case "presentFather": datatable = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusP = 1 ");
                        _ViewList4 = datatable.ToList<ViewAttData>();
                        _TempViewList4 = new List<ViewAttData>();
                        title = "Present Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresentFather.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresentFather.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList4, _ViewList4), _dateFrom + " TO " + _dateTo);

                        break;
                    case "absent": DataTable dt5 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusAB = 1 ");
                        List<ViewAttData> _ViewList5 = dt5.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList5 = new List<ViewAttData>();
                        title = "Absent Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAbsent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAbsent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList5, _ViewList5), _dateFrom + " TO " + _dateTo);

                        break;
                    case "lv_application": DataTable dt6 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                + _dateTo + "'" + " )" + " and (StatusLeave=1 OR StatusHL=1)");
                        List<ViewAttData> _ViewList6 = dt6.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList6 = new List<ViewAttData>();
                        title = "Leave Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAbsent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAbsent.rdlc";

                        //LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList6, _ViewList6), _dateFrom + " TO " + _dateTo);
                        //To-do Develop Leave Attendance Report
                        break;
                    case "short_lv": DataTable dt7 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusSL=1");
                        List<ViewAttData> _ViewList7 = dt7.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList7 = new List<ViewAttData>();
                        title = "Short Leave Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRShortLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRShortLeave.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList7, _ViewList7), _dateFrom + " TO " + _dateTo);

                        break;
                    case "late_in": DataTable dt8 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and StatusLI=1 ");
                        List<ViewAttData> _ViewList8 = dt8.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList8 = new List<ViewAttData>();
                        title = "Late In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateIn.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "late_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                            + _dateTo + "'" + " )" + " and StatusLO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Late Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "early_in": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                           + _dateTo + "'" + " )" + " and StatusEI=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Early In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyIn.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "early_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                             + _dateTo + "'" + " )" + " and StatusEO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Early Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "overtime": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                         + _dateTo + "'" + " )" + " and StatusOT=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "OverTime Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DROverTime.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DROverTime.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "missing_attendance": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and ((TimeIn is null and TimeOut is not null) or (TimeIn is not null and TimeOut is null)) ");

                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        title = "Missing Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMissingAtt.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMissingAtt.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "multiple_in_out": dt = qb.GetValuesfromDB("select * from ViewMultipleInOut " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and (Tin1 is not null or TOut1 is not null)");
                        //change query for multiple_in_out
                        List<ViewMultipleInOut> _ViewList9 = dt.ToList<ViewMultipleInOut>();
                        List<ViewMultipleInOut> _TempViewList9 = new List<ViewMultipleInOut>();
                        title = "Multiple In/Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMultipleInOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMultipleInOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList9, _ViewList9), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion

                    #region -- Monthly Report --
                    case "monthly_leave_sheet": string _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList1 = dt.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MLvConsumed.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MLvConsumed.rdlc";
                        int monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        int monthTo = Convert.ToDateTime(_dateTo).Month;
                        //int totalMonths = monthfrom < monthTo ? monthTo : monthfrom;
                        for (int ul = monthfrom > monthTo ? monthTo : monthfrom; ul <= (monthfrom < monthTo ? monthTo : monthfrom); ul++)
                        {
                            LoadReport(PathString, GetLV(ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), monthfrom, Convert.ToDateTime(_dateFrom)), ul);

                        }

                        // LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom);
                        break;

                    case "monthly_21-20": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1 " + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Sheet (21st to 20th)";
                        List<ViewMonthlyDataPer> _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        List<ViewMonthlyDataPer> _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSheetP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSheetP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;


                    case "monthly_1-31": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Period = " + _period);
                        title = "Monthly Sheet (1st to 31st)";
                        List<ViewMonthlyData> _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        List<ViewMonthlyData> _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSheetC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSheetC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;

                    case "monthlysummary_21-20": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Summary Report (21st to 20th)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "monthlysummary_1-31": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Summary (1st to 31st)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummaryC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummaryC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom);
                        break;

                    case "monthly_21-20_excel": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                         monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        //dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Period = " + _period);
                        title = "Monthly Sheet (21st to 20th)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;

                    case "monthly_1-31_consolidated": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyData " + query + " and Status=1" + " and" + consolidatedMonth);
                        title = "Monthly Consolidated Attendance Sheet (1st to 31th)";
                        _ViewListMonthlyData = dt.ToList<ViewMonthlyData>();
                        _TempViewListMonthlyData = new List<ViewMonthlyData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelC.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelC.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyData, _ViewListMonthlyData), _dateFrom + " to " + _dateTo);
                        break;

                    case "monthly_21-20_consolidated": _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        consolidatedMonth = "";
                        monthfrom = Convert.ToDateTime(_dateFrom).Month;
                        monthTo = Convert.ToDateTime(_dateTo).Month;
                        for (int i = monthfrom; i <= monthTo; i++)
                        {
                            consolidatedMonth = consolidatedMonth + "  Period =" + i + Convert.ToDateTime(_dateFrom).Year.ToString() + " OR";
                        }
                        if (consolidatedMonth.Length > 4)
                            consolidatedMonth = consolidatedMonth.Substring(0, consolidatedMonth.Length - 3);
                        dt = qb.GetValuesfromDB("select * from ViewMonthlyDataPer " + query + " and Status=1" + " and" + consolidatedMonth);
                        title = "Monthly Consolidated (21th to 20th)(Excel)";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRDetailExcelP.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom + " to " + _dateTo);
                        break;

                    case "monthly_21-20_flexy": 
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Employee Record Report";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        LoadReport(LoadPermanentMonthlyDT(ReportsFilterImplementation(fm, _TempViewList, _ViewList), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo)), PathString, _dateFrom + " TO " + _dateTo);
                        break;
                    #endregion

                    #region -- Detailed Report --
                    case "emp_att": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        title = "Employee Attendance";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_attTime": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        title = "Employee Attendance";
                        dt1 = qb.GetValuesfromDB("select * from ViewDetailAttData " + query + " and Status=1 and CompanyID=1 " + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
                        _ViewList2 = dt1.ToList<ViewDetailAttData>();
                         _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpSumTime.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpSumTime.rdlc";
                        LoadReports(PathString,AddEmpSumTime(_ViewList2, ReportsFilterImplementation(fm, _TempViewList, _ViewList)), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo));
                        break;
                    case "emp_absent":
                        _period = Convert.ToDateTime(_dateFrom).Month.ToString() + Convert.ToDateTime(_dateFrom).Year.ToString();
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )" + " and StatusAB = 1 ");
                        title = "Employee Absent";
                        _ViewListMonthlyDataPer = dt.ToList<ViewMonthlyDataPer>();
                        _TempViewListMonthlyDataPer = new List<ViewMonthlyDataPer>();
                        //Change the Paths

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    case "lv_quota": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1");
                        _ViewList1 = dt.ToList<EmpView>();
                        _TempViewList1 = new List<EmpView>();
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MYLeaveSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MYLeaveSummary.rdlc";
                        LoadReport(PathString, GYL(ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), Convert.ToDateTime(_dateFrom)));
                        // LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListMonthlyDataPer, _ViewListMonthlyDataPer), _dateFrom);
                        break;
                    #endregion

                    #region ----Summary Report----
                    /////////////////////////////////////////////////////////////   
                        /////////////////Summary Reports////////////////////////////
                        ///////////////////////////////////////////////////////////
                    case "company_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm,_dateFrom,_dateTo,"C"), _dateFrom + " TO " + _dateTo,"Company Consolidated Summary");
                        break;
                    case "company_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "C"), _dateFrom + " TO " + _dateTo, "Company Strength Summary");
                        break;
                    case "company_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "C"), _dateFrom + " TO " + _dateTo, "Company Work Times Summary");
                        break;
                    case "location_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L");
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Consolidated Summary");
                        break;
                    case "location_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Strength Summary");
                        break;
                    case "location_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "L"), _dateFrom + " TO " + _dateTo, "Location Work Times Summary");
                        break;
                    case "shift_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Consolidated Summary");
                        break;
                    case "shift_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Strength Summary");
                        break;
                    case "shift_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "S"), _dateFrom + " TO " + _dateTo, "Shift Work Times Summary");
                        break;
                    case "category_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Consolidated Summary");
                        break;
                    case "category_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Strength Summary");
                        break;
                    case "category_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "A"), _dateFrom + " TO " + _dateTo, "Category Work Times Summary");
                        break;
                    case "type_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Consolidated Summary");
                        break;
                    case "type_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Strength Summary");
                        break;
                    case "type_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "T"), _dateFrom + " TO " + _dateTo, "Employee Type Work Times Summary");
                        break;
                    case "dept_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Consolidated Summary");
                        break;
                    case "dept_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Strength Summary");
                        break;
                    case "dept_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "D"), _dateFrom + " TO " + _dateTo, "Department Work Times Summary");
                        break;
                    case "section_consolidated":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSConsolidated.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSConsolidated.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Consolidated Summary");
                        break;
                    case "section_strength":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSEmpStrength.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSEmpStrength.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Strength Summary");
                        break;
                    case "section_worktimes":
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DSWorkSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DSWorkSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _dateFrom, _dateTo, "E"), _dateFrom + " TO " + _dateTo, "Section Work Times Summary");
                        break;
                    #endregion
                }

            }
        }
        private void LoadReports(string path, DataTable _Summary,DateTime? dtFrom,DateTime? dtTo)
        {
            string _Header = "Employee Attendance Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Employee Attendance Summary";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _Summary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", dtFrom.Value.ToString("dd-MMM-yyyy") + " TO" + dtTo.Value.ToString("dd-MMM-yyyy"), false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp,rp1});
            ReportViewer1.LocalReport.Refresh();
        }
        #region -- AttSummaryReport--
        private void monthlyProductivityProcess(String _dateFrom,String _dateTo,String query)
        {
            QueryBuilder qb = new QueryBuilder();
            string PathString = "";
          DataTable dt4  = qb.GetValuesfromDB("select * from ViewAttData " + query + " and Status=1" + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " )");
            List<ViewAttData> ListOfAttDate = new List<ViewAttData>();
            List<ViewAttData> TempList = new List<ViewAttData>();
            List<ViewAttData> finalOutput = new List<ViewAttData>();
            title = "Employee Attendace Summary New";
            if (GlobalVariables.DeploymentType == false)
                PathString = "/Reports/RDLC/MonthlyProductivityEmployees.rdlc";
            else
                PathString = "/WMS/Reports/RDLC/MonthlyProductivityEmployees.rdlc";
            ListOfAttDate = dt4.ToList<ViewAttData>();
            TempList = new List<ViewAttData>();

          finalOutput =  ReportsFilterImplementation(fm, TempList, ListOfAttDate);
           List<EmpMonthlyProductivityEntity> empe = new List<EmpMonthlyProductivityEntity>();
            empe = EmpMonthlyProductivityEntity.ProcessAttendence(finalOutput, _dateFrom, _dateTo);
            LoadReport(empe,PathString,_dateFrom,_dateTo);



        }

        private DataTable LoadPermanentMonthlyDT(List<EmpView> list, DateTime datefrom, DateTime dateto)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<AttData> attData = new List<AttData>();
            List<AttData> _EmpAttData = new List<AttData>();
            foreach (EmpView emp in list)
            {
                try
                {
                    attData = db.AttDatas.Where(aa => aa.EmpID == emp.EmpID && aa.AttDate >= datefrom && aa.AttDate <= dateto).ToList();
                    PermanentMonthly cmp = new PermanentMonthly();
                    _EmpAttData = attData.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    AttMnDataPer attMn = new AttMnDataPer();
                    attMn = cmp.processPermanentMonthlyAttSingle((DateTime)datefrom, (DateTime)dateto, emp, _EmpAttData);
                    AddDataToMonthlyDataAtable(attMn.Period.ToString(), attMn.EmpMonth.ToString(), (DateTime)attMn.StartDate, (DateTime)attMn.EndDate, attMn.EmpNo, (int)attMn.EmpID, attMn.EmpName, emp.SectionName, emp.DeptName, emp.TypeName, emp.LocName, emp.ShiftName, attMn.D21, attMn.D22, attMn.D23, attMn.D24, attMn.D25, attMn.D26, attMn.D27, attMn.D28, attMn.D29, attMn.D30, attMn.D31, attMn.D1, attMn.D2, attMn.D3, attMn.D4, attMn.D5, attMn.D6, attMn.D7, attMn.D8, attMn.D9, attMn.D10, attMn.D11, attMn.D12, attMn.D13, attMn.D14, attMn.D15, attMn.D16, attMn.D17, attMn.D18, attMn.D19, attMn.D20, (short)attMn.TotalDays, (short)attMn.WorkDays, (short)attMn.PreDays, (short)attMn.AbDays, (short)attMn.RestDays, (short)attMn.GZDays, (short)attMn.LeaveDays, (short)attMn.OfficialDutyDays, (short)attMn.TEarlyIn, (short)attMn.TEarlyOut, (short)attMn.TLateIn, (short)attMn.TLateOut, (short)attMn.TWorkTime, (short)attMn.TNOT, (short)attMn.TGZOT, (short)attMn.ExpectedWrkTime, (short)attMn.OT1, (short)attMn.OT2, (short)attMn.OT3, (short)attMn.OT4, (short)attMn.OT5, (short)attMn.OT6, (short)attMn.OT7, (short)attMn.OT8, (short)attMn.OT9, (short)attMn.OT10, (short)attMn.OT11, (short)attMn.OT12, (short)attMn.OT13, (short)attMn.OT14, (short)attMn.OT15, (short)attMn.OT16, (short)attMn.OT17, (short)attMn.OT18, (short)attMn.OT19, (short)attMn.OT20, (short)attMn.OT21, (short)attMn.OT22, (short)attMn.OT23, (short)attMn.OT24, (short)attMn.OT25, (short)attMn.OT26, (short)attMn.OT27, (short)attMn.OT28, (short)attMn.OT29, (short)attMn.OT30, (short)attMn.OT31);
                }
                catch (Exception ex)
                {

                }
            }
            return FlexyMonthlyReportDT;
        }
        #endregion

        #region -- Load Report --
        private void LoadReport(List<EmpMonthlyProductivityEntity> empe, string PathString, string _dateFrom, string _dateTo)
        {
            string Header = "Monthly Productivity Sheet";
            this.ReportViewer1.LocalReport.DisplayName = "Monthly Productivity Sheet";
            this.ReportViewer1.ProcessingMode = ProcessingMode.Local;
            string Month = Convert.ToDateTime(_dateFrom).Date.ToString("MMMM-yyyy");
            Month = "For the Month of " + Month;
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            this.ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<EmpMonthlyProductivityEntity> ie;
            ie = empe.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();

            ReportDataSource datasource1 = new ReportDataSource("DataSet2", companyImage);
            ReportDataSource datasource2 = new ReportDataSource("DataSet1", ie);

            this.ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            this.ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", Month, false);
            ReportParameter rp1 = new ReportParameter("Header", Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            this.ReportViewer1.LocalReport.Refresh();


            //this.ReportViewer1.LocalReport.DisplayName = title;
            //ReportViewer1.ProcessingMode = ProcessingMode.Local;
            //ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            //System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            //ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            //IEnumerable<DailySummary> ie;
            //ie = list.AsQueryable();
            //IEnumerable<EmpPhoto> companyImage;
            //companyImage = companyimage.AsQueryable();
            //ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            //ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            //ReportViewer1.LocalReport.DataSources.Clear();
            //ReportViewer1.LocalReport.EnableExternalImages = true;
            //ReportViewer1.LocalReport.DataSources.Add(datasource1);
            //ReportViewer1.LocalReport.DataSources.Add(datasource2);
            //ReportParameter rp = new ReportParameter("Date", p, false);
            //ReportParameter rp1 = new ReportParameter("Header", Header, false);
            //this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            //ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(DataTable dataTable, string PathString, string p)
        {
            string _Header = "Flexy Monthly Sheet";
            //string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Flexy Monthly Sheet";
            string Date = p;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", dataTable);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string PathString, List<DailySummary> list, string p,string Header)
        {
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<DailySummary> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", p, false);
            ReportParameter rp1 = new ReportParameter("Header", Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewBadli> list, string p)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewBadli> ie;
            ie = list.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", p, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp1, rp });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<TASReportDataSet.SummarizedMonthlyReportDataTable> VMLD, string p)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<TASReportDataSet.SummarizedMonthlyReportDataTable> ie;
            ie = VMLD.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<AttDeptSummary> AttDept, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<AttDeptSummary> ie;
            ie = AttDept.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Title", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewMultipleInOut> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMultipleInOut> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewDetailAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewDetailAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);

            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewMonthlyData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            date = "";
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, List<ViewMonthlyDataPer> _Employee, String date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyDataPer> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, DataTable _LvSummary)
        {
            string _Header = "Year wise Leaves Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Leave Summary Report";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        ///
        private void LoadReport(string path, List<EmpView> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<EmpView> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, DataTable _LvSummary, int i)
        {
            string _Header = "Monthly Leaves Sheet";
            //string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Leave Balance Report";
            string Date = "Month: " + Convert.ToDateTime(_dateFrom).Date.ToString("MMMM");
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewLvApplication> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewLvApplication> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<EmpPhoto> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }

        #endregion

        #region -- Filters Implementation--
        private List<ViewMultipleInOut> ReportsFilterImplementation(FiltersModel fm, List<ViewMultipleInOut> _TempViewList, List<ViewMultipleInOut> _ViewList)
        {

            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private List<ViewLvApplication> ReportsFilterImplementation(FiltersModel fm, List<ViewLvApplication> _TempViewList, List<ViewLvApplication> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //EmpView
        private List<DailySummary> ReportsFilterImplementation(FiltersModel fm, string dateFrom, string dateTo, string Criteria)
        {
            List<DailySummary> ViewDS = new List<DailySummary>();
            List<DailySummary> TempDS = new List<DailySummary>();
            QueryBuilder qb = new QueryBuilder();
            DataTable dt = new DataTable();
            switch (Criteria)
            {
                case "C":
                    //for company
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.CompanyFilter.Count > 0)
                    {
                        foreach (var comp in fm.CompanyFilter)
                        {
                            short _compID = Convert.ToInt16(comp.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "L":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.LocationFilter.Count > 0)
                    {
                        foreach (var loc in fm.LocationFilter)
                        {
                            short _locID = Convert.ToInt16(loc.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _locID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "D":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.DepartmentFilter.Count > 0)
                    {
                        foreach (var dept in fm.DepartmentFilter)
                        {
                            short _deptID = Convert.ToInt16(dept.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _deptID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "E":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.SectionFilter.Count > 0)
                    {
                        foreach (var sec in fm.SectionFilter)
                        {
                            short _secID = Convert.ToInt16(sec.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _secID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;

                case "S":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.ShiftFilter.Count > 0)
                    {
                        foreach (var shift in fm.ShiftFilter)
                        {
                            short _shiftID = Convert.ToInt16(shift.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _shiftID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "T":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.TypeFilter.Count > 0)
                    {
                        foreach (var type in fm.TypeFilter)
                        {
                            short _typeID = Convert.ToInt16(type.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _typeID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                //case "A":
                //    if (fm.CompanyFilter.Count > 0)
                //    {
                //        foreach (var comp in fm.CompanyFilter)
                //        {
                //            short _compID = Convert.ToInt16(comp.ID);
                //            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                //        }
                //        ViewDS = TempDS.ToList();
                //    }
                //    else
                //        TempDS = ViewDS.ToList();
                //    TempDS.Clear();
                //    break;
            }
            return ViewDS;
        }
        public List<EmpView> ReportsFilterImplementation(FiltersModel fm, List<EmpView> _TempViewList, List<EmpView> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        private List<ViewBadli> ReportsFilterImplementation(FiltersModel fm, List<ViewBadli> _TempViewList, List<ViewBadli> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    //baldi doesnt have the division id so using division name in
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == Convert.ToInt16(div.ID)).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //ViewAttData
        public List<ViewAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewAttData> _TempViewList, List<ViewAttData> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        public List<EmpPhoto> GetCompanyImages(FiltersModel fm)
        {
            TAS2013Entities ctx = new TAS2013Entities();
            companyimage = new List<EmpPhoto>();
            if (fm.CompanyFilter.Count > 1)
            {
                companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == 4785).First());
            }
            else
            {
                if (fm.CompanyFilter.Count > 0)
                {
                    int id = Int32.Parse(fm.CompanyFilter.First().ID);
                    Company comp = ctx.Companies.Where(aa => aa.CompID == id).FirstOrDefault();
                    companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == comp.ImageID).First());
                }
                else
                {
                    companyimage.Add(ctx.EmpPhotoes.Where(aa => aa.PhotoID == 4785).First());
                }
            }

            return companyimage;

        }

        //ViewAttData
        public List<ViewDetailAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewDetailAttData> _TempViewList, List<ViewDetailAttData> _ViewList)
        {
           
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
       
        //ViewMonthlyData
        public List<ViewMonthlyData> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyData> _TempViewList, List<ViewMonthlyData> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        //ViewMonthlyDataPer
        public List<ViewMonthlyDataPer> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyDataPer> _TempViewList, List<ViewMonthlyDataPer> _ViewList)
        {
            //for company
            if (fm.CompanyFilter.Count > 0)
            {
                foreach (var comp in fm.CompanyFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CompanyID == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            if (fm.CrewFilter.Count > 0)
            {
                foreach (var cre in fm.CrewFilter)
                {
                    short _crewID = Convert.ToInt16(cre.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();





            //for division
            if (fm.DivisionFilter.Count > 0)
            {
                foreach (var div in fm.DivisionFilter)
                {
                    short _divID = Convert.ToInt16(div.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        #endregion

        private DataTable GYL(List<EmpView> _Emp,DateTime dateTimeLv)
        {
            TAS2013Entities context = new TAS2013Entities();
            List<LvConsumed> leaveQuota = new List<LvConsumed>();
            List<LvConsumed> tempLeaveQuota = new List<LvConsumed>();
            string year = dateTimeLv.Year.ToString();
            leaveQuota = context.LvConsumeds.Where(aa => aa.Year == year).ToList();
            foreach (var emp in _Emp)
            {
                int EmpID=0;
                string EmpNo = ""; string EmpName = "";
                float TotalAL = 0; float BalAL = 0; float TotalCL = 0; float BalCL = 0; float TotalSL = 0; float BalSL = 0;
                float JanAL = 0; float JanCL = 0; float JanSL = 0; float FebAL = 0; float FebCL = 0; float FebSL = 0;
                float MarchAL = 0; float MarchCL = 0; float MarchSL = 0;
                float AprilAL = 0; float AprilCL = 0; float AprilSL = 0;
                float MayAL = 0; float MayCL = 0; float MaySL = 0;
                float JunAL = 0; float JunCL = 0; float JunSL = 0;
                float JullyAL = 0; float JullyCL = 0; float JullySL = 0;
                float AugAL = 0; float AugCL = 0; float AugSL = 0;
                float SepAL = 0; float SepCL = 0; float SepSL = 0;
                float OctAL = 0; float OctCL = 0; float OctSL = 0;
                float NovAL = 0; float NovCL = 0; float NovSL = 0;
                float DecAL = 0; float DecCL = 0; float DecSL = 0;
                string Remarks = ""; string DeptName = ""; short DeptID = 0; string LocationName = ""; short LocationID = 0; string SecName = ""; short SecID = 0; string DesgName = ""; short DesigID = 0; string CrewName = ""; short CrewID = 0; string CompanyName = ""; short CompanyID=0;
                tempLeaveQuota = leaveQuota.Where(aa => aa.EmpID == emp.EmpID).ToList();
                foreach (var leave in tempLeaveQuota)
                {
                    EmpID = emp.EmpID;
                    EmpNo = emp.EmpNo;
                    EmpName = emp.EmpName;
                    DeptID = (short)emp.DeptID;
                    DeptName = emp.DeptName;
                    LocationName = emp.LocName;
                    LocationID = (short)emp.LocID;
                    SecName = emp.SectionName;
                    SecID = (short)emp.SecID;
                    DesgName = emp.DesignationName;
                    DesigID = (short)emp.DesigID;
                    CrewName = emp.CrewName;
                    CrewID = (short)emp.CrewID;
                    CompanyName = emp.CompName;
                    CompanyID = (short)emp.CompanyID;
                    switch (leave.LeaveType)
                    {
                        case "A"://Casual
                            JanCL = (float)leave.JanConsumed;
                            FebCL = (float)leave.FebConsumed;
                            MarchCL = (float)leave.MarchConsumed;
                            AprilCL = (float)leave.AprConsumed;
                            MayCL = (float)leave.MayConsumed;
                            JunCL = (float)leave.JuneConsumed;
                            JullyCL = (float)leave.JulyConsumed;
                            AugCL = (float)leave.AugustConsumed;
                            SepCL = (float)leave.SepConsumed;
                            OctCL = (float)leave.OctConsumed;
                            NovCL = (float)leave.NovConsumed;
                            DecCL = (float)leave.DecConsumed;
                            TotalCL = (float)leave.TotalForYear;
                            BalCL = (float)leave.YearRemaining;
                            break;
                        case "B"://Anual
                            JanAL = (float)leave.JanConsumed;
                            FebAL = (float)leave.FebConsumed;
                            MarchAL = (float)leave.MarchConsumed;
                            AprilAL = (float)leave.AprConsumed;
                            MayAL = (float)leave.MayConsumed;
                            JunAL = (float)leave.JuneConsumed;
                            JullyAL = (float)leave.JulyConsumed;
                            AugAL = (float)leave.AugustConsumed;
                            SepAL = (float)leave.SepConsumed;
                            OctAL = (float)leave.OctConsumed;
                            NovAL = (float)leave.NovConsumed;
                            DecAL = (float)leave.DecConsumed;
                            TotalAL = (float)leave.TotalForYear;
                            BalAL = (float)leave.YearRemaining;
                            break;
                        case "C"://Sick
                            JanSL = (float)leave.JanConsumed;
                            FebSL = (float)leave.FebConsumed;
                            MarchSL = (float)leave.MarchConsumed;
                            AprilSL = (float)leave.AprConsumed;
                            MaySL = (float)leave.MayConsumed;
                            JunSL = (float)leave.JuneConsumed;
                            JullySL = (float)leave.JulyConsumed;
                            AugSL = (float)leave.AugustConsumed;
                            SepSL = (float)leave.SepConsumed;
                            OctSL = (float)leave.OctConsumed;
                            NovSL = (float)leave.NovConsumed;
                            DecSL = (float)leave.DecConsumed;
                            TotalSL = (float)leave.TotalForYear;
                            BalSL = (float)leave.YearRemaining;
                            break;
                    }
                                    }
                AddDataToDT(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL, AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL, SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, (short)DeptID, LocationName, (short)LocationID, SecName, (short)SecID, DesgName, DesigID, CrewName, CrewID, CompanyName, (short)CompanyID);

            }
            return MYLeaveSummaryDT;
        }
        DataTable MYLeaveSummaryDT = new DataTable();
        
        public void CreateDataTable()
        {
            MYLeaveSummaryDT.Columns.Add("EmpID", typeof(int));
            MYLeaveSummaryDT.Columns.Add("EmpNo", typeof(string));
            MYLeaveSummaryDT.Columns.Add("EmpName", typeof(string));

            MYLeaveSummaryDT.Columns.Add("TotalAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalAL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalCL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalSL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JanAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("FebAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MarchAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AprilAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MayAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MayCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MaySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JunAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JulyAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulyCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AugAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("SepAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("OctAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("NovAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("DecAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("Remarks", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("SecName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("SecID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("DesgName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DesgID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CrewName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CrewID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CompanyName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CompanyID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("LocationName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("LocationID", typeof(short));
            LvSummaryMonth.Columns.Add("EmpNo", typeof(string));
            LvSummaryMonth.Columns.Add("EmpName", typeof(string));
            LvSummaryMonth.Columns.Add("Designation", typeof(string));
            LvSummaryMonth.Columns.Add("Section", typeof(string));
            LvSummaryMonth.Columns.Add("Department", typeof(string));
            LvSummaryMonth.Columns.Add("EmpType", typeof(string));
            LvSummaryMonth.Columns.Add("Category", typeof(string));
            LvSummaryMonth.Columns.Add("Location", typeof(string));
            LvSummaryMonth.Columns.Add("TotalCL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalSL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalAL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalCPL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedCL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedSL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedAL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedCPL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedLWOP", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceSL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceAL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCPL", typeof(float));
            LvSummaryMonth.Columns.Add("Remarks", typeof(string));
            LvSummaryMonth.Columns.Add("Month", typeof(string));
        }


        public void AddDataToDT(int EmpID, string EmpNo, string EmpName, float TotalAL, float BalAL,
            float TotalCL, float BalCL, float TotalSL, float BalSL,
            float JanAL, float JanCL, float JanSL,
            float FebAL, float FebCL, float FebSL,
            float MarchAL, float MarchCL, float MarchSL,
            float AprilAL, float AprilCL, float AprilSL,
            float MayAL, float MayCL, float MaySL,
            float JunAL, float JunCL, float JunSL,
            float JullyAL, float JullyCL, float JullySL,
            float AugAL, float AugCL, float AugSL,
            float SepAL, float SepCL, float SepSL,
            float OctAL, float OctCL, float OctSL,
            float NovAL, float NovCL, float NovSL,
            float DecAL, float DecCL, float DecSL,
            string Remarks, string DeptName, short DeptID, string LocationName, short LocationID, string SecName, short SecID, string DesgName, short DesgID, string CrewName, short CrewID, string CompanyName, short CompanyID)
        {
            MYLeaveSummaryDT.Rows.Add(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL,
                AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL,
                SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, DeptID, LocationName, LocationID, CrewName, CrewID, SecName, SecID, CompanyName, CompanyID);
        }

        #region --Leave Process--
        private DataTable GetLV(List<EmpView> _Emp, int month,DateTime dateTimeLv)
        {
            using (var ctx = new TAS2013Entities())
            {

                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                LvConsumed _lvTemp = new LvConsumed();
                string year = dateTimeLv.Year.ToString();
                _lvConsumed = ctx.LvConsumeds.Where(aa=>aa.Year==year).ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                //List<LvData> lvData = ctx.LvDatas.Where(aa=>aa.AttDate>=)
                foreach (var emp in _Emp)
                {
                    float BeforeCL = 0, UsedCL = 0, BalCL = 0;
                    float BeforeSL = 0, UsedSL = 0, BalSL = 0;
                    float BeforeAL = 0, UsedAL = 0, BalAL = 0;
                    string _month = "";
                    List<LvConsumed> entries = _lvConsumed.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    string EmpLvC = emp.EmpID.ToString() + "A" + dateTimeLv.Year.ToString();
                    string EmpLvS = emp.EmpID.ToString() + "C" + dateTimeLv.Year.ToString();
                    string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    string EmpLvE = emp.EmpID.ToString() + "E" + dateTimeLv.Year.ToString();
                    //string EmpLvA = emp.EmpID.ToString() + "B" + dateTimeLv.Year.ToString();
                    LvConsumed eCL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvC);
                    LvConsumed eSL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvS);
                    LvConsumed eAL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvA);
                    LvConsumed eCPL = entries.FirstOrDefault(lv => lv.EmpLvTypeYear == EmpLvE);
                    if (entries.Count > 0 && eCL!=null && eSL!=null && eAL!=null)
                    {
                        switch (month)
                        {
                            case 1:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear;
                                UsedCL = (float)eCL.JanConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear;
                                UsedSL = (float)eSL.JanConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear;
                                UsedAL = (float)eAL.JanConsumed;
                                _month = "January";
                                break;
                            case 2:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - (float)eCL.JanConsumed;
                                UsedCL = (float)eCL.FebConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - (float)eSL.JanConsumed;
                                UsedSL = (float)eSL.FebConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - (float)eAL.JanConsumed;
                                UsedAL = (float)eAL.FebConsumed;
                                break;
                                _month = "Febu";
                            case 3:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed);
                                UsedCL = (float)eCL.MarchConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed);
                                UsedSL = (float)eSL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                break;
                            case 4:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed);
                                UsedCL = (float)eCL.AprConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed);
                                UsedSL = (float)eSL.AprConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed);
                                UsedAL = (float)eAL.AprConsumed;
                                break;
                            case 5:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed);
                                UsedCL = (float)eCL.MayConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed);
                                UsedSL = (float)eSL.MayConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed);
                                UsedAL = (float)eAL.MayConsumed;
                                break;
                            case 6:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed);
                                UsedCL = (float)eCL.JuneConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed);
                                UsedSL = (float)eSL.JuneConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed);
                                UsedAL = (float)eAL.JuneConsumed;
                                break;
                            case 7:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed);
                                UsedCL = (float)eCL.JulyConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed);
                                UsedSL = (float)eSL.JulyConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed);
                                UsedAL = (float)eAL.JulyConsumed;
                                break;
                            case 8:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed);
                                UsedCL = (float)eCL.AugustConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed);
                                UsedSL = (float)eSL.AugustConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed);
                                UsedAL = (float)eAL.AugustConsumed;
                                break;
                            case 9:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed);
                                UsedCL = (float)eCL.SepConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed);
                                UsedSL = (float)eSL.SepConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                break;
                            case 10:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed);
                                UsedCL = (float)eCL.OctConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed);
                                UsedSL = (float)eSL.OctConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                break;
                            case 11:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed);
                                UsedCL = (float)eCL.NovConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed);
                                UsedSL = (float)eSL.NovConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed);
                                UsedAL = (float)eAL.NovConsumed;
                                break;
                            case 12:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed + (float)eCL.NovConsumed);
                                UsedCL = (float)eCL.DecConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed + (float)eSL.NovConsumed);
                                UsedSL = (float)eSL.DecConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed + (float)eAL.NovConsumed);
                                UsedAL = (float)eAL.DecConsumed;
                                break;

                        }
                        BalCL = (float)(BeforeCL - UsedCL);
                        BalSL = (float)(BeforeSL - UsedSL);
                        BalAL = (float)(BeforeAL - UsedAL);
                        //AddDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName, emp.DeptName, emp.TypeName, emp.CatName, emp.LocName, BeforeCL, BeforeSL, BeforeAL, UsedCL, UsedSL, UsedAL, BalCL, BalSL, BalAL, _month);
                    }

                }
            }
            return LvSummaryMonth;
        }
        public void AddDataToDT(string EmpNo, string EmpName, string Designation, string Section,
                                 string Department, string EmpType, string Category, string Location,
                                 float TotalCL, float TotalSL, float TotalAL, float TotalCPL,
                                 float ConsumedCL, float ConsumedSL, float ConsumedAL,float ConsumedCPL,float ConsumedLWOP,
                                 float BalanaceCL, float BalanaceSL, float BalananceAL, float BalanceCPL, string Month)
        {
            LvSummaryMonth.Rows.Add(EmpNo, EmpName, Designation, Section, Department, EmpType, Category, Location,
                TotalCL, TotalSL, TotalAL,TotalCPL, ConsumedCL, ConsumedSL, ConsumedAL,ConsumedCPL,ConsumedLWOP,
                BalanaceCL, BalanaceSL, BalananceAL,BalanceCPL, Month);
        }
        DataTable LvSummaryMonth = new DataTable();


        #endregion

        DataTable FlexyMonthlyReportDT = new DataTable();

        public void CreateFlexyMonthlyDataTable()
        {
            FlexyMonthlyReportDT.Columns.Add("Period", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("EmpMonth", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("StartDate", typeof(DateTime));
            FlexyMonthlyReportDT.Columns.Add("EndDate", typeof(DateTime));
            FlexyMonthlyReportDT.Columns.Add("EmpNo", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("EmpID", typeof(int));
            FlexyMonthlyReportDT.Columns.Add("EmpName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("SectionName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("DeptName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("TypeName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("LocName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("ShiftName", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D21", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D22", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D23", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D24", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D25", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D26", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D27", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D28", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D29", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D30", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D31", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D1", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D2", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D3", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D4", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D5", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D6", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D7", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D8", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D9", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D10", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D11", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D12", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D13", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D14", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D15", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D16", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D17", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D18", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D19", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("D20", typeof(string));
            FlexyMonthlyReportDT.Columns.Add("TotalDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("WorkDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("PreDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("AbDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("RestDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("GZDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("LeaveDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OfficialDutyDays", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TEarlyIn", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TEarlyOut", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TLateIn", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TLateOut", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TWorkTime", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TNOT", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("TGZOT", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("ExpectedWrkTime", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT1", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT2", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT3", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT4", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT5", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT6", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT7", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT8", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT9", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT10", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT11", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT12", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT13", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT14", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT15", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT16", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT17", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT18", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT19", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT20", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT21", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT22", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT23", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT24", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT25", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT26", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT27", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT28", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT29", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT30", typeof(Int16));
            FlexyMonthlyReportDT.Columns.Add("OT31", typeof(Int16));

        }

        public void AddDataToMonthlyDataAtable(string Period,string EmpMonth,DateTime  StartDate,DateTime  EndDate,string EmpNo,int  EmpID,string EmpName,string SectionName,string DeptName,string TypeName,string LocName,string ShiftName, string D21,string D22,string D23,string D24,string D25,string D26,string D27,string D28,string D29,string D30,string D31,string D1,string D2,string D3,string D4,string D5,string D6,string D7,string D8,string D9,string D10,string D11,string D12,string D13,string D14,string D15,string D16,string D17,string D18,string D19,string D20,Int16 TotalDays,Int16 WorkDays,Int16 PreDays,Int16 AbDays,Int16 RestDays,Int16 GZDays,Int16 LeaveDays,Int16 OfficialDutyDays,Int16 TEarlyIn,Int16 TEarlyOut,Int16 TLateIn,Int16 TLateOut,Int16 TWorkTime,Int16 TNOT,Int16 TGZOT,Int16 ExpectedWrkTime,Int16 OT1,Int16 OT2,Int16 OT3,Int16 OT4,Int16 OT5,Int16 OT6,Int16 OT7,Int16 OT8,Int16 OT9,Int16 OT10,Int16 OT11,Int16 OT12,Int16 OT13,Int16 OT14,Int16 OT15,Int16 OT16,Int16 OT17,Int16 OT18,Int16 OT19,Int16 OT20,Int16 OT21,Int16 OT22,Int16 OT23,Int16 OT24,Int16 OT25,Int16 OT26,Int16 OT27,Int16 OT28,Int16 OT29,Int16 OT30,Int16 OT31)
        {
            FlexyMonthlyReportDT.Rows.Add(Period,EmpMonth,StartDate,EndDate,EmpNo,EmpID,EmpName,SectionName,DeptName,TypeName,LocName,ShiftName,D21,D22,D23,D24,D25,D26,D27,D28,D29,D30,D31,D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,D16,D17,D18,D19,D20,TotalDays,WorkDays,PreDays,AbDays,RestDays,GZDays,LeaveDays,OfficialDutyDays,TEarlyIn,TEarlyOut,TLateIn,TLateOut,TWorkTime,TNOT,TGZOT,ExpectedWrkTime,OT1,OT2,OT3,OT4,OT5,OT6,OT7,OT8,OT9,OT10,OT11,OT12,OT13,OT14,OT15,OT16,OT17,OT18,OT19,OT20,OT21,OT22,OT23,OT24,OT25,OT26,OT27,OT28,OT29,OT30,OT31);

        }

        #region -- Emp Summary Report with Time Difference --
        DataTable EmpSummTimes = new DataTable();
        public void CreateEmpSumTimeDatatable()
        {
            EmpSummTimes.Columns.Add("EmpDate", typeof(string));
            EmpSummTimes.Columns.Add("AttDate", typeof(DateTime));
            EmpSummTimes.Columns.Add("EmpNo", typeof(string));
            EmpSummTimes.Columns.Add("EmpID", typeof(int));
            EmpSummTimes.Columns.Add("EmpName", typeof(string));
            EmpSummTimes.Columns.Add("SectionName", typeof(string));
            EmpSummTimes.Columns.Add("DeptName", typeof(string));
            EmpSummTimes.Columns.Add("TypeName", typeof(string));
            EmpSummTimes.Columns.Add("LocName", typeof(string));
            EmpSummTimes.Columns.Add("ShiftName", typeof(string));
            EmpSummTimes.Columns.Add("Tin0", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut0", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff0", typeof(int));
            EmpSummTimes.Columns.Add("Tin1", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut1", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff1", typeof(int));
            EmpSummTimes.Columns.Add("Tin2", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut2", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff2", typeof(int));
            EmpSummTimes.Columns.Add("Tin3", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut3", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff3", typeof(int));
            EmpSummTimes.Columns.Add("Tin4", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut4", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff4", typeof(int));
            EmpSummTimes.Columns.Add("Tin5", typeof(DateTime));
            EmpSummTimes.Columns.Add("TOut5", typeof(DateTime));
            EmpSummTimes.Columns.Add("Diff5", typeof(int));
            EmpSummTimes.Columns.Add("WorkMin", typeof(int));
            EmpSummTimes.Columns.Add("ShiftMin", typeof(int));
            EmpSummTimes.Columns.Add("DutyCode", typeof(string));
            EmpSummTimes.Columns.Add("Loss", typeof(int));
            EmpSummTimes.Columns.Add("Extra", typeof(int));
            EmpSummTimes.Columns.Add("Remarks", typeof(string));
        }

        public DataTable AddEmpSumTime(List<ViewDetailAttData> attDatas, List<EmpView> emps)
        {
            foreach (var emp in emps)
            {
                List<ViewDetailAttData> tempAttData = new List<ViewDetailAttData>();
                tempAttData = attDatas.Where(aa => aa.EmpID == emp.EmpID).ToList();
                foreach (var attdata in tempAttData)
                {
                    int diff0 = 0;
                    int diff1 = 0;
                    int diff2 = 0;
                    int diff3 = 0;
                    int diff4 = 0;
                    int diff5 = 0;
                    int WorkMins = 0;
                    int Loss = 0;
                    int Extra = 0;
                    if (attdata.Tin0 != null && attdata.Tout0 != null)
                    {
                        TimeSpan t0 = (TimeSpan)(attdata.Tout0 - attdata.Tin0);
                        diff0 = (int)t0.TotalMinutes;
                    }
                    if (attdata.Tin1 != null && attdata.Tout1 != null)
                    {
                        if (attdata.Tin0 == attdata.Tin1 && attdata.Tout0 == attdata.Tout1)
                        {
                        }
                        else
                        {
                            TimeSpan t1 = (TimeSpan)(attdata.Tout1 - attdata.Tin1);
                            diff1 = (int)t1.TotalMinutes;
                        }
                    }
                    if (attdata.Tin2 != null && attdata.Tout2 != null)
                    {
                        if (attdata.Tin1 == attdata.Tin2 && attdata.Tout1 == attdata.Tout2)
                        {
                        }
                        else
                        {
                            TimeSpan t2 = (TimeSpan)(attdata.Tout2 - attdata.Tin2);
                            diff2 = (int)t2.TotalMinutes;
                        }
                    }
                    if (attdata.Tin3 != null && attdata.Tout3 != null)
                    {
                        if (attdata.Tin2 == attdata.Tin3 && attdata.Tout2 == attdata.Tout3)
                        {
                        }
                        else
                        {
                            TimeSpan t3 = (TimeSpan)(attdata.Tout3 - attdata.Tin3);
                            diff3 = (int)t3.TotalMinutes;
                        }
                    }
                    if (attdata.Tin4 != null && attdata.Tout4 != null)
                    {
                        if (attdata.Tin3 == attdata.Tin4 && attdata.Tout3 == attdata.Tout4)
                        {
                        }
                        else
                        {
                            TimeSpan t4 = (TimeSpan)(attdata.Tout4 - attdata.Tin4);
                            diff4 = (int)t4.TotalMinutes;
                        }
                    }
                    if (attdata.Tin5 != null && attdata.Tout5 != null)
                    {
                        if (attdata.Tin4 == attdata.Tin5 && attdata.Tout4 == attdata.Tout5)
                        {
                        }
                        else
                        {
                            TimeSpan t5 = (TimeSpan)(attdata.Tout5 - attdata.Tin5);
                            diff5 = (int)t5.TotalMinutes;
                        }
                    }
                    WorkMins = diff0 + diff1 + diff2 + diff3 + diff4 + diff5;
                    if (WorkMins > attdata.ShifMin)
                    {
                        Extra = (int)(WorkMins - attdata.ShifMin);
                    }
                    else
                    {
                        Loss = (int)(attdata.ShifMin-WorkMins);
                    }
                    AddDataToSumTimeDatatable(attdata.EmpDate, attdata.AttDate, attdata.EmpNo, attdata.EmpID, emp.EmpName, emp.SectionName,
                        emp.DeptName, emp.TypeName, emp.LocName, emp.ShiftName, attdata.Tin0, attdata.Tout0, diff0, attdata.Tin1, attdata.Tout1,
                        diff1, attdata.Tin2, attdata.Tout2, diff2, attdata.Tin3, attdata.Tout3, diff3, attdata.Tin4, attdata.Tout4, diff4,
                        attdata.Tin5, attdata.Tout5, diff5, WorkMins, attdata.ShifMin, attdata.DutyCode, Loss, Extra, attdata.Remarks);

                }
            }
            return EmpSummTimes;
        }
        public void AddDataToSumTimeDatatable(string EmpDate, DateTime? AttDate, string EmpNo, int? EmpID, string EmpName,
    string SectionName, string DeptName, string TypeName, string LocName, string ShiftName,
                                      DateTime? Tin0, DateTime? Tout0, int? Diff0, DateTime? Tin1, DateTime? Tout1, int? Diff1,
    DateTime? Tin2, DateTime? Tout2, int? Diff2, DateTime? Tin3, DateTime? Tout3, int? Diff3, DateTime? Tin4, DateTime? Tout4, int? Diff4,
    DateTime? Tin5, DateTime? Tout5, int? Diff5, int? WorkMin, int? ShiftMin, string DutyCode, int? LossMin, int? ExtraMin, string Remarks)
        {
            EmpSummTimes.Rows.Add(EmpDate, AttDate, EmpNo, EmpID, EmpName, SectionName, DeptName, TypeName, LocName, ShiftName, Tin0, 
                Tout0, Diff0,Tin1, Tout1, Diff1, Tin2, Tout2, Diff2, Tin3, Tout3, Diff3, Tin4, Tout4, Diff4, Tin5, Tout5, Diff5, 
                WorkMin, ShiftMin, DutyCode, LossMin, ExtraMin, Remarks);
        }
        #endregion
    }
}