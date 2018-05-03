using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using ITWorkLogs.Models;
using CrystalDecisions.Web;
using ITWorkLogs.ViewModels;

namespace ITWorkLogs.Controllers
{
    public class WorkLogsReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult WorkLogsReport()
        {
            WorklogsReportViewerViewModel model = new WorklogsReportViewerViewModel();
            string content = Url.Content("~/Reports/WorkLogsReports/EmployeeList.aspx");
            model.ReportPath = content;
            return View("ReportViewer", model);
        }
   
        //public ActionResult ExportCompleteTaskReport()
        //{
        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Path.Combine(Server.MapPath("~/Reports"), "worklogs.rpt"));

        //    rd.SetDataSource(db.workLogs.Select(c => new
        //    {
        //      Personnel = c.Personnel == null ? "" : c.Personnel,
        //      Department = c.Department == null ? "" : c.Department,
        //      PersonConcern = c.PersonConcern == null ? "" : c.PersonConcern,
        //      Concern = c.Concern == null ? "" : c.Concern,
        //      Details = c.Details == null ? "" : c.Details,
        //      Remarks = c.Remarks == null ? "" : c.Remarks,
        //      Status = c.Status == null ? "" : c.Status,
        //      DoneBy = c.DoneBy == null ? "" : c.DoneBy,
        //      DateDone = c.DateDone.ToString() == null ? "": c.DateDone.ToString(),
        //      DateIn = c.DateIN.ToString() == null ? "" : c.DateIN.ToString(),
              
        //    }).Where(x=>x.Status=="DONE").ToList());
        //}
    }
}