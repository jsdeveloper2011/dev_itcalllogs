using ITWorkLogs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITWorkLogs
{
    public class functions : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //function get controlNumber
        public string getcontrolNum()
        {
            var controlNum = "0";

            var listWorkLogs = db.workLogs.ToList(); //list of worklogs
            var maxControlId = listWorkLogs.Max(x => x.ControlID);//get max controlID

            if (maxControlId == null)
            {
                controlNum = 1.ToString("D4");
            }
            else
            {
                string YearMonth = DateTime.Now.ToString("yy") + DateTime.Now.Month.ToString("d2"); //get year and month today
                var firstCId = maxControlId.Substring(0, 4);

                if (firstCId != YearMonth) // if not equal  year/month today then back to 0001 
                {
                    controlNum = 1.ToString("D4");
                }
                else
                {
                    var maxcId = maxControlId.Substring(maxControlId.Length - 4);// last 4 digits
                    int maxId = Convert.ToInt32(maxcId); //convert string to int
                    maxId++;// add 1
                    controlNum = maxId.ToString("D4"); //convert to string 
                }
            }
            return controlNum;
        }
        //user tasks
        public static List<SelectListItem> GetStatus()
        {
            var statuslist = new List<SelectListItem>();

            statuslist.Add(new SelectListItem() { Text = "ONGOING TASKS", Value = "ONGOING" });
            statuslist.Add(new SelectListItem() { Text = "PENDING TASKS", Value = "PENDING" });
            statuslist.Add(new SelectListItem() { Text = "COMPLETED TASKS", Value = "DONE" });
            statuslist.Add(new SelectListItem() { Text = "ENCODED TASKS", Value = "ENCODED" });
            statuslist.Add(new SelectListItem() { Text = "DELETED TASKS", Value = "CN" });

            return statuslist.ToList();
        }
        //ot = other tasks | all tasks
        public static List<SelectListItem>ot_Status()
        {
            var statuslist = new List<SelectListItem>();

            statuslist.Add(new SelectListItem() { Text = "ONGOING TASKS", Value = "ONGOING" });
            statuslist.Add(new SelectListItem() { Text = "PENDING TASKS", Value = "PENDING" });
            statuslist.Add(new SelectListItem() { Text = "COMPLETED TASKS", Value = "DONE" });

            return statuslist.ToList();
        }

        public static List<SelectListItem> manualList()
        {
            var statusTask = new List<SelectListItem>();

            statusTask.Add(new SelectListItem() { Text = "ALL MANUAL", Value = "ALLMANUAL" });
            statusTask.Add(new SelectListItem() { Text = "NO INTERNET", Value = "NOINTERNET" });

            return statusTask.ToList();
        }

        public IEnumerable<WorkLogs> workPagedlist(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            int pageSize = 50;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<WorkLogs> taskList = null;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
                
            if (statusTask == "ENCODED")
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    taskList = db.workLogs.Where(x => (x.ControlID ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Department ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Personnel ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.PersonConcern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Concern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Status ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).Where(x => x.CreatedBy == users.FullName).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
                else
                {
                    taskList = db.workLogs.Where(x => x.CreatedBy == users.FullName).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    taskList = db.workLogs.Where(x => (x.ControlID ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Department ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Personnel ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.PersonConcern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Concern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).Where(x => x.Personnel == users.FullName && x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);

                }
                else
                {
                    taskList = db.workLogs.Where(x => x.Personnel == users.FullName && x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }

            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.Page = page;
            ViewBag.statusTask = statusTask;

            return taskList;
        }

        public IEnumerable<WorkLogs> manualConcernPagedlist(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageSize = 25;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<WorkLogs> taskList = null;


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (statusTask == "ALLMANUAL")
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    taskList = db.workLogs.Where(x => (x.ControlID ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Department ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Personnel ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.PersonConcern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Concern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).Where(x => x.Concern == "MANUAL").OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
                else
                {
                    taskList = db.workLogs.Where(x => x.Concern == "MANUAL").OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
            }
            else if(statusTask == "NOINTERNET")
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    taskList = db.workLogs.Where(x => (x.ControlID ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Department ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Personnel ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.PersonConcern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Concern ?? "").ToLower().Contains(searchString.ToLower())
                                            || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).Where(x => x.Concern == "MANUAL" && x.Details=="NO INTERNET").OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
                else
                {
                    taskList = db.workLogs.Where(x => x.Concern == "MANUAL" && x.Details == "NO INTERNET").OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
                }
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.statusTask = statusTask;
            ViewBag.Page = page;

            return taskList;
        }

        public IEnumerable<WorkLogs> othertasksPagedlist(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageSize = 25;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<WorkLogs> taskList = null;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                taskList = db.workLogs.Where(x => (x.ControlID ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Department ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Personnel ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.PersonConcern ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Concern ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).Where(x => x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            } 
            else
            {
                taskList = db.workLogs.Where(x=> x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.statusTask = statusTask;
            ViewBag.Page = page;

            return taskList;
        }

    }
}