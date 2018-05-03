using ITWorkLogs.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ITWorkLogs.Controllers
{
    public class SysOfflineController : synchelperFunctions
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SysOffline
        public ActionResult Index()
        {
            return View("Index");
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            sysOfflines sysof = await db.sysoffline.FindAsync(id);
            if (sysof == null)
            {
                return HttpNotFound();
            }

            return View(sysof);
        }

        public ActionResult partialsysOffline(string sortOrder, string currentFilter, string searchString, int? page)
        { 
            return PartialView(sysOfflinePagedList(sortOrder, currentFilter, searchString, page));
        }

        //sysOfflines
        public PartialViewResult addSysOffline()
        {
            ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js
            sysOfflineViewModel viewModel = new sysOfflineViewModel     
            {
                newSysOffline = new sysOfflines()
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> addSysOffline(sysOfflineViewModel viewModel, string sortOrder, string currentFilter, string searchString, int? page)
        {
            string Details = viewModel.newSysOffline.Details.ToUpper();
            string Branches = viewModel.newSysOffline.Branches.ToUpper();

            bool dataCheck = this.checkData(Details,Branches);

            if (dataCheck)  
            {
                var sysoff = await db.sysoffline.FirstOrDefaultAsync(m => m.Details == Details && m.Branches == Branches);
                return PartialView(sysOfflinePagedList(sortOrder, currentFilter, searchString, page));
            }
            else
            {
                ViewBag.Branches = new SelectList(db.branches.ToList().OrderBy(x => x.BRANCHNAME), "BRANCHNAME", "BRANCHNAME"); //list of branches  - get in branches table - js

                var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...
                viewModel.newSysOffline.CreatedBy = users.FullName.ToUpper();
                viewModel.newSysOffline.DateCreated = DateTime.Now;

                //convert to uppercase...
                viewModel.newSysOffline.Branches = viewModel.newSysOffline.Branches.ToUpper();
                viewModel.newSysOffline.personConcern = viewModel.newSysOffline.personConcern.ToUpper();
                viewModel.newSysOffline.Details = viewModel.newSysOffline.Details.ToUpper();
                db.sysoffline.Add(viewModel.newSysOffline);
                await db.SaveChangesAsync();
            }

            return PartialView(sysOfflinePagedList(sortOrder, currentFilter, searchString, page));
        }
        
        //###################################################################
        public ActionResult partialrefsysOffline(int sysOfflineId,string sortOrder, string currentFilter, string searchString, int? page)
        {
            return PartialView(refsysOfflinePagedList(sysOfflineId,sortOrder, currentFilter, searchString, page));
        }

        public PartialViewResult addrefSysOffline(int id)
        {
            ref_sysOfflineViewModel viewmodel = new ref_sysOfflineViewModel
            {
                newrefSysOffline = new ref_sysOfflines(),
                ref_sysOffline = db.ref_sysoffline.OrderByDescending(x=>x.DateCreated).Where(m => m.sysOfflineId == id && m.Status == "ACTIVE")
            };

            return PartialView(viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> addrefSysOffline(int id, ref_sysOfflineViewModel viewModel)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            viewModel.newrefSysOffline.sysOfflineId = id;
            viewModel.newrefSysOffline.CreatedBy = users.FullName.ToUpper();
            viewModel.newrefSysOffline.DateCreated = DateTime.Now;

            //convert to uppercase...
            viewModel.newrefSysOffline.Remarks = viewModel.newrefSysOffline.Remarks.ToUpper();
            viewModel.newrefSysOffline.Status = "ACTIVE";
            db.ref_sysoffline.Add(viewModel.newrefSysOffline);
            await db.SaveChangesAsync();
                
            return PartialView("partialrefsysOffline", await db.ref_sysoffline.Where(m => m.sysOfflineId == id && m.Status == "ACTIVE").OrderByDescending(x=>x.DateCreated).ToListAsync());
        }

        public async Task<ActionResult> DeleterefSysOffline(int id)
        {
            ref_sysOfflines model = await db.ref_sysoffline.SingleOrDefaultAsync(m => m.ref_sysOfflineId == id);

            if (model != null)
            {
                model.Status = "CN";
                await db.SaveChangesAsync();
                return PartialView("partialrefsysOffline", await db.ref_sysoffline.Where(m => m.sysOfflineId == model.sysOfflineId && m.Status== "ACTIVE").OrderByDescending(x => x.DateCreated).ToListAsync());
            }
            return PartialView("partialrefsysOffline", await db.ref_sysoffline.Where(m => m.sysOfflineId == model.sysOfflineId && m.Status == "ACTIVE").OrderByDescending(x => x.DateCreated).ToListAsync());
        }

        // GET: /Phone/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            var sysoff = db.sysoffline.Find(id);
            if (sysoff == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", sysoff);
        }

        // POST: WorkLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(sysOfflines sysoffline)
        {
            var users = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name); //list of users...

            if (ModelState.IsValid)
            {   
                //convert to uppercase..
                sysoffline.ModifiedBy = users.FullName.ToUpper();
                sysoffline.DateModified = DateTime.Now;

                db.Entry(sysoffline).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("Edit", sysoffline);
        }
        //check names...

        public bool checkData(string Details, string newSysOffline_Branches)
        {
            var model = db.sysoffline.Where(m => m.Details == "NO INTERNET" && m.Branches == newSysOffline_Branches).ToList();

            if (model.Count == 0)
            {
                return false;
            }
            else
            {
                var newModel = model.Last();

                if (newModel.DateConnected == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
