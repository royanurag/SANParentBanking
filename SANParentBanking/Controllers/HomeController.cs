using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SANParentBanking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinancialRes()
        {
            ViewBag.Message = "Financial Resources";

            return View();
        }

        public ActionResult ProjStat()
        {
            ViewBag.Message = "Project Status";

            return View();
        }
    }
}