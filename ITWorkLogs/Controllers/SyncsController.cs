using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITWorkLogs.Models;

namespace ITWorkLogs.Controllers
{
    public class SyncsController : synchelperFunctions
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Syncs
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult partialsynclogs(string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            return PartialView(syncPagedList(statusTask, sortOrder, currentFilter, searchString, page));
        }

        // GET: Syncs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sync sync = await db.sync.FindAsync(id);
            if (sync == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", sync);
        }

        //sync
        public PartialViewResult addSync()
        {
            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("admin") && !u.FullName.Contains("itdept")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            //Note: I didn't use the viewModel because  we must check/change the status before updating the partial view  so  i declare list of worklogs here.
            //checking if have a ongoing to be pending...
            var synclist = db.sync.ToList();// list of worklogs needed in foreach
            var dateNow = DateTime.Now; //get date time now
            foreach (var tasks in synclist.Where(x => x.Status == "ONGOING" && x.DateCreated.ToString("MM/dd/yyyy") != dateNow.ToString("MM/dd/yyyy")))
            {
                if (tasks != null) // add this
                {
                    var model = db.sync.Find(tasks.SynchId);
                    model.Status = "PENDING";
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            SyncViewMoodel viewModel = new SyncViewMoodel
            {
                newSync = new Sync(),
            };

            ViewData["statusTask"] = ot_Status();
            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> addSync (SyncViewMoodel viewModel, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("admin") && !u.FullName.Contains("itdept")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            viewModel.newSync.CreatedBy = users.FullName.ToUpper();
            viewModel.newSync.DateCreated = DateTime.Now;
            viewModel.newSync.DateIn = DateTime.Now;

            //convert to uppercase...
            viewModel.newSync.Branches = viewModel.newSync.Branches.ToUpper();
            viewModel.newSync.Personnel = viewModel.newSync.Personnel.ToUpper();
            viewModel.newSync.Reason = viewModel.newSync.Reason.ToUpper();
            viewModel.newSync.Downloading = viewModel.newSync.Downloading.ToUpper();
            viewModel.newSync.Uploading = viewModel.newSync.Uploading.ToUpper();
            viewModel.newSync.Status = "ONGOING";
            viewModel.newSync.Others = false;
            db.sync.Add(viewModel.newSync);
            await db.SaveChangesAsync();

            return PartialView(syncPagedList(statusTask, sortOrder, currentFilter, searchString, page));
        }

        public ActionResult timestarted (int? id, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            var model = db.sync.FirstOrDefault(x => x.SynchId == id);
            model.TimeStarted = DateTime.Now;
            model.Others = false;
            model.Status = "ONGOING";
            model.ModifiedBy = users.FullName.ToUpper();
            model.DateModified = DateTime.Now;

            db.SaveChanges();
            var tasklist = syncPagedList(statusTask, sortOrder, currentFilter, searchString, page); // check workPagedlist function
            return PartialView("partialsynclogs", tasklist);
        }

        public ActionResult timeended(int? id, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
            var model = db.sync.FirstOrDefault(x => x.SynchId == id);
            model.TimeEnded = DateTime.Now;
            model.Status = "DONE";
            model.ModifiedBy = users.FullName.ToUpper();
            model.DateModified = DateTime.Now;
            db.SaveChanges();

            var tasklist = syncPagedList(statusTask, sortOrder, currentFilter, searchString, page); // check workPagedlist function
            return PartialView("partialsynclogs", tasklist);
        }

        public ActionResult GETProblemEncountered (int? id, string statusTask, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (id == null)
            {
                throw new HttpException(404, "ID is null");
            }

            var tasklist = syncPagedList(statusTask, sortOrder, currentFilter, searchString, page); // check workPagedlist function
            return PartialView("partialsynclogs", tasklist);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult POSTProblemEncountered(ref_syncerrors model ,int syncid, string statusTask, string problem, string solution, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (syncid == 0)
            {
                throw new HttpException(404, "ID is null");
            }

            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            if (solution == "" || problem == "")
            {
                ViewBag.notification = true;
            }
            else
            {
                model.SyncId = syncid;
                model.Solution = solution;
                model.ProblemEncountered = problem;
                model.CreatedBy = users.FullName;
                model.DateCreated = DateTime.Now;
                db.syncerrror.Add(model);
                db.SaveChanges();
            }

            var tasklist = syncPagedList(statusTask, sortOrder, currentFilter, searchString, page); // check workPagedlist function
            return PartialView("partialsynclogs", tasklist);
        }

        // GET: /Phone/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var syncs = db.sync.Find(id);

            if (syncs == null)
            {
                return HttpNotFound();
            }

            var branches = db.branches.Where(s => s.BRANCHNAME != null)
                  .Select(s => new SelectListItem
                  {
                      Value = s.BRANCHNAME,
                      Text = s.BRANCHNAME 
                  });

            ViewBag.branchesList = new SelectList(branches, "Value", "Text"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("admin") && !u.FullName.Contains("itdept")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            return PartialView("Edit", syncs);
        }

        // POST: WorkLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Sync sync)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            var branches = db.branches.Where(s => s.BRANCHNAME != null)
                  .Select(s => new SelectListItem
                  {
                      Value = s.BRANCHNAME,
                      Text = s.BRANCHNAME
                  });


            ViewBag.branchesList = new SelectList(branches, "Value", "Text"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().Where(u => !u.FullName.Contains("admin")).OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            if (ModelState.IsValid)
            {
                //convert to uppercase..
                sync.Branches = sync.Branches.ToUpper();
                sync.Personnel = sync.Personnel.ToUpper();
                sync.Reason = sync.Reason.ToUpper();
                sync.ModifiedBy = users.FullName.ToUpper();
                sync.DateModified = DateTime.Now;

                db.Entry(sync).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("Edit", sync);
        }

        // GET: Syncs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sync sync = await db.sync.FindAsync(id);
            if (sync == null)
            {
                return HttpNotFound();
            }
            return View(sync);
        }

        // POST: Syncs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Sync sync = await db.sync.FindAsync(id);
            db.sync.Remove(sync);
            await db.SaveChangesAsync();
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
