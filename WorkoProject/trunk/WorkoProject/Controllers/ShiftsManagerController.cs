using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkoProject.Models;
using WorkoProject.Utils;

namespace WorkoProject.Controllers
{
    [VerifyUserFilter]
    public class ShiftsManagerController : Controller
    {
        private DBService.DB clnt = new DBService.DB();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConstrainsSubmission()
        {
            ShiftsConstrains model = new ShiftsConstrains();

            ///TODO: get current constrains if exists(for next week only)
            ///      to let the worker edit his choices

            return View(model);
        }

        [HttpPost]
        public ActionResult ConstrainsSubmission(ShiftsConstrains model)
        {
            return View();
        }

        public ActionResult ScheduleConstrains()
        {
            ViewData["Stations"] = clnt.GetStations(Entities.StationStatus.None);

            return View();
        }

        [HttpPost]
        public ActionResult ScheduleConstrains(ScheduleConstrains model)
        {
            return View();
        }
    }
}