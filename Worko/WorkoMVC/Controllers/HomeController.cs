using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkoMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Empty()
        {
            return View();
        }

        public ActionResult AngularTest()
        {
            return View("AngularTest");
        }
    }
}
