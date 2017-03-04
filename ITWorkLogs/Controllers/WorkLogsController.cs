using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using ITWorkLogs.Models;
using System.Linq;
using System;

namespace ITWorkLogs.Controllers
{
    [Authorize]
    public class WorkLogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        // GET: WorkLogs/Details/5
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
            return View(workLogs);
        }

        //workLogs
        public ActionResult addWork()
        {
            WorkLogsViewModel viewModel = new WorkLogsViewModel
            {
                newWorkLogs = new WorkLogs(),
                worklogs = db.workLogs
            };

            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHCODE), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            ViewBag.Name = new SelectList(db.Users.ToList().OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc

            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> addWork( WorkLogsViewModel viewModel)
        {
            if (viewModel.worklogs != null)
            {
                viewModel.newWorkLogs = new WorkLogs();
                viewModel.worklogs = db.workLogs;
                return PartialView(viewModel);
            }
            else
            {
                string YearMonth = DateTime.Now.ToString("yy") + DateTime.Now.Month.ToString("d2"); //year and month - ex.1702

                ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHCODE), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
                ViewBag.Name = new SelectList(db.Users.ToList().OrderBy(x => x.FullName), "FullName", "FullName");// list of users - get in users table - jsc
                viewModel.newWorkLogs.Status = "ONGOING";
                viewModel.newWorkLogs.ControlID = YearMonth + " - IT" + getcontrolNum(); //workLogs.WorkID.ToString("D4"); /
                viewModel.newWorkLogs.CreatedBy = User.Identity.Name;
                viewModel.newWorkLogs.DateCreated = DateTime.Now;
                db.workLogs.Add(viewModel.newWorkLogs);
            
                await db.SaveChangesAsync();

                return PartialView("partialworkLogs", await db.workLogs.ToListAsync());
            }
        }

        // GET: WorkLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            return View(workLogs);
        }

        // POST: WorkLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WorkID,ControlNo,Personnel,Department,PersonConcern,Concern,Details,Remarks,Status,DoneBy,DateDone,CancelBy,DateCanceled,TransferBy,DateTransfered,CreatedBy,DateCreated,ModifiedBy,DateModified")] WorkLogs workLogs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workLogs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(workLogs);
        }

        // GET: WorkLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(workLogs);
        }

        // POST: WorkLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WorkLogs workLogs = await db.workLogs.FindAsync(id);
            db.workLogs.Remove(workLogs);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //function get controlNumber
        public string getcontrolNum()
        {
            var controlNum = "0";

            var listWorkLogs =  db.workLogs.ToList(); //list of worklogs
            var maxControlId = listWorkLogs.Max(x => x.ControlID);

            if (maxControlId == null ) 
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
