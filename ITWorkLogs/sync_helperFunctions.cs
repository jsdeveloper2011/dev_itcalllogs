using ITWorkLogs.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITWorkLogs
{
    public class synchelperFunctions : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //ot = other tasks | all tasks
        public static List<SelectListItem>ot_Status()
        {
            var statuslist = new List<SelectListItem>();

            statuslist.Add(new SelectListItem() { Text = "ONGOING TASKS", Value = "ONGOING" });
            statuslist.Add(new SelectListItem() { Text = "PENDING TASKS", Value = "PENDING" });
            statuslist.Add(new SelectListItem() { Text = "COMPLETED TASKS", Value = "DONE" });

            return statuslist.ToList();
        }

        public IEnumerable<Sync> syncPagedList(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            int pageSize = 25;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Sync> taskList = null;

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
                taskList = db.sync.Where(x => (x.Branches ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Reason ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Downloading ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Uploading ?? "").ToLower().Contains(searchString.ToLower())).Where(x=>x.Personnel==users.FullName && x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                taskList = db.sync.Where(x => x.Personnel == users.FullName && x.Status == statusTask).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.Page = page;
            ViewBag.statusTask = statusTask;

            return taskList;
        }

        ////////////// for system offline / all manual branches
        public IEnumerable<sysOfflines> sysOfflinePagedList(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            int pageSize = 25;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<sysOfflines> taskList = null;

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
                taskList = db.sysoffline.Where(x => (x.Branches?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.personConcern ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Details ?? "").ToLower().Contains(searchString.ToLower())).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }
            else
            {
                taskList = db.sysoffline.OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.Page = page;

            return taskList;
        }

        public IEnumerable<ref_sysOfflines> refsysOfflinePagedList(int sysOfflineId,string sortOrder, string currentFilter, string searchString, int? page)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            int pageSize = 10;
            int pageIndex = page ?? 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var viewModel = db.ref_sysoffline.ToList();

            IPagedList<ref_sysOfflines> taskList = null;

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
                taskList = db.ref_sysoffline.Where(x => (x.DateIn.ToString() ?? "").ToLower().Contains(searchString.ToLower())
                                        || (x.Remarks ?? "").ToLower().Contains(searchString.ToLower())).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }
            else
            {

               taskList = db.ref_sysoffline.Where(x=>x.sysOfflineId == sysOfflineId).OrderByDescending(x => x.DateCreated).ToPagedList(pageIndex, pageSize);
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.Page = page;

            return taskList;
        }
    }
}