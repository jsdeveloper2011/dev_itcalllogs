using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITWorkLogs.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "IT Work Logs.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "JOHN SONNY CRUZ";

            return View();
        }
    }


}