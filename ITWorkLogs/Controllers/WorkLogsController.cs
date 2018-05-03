using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;           
using ITWorkLogs.Models;
using System.Linq; 
using System;
using System.Web;

namespace ITWorkLogs.Controllers
{ 
    [Authorize]
    public class WorkLogsController : functions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult mtaskconcern()
        {
            ViewData["statusTask"] = manualList();
            return View();
        }

        public ActionResult Manualtasks( string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if(statusTask == "NOINTERNET") {
                ViewBag.status = true;
            }
            return PartialView(manualConcernPagedlist(statusTask, sortOrder, currentFilter, searchString, page));
        }

        // GET: WorkLogs/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkLogs workLogs = await db.workLogs.FindAsync(id); 
            if (workLogs == null)
            {
                return HttpNotFound();
            } 
            return PartialView("_Details", workLogs);
        }

        //WORKLOGS
        public PartialViewResult addWork()
        {
            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("admin")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            ViewBag.fullname = users.FullName;

            //#####################################################################//

            //Note: I didn't use the viewModel because  we must check/change the status before updating the partial view  so  i declare list of worklogs here.
            //checking if have a ongoing to be pending...
            var worklogs = db.workLogs.ToList();// list of worklogs needed in foreach
            var dateNow = DateTime.Now; //get date time now
            foreach (var tasks in worklogs.Where(x => x.Status == "ONGOING" && x.DateCreated.ToString("MM/dd/yyyy") != dateNow.ToString("MM/dd/yyyy")))
            {
                if (tasks != null) // add this
                {
                    var model = db.workLogs.Find(tasks.WorkID);
                    model.Status = "PENDING";
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            WorkLogsViewModel viewModel = new WorkLogsViewModel
             {
                newWorkLogs = new WorkLogs(),
             };

            ViewData["statusTask"] = GetStatus();
            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> addWork( WorkLogsViewModel viewModel,string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            string YearMonth = DateTime.Now.ToString("yy") + DateTime.Now.Month.ToString("d2"); //year and month - ex.1702

            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("itdept") && !u.FullName.Contains("admin")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc
    
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            viewModel.newWorkLogs.ControlID = YearMonth + " - IT" + getcontrolNum(); //workLogs.WorkID.ToString("D4"); /
            viewModel.newWorkLogs.CreatedBy = users.FullName.ToUpper();
            viewModel.newWorkLogs.DateCreated = DateTime.Now;
            viewModel.newWorkLogs.DateIN = DateTime.Now;

            //convert to uppercase...
            viewModel.newWorkLogs.Department = viewModel.newWorkLogs.Department.ToUpper();
            viewModel.newWorkLogs.Personnel = viewModel.newWorkLogs.Personnel.ToUpper();
            viewModel.newWorkLogs.PersonConcern = viewModel.newWorkLogs.PersonConcern.ToUpper();
            viewModel.newWorkLogs.Concern = viewModel.newWorkLogs.Concern.ToUpper();
            viewModel.newWorkLogs.Details = viewModel.newWorkLogs.Details.ToUpper();
            //condition in list of ebt manual...
            if (viewModel.newWorkLogs.Personnel == "ITDEPT")
            {
                viewModel.newWorkLogs.Status = "MANUAL";
            }
            else
            {
                viewModel.newWorkLogs.Status = "ONGOING";
            }

            db.workLogs.Add(viewModel.newWorkLogs);
            await db.SaveChangesAsync();

            return PartialView(workPagedlist(statusTask, sortOrder, currentFilter, searchString, page));
        }

        //GET
        public ActionResult Done (int? id, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var tasklist = workPagedlist(statusTask, sortOrder, currentFilter, searchString, page);// check workPagedlist function
            return PartialView("partialworkLogs", tasklist);
        }

        //POST OF ACTIONRESULT "DONE"
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Remarks(int? id, string statusTask, string remarks, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            var model = db.workLogs.FirstOrDefault(x => x.WorkID == id);

            model.Remarks = remarks.ToUpper();
            model.Status = "DONE";
            model.DateDone = DateTime.Now;
            model.DoneBy = users.FullName.ToUpper();
            db.SaveChanges();
            var tasklist = workPagedlist(statusTask, sortOrder, currentFilter, searchString, page);// check workPagedlist function
            return PartialView("partialworkLogs", tasklist);
        }


        //CANCEL --- //////////////////////////////

        //GET
        public ActionResult Cancel(int? id,string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var tasklist = workPagedlist(statusTask, sortOrder, currentFilter, searchString, page);// check workPagedlist function
            return PartialView("partialworkLogs", tasklist);
        }       

        //POST  
        public ActionResult Cancelled(int? id, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            var model = db.workLogs.FirstOrDefault(x => x.WorkID == id);
            model.Status = "CN"; 
            model.DateCanceled = DateTime.Now;
            model.CancelBy = users.FullName.ToUpper();

            db.SaveChanges();
            var tasklist = workPagedlist(statusTask, sortOrder, currentFilter, searchString, page); // check workPagedlist function
            return PartialView("partialworkLogs", tasklist);
        }

        //EDIT --- //////////////////////////////

        // GET: /Phone/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {          
            var worklogs = db.workLogs.Find(id);

            if (worklogs == null)
            {
                return HttpNotFound();
            }

            var branches = db.branches.Where(s => s.BRANCHNAME + " " + "(" + s.BRANCHCODE + ")" != null)
                  .Select(s => new SelectListItem
                  {
                      Value = s.BRANCHNAME + " " + "(" + s.BRANCHCODE + ")",
                      Text =  s.BRANCHNAME + " " + "(" + s.BRANCHCODE + ")"
                  });

            ViewBag.Branches = new SelectList(branches, "Value", "Text"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u =>  !u.FullName.Contains("admin")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            return PartialView("Edit", worklogs);
        }

        // POST: WorkLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WorkID,ControlID,Personnel,Department,PersonConcern,Concern,Details,Remarks,Status,DoneBy,DateDone,CancelBy,DateCanceled,TransferBy,DateTransfered,CreatedBy,DateCreated,ModifiedBy,DateModified,DateIn")] WorkLogs workLogs)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            var branches = db.branches.Where(s => s.BRANCHNAME + " " + "(" + s.BRANCHCODE + ")" != null)
                   .Select(s => new SelectListItem
                   {
                       Value = s.BRANCHNAME + " " + "(" + s.BRANCHCODE + ")",
                       Text = s.BRANCHNAME + " " +  "(" + s.BRANCHCODE + ")"
                   });

            ViewBag.Branches = new SelectList(branches, "Value", "Text"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u =>  !u.FullName.Contains("admin")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc
                
            if (ModelState.IsValid)
            {
                if (workLogs.Personnel == "ITDEPT")
                {
                    workLogs.Status = "MANUAL";
                }

                //convert to uppercase..
                workLogs.Department = workLogs.Department.ToUpper();
                workLogs.Personnel = workLogs.Personnel.ToUpper();
                workLogs.PersonConcern = workLogs.PersonConcern.ToUpper();
                workLogs.Concern = workLogs.Concern.ToUpper();
                workLogs.Details = workLogs.Details.ToUpper();
                workLogs.ModifiedBy = users.FullName.ToUpper();
                workLogs.DateModified = DateTime.Now;

                db.Entry(workLogs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("Edit", workLogs);
        }

        //Partial View
        public ActionResult partialworklogs(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if(statusTask == "ENCODED")
            {
                ViewBag.Personnel = true;//list of all encoded tasks... including other personnel that user encoded...
            }
            return PartialView(workPagedlist(statusTask, sortOrder, currentFilter,searchString,page));
        }

        //AUTOCOMPLETE-------------------------------------------------------------------------------------------------------

        //autoCompleteBranches
        public JsonResult autoCompleteBranches(string term)
        {
            var model = db.branches.ToList();
            var result = (from x in model
                          where (x.BRANCHNAME.ToUpper()).Contains(term.ToUpper())
                          select new
                          {
                              x.BRANCHCODE,
                              x.BRANCHNAME
                          }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //autoCompletePersonConcern 
        public JsonResult autoCompletePersonConcern(string term)
        {
            var model = db.workLogs.ToList();
            var result = (from x in model
                          where (x.PersonConcern.ToUpper()).Contains(term.ToUpper())
                          select new
                          {
                              x.PersonConcern
                          }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //autoCompleteConcern 
        public JsonResult autoCompleteConcern(string term)
        {
            var model = db.workLogs.ToList();
            var result = (from x in model
                          where (x.Concern.ToUpper()).Contains(term.ToUpper())
                          select new
                          {
                              x.Concern
                          }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //autoCompleteDetails 
        public JsonResult autoCompleteDetails(string term)
        {
            var model = db.workLogs.ToList();
            var result = (from x in model
                          where (x.Details.ToUpper()).Contains(term.ToUpper())
                          select new
                          {
                              x.Details
                          }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //-------------------------------------------------------------------------//

        //Other tasks
        //###################################################################################

        public ActionResult otherTasks()
        {
            ViewData["statusTask"] = ot_Status();
            return View();
        }

        public ActionResult partialothertasks(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            return PartialView(othertasksPagedlist(statusTask, sortOrder, currentFilter, searchString, page));
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
