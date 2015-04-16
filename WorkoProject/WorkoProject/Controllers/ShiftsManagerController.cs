using Entities;
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
            model.WSID = clnt.GetWSID();
            model.WorkerId = SessionManager.CurrentWorker.IdNumber;
            model.Constrains = clnt.GetWorkerConstrains(model.WorkerId, model.WSID);

            return View(model);
        }

        [HttpPost]
        public ActionResult ConstrainsSubmission(ShiftsConstrains model)
        {
            ShiftsConstrainsDC sc = new ShiftsConstrainsDC()
            {
                Constrains = model.Constrains,
                WorkerId = model.WorkerId,
                WSID = model.WSID
            };

            if (clnt.AddWorkerConstrains(sc) == 1)
            {
                TempData["Success"] = true;
            }
            else
            {
                TempData["ErrorMessgae"] = new Message()
                {
                    Title = "אירעה שגיאה",
                    Content = "אירעה שגיאה בעת הזנת האילוצים, נסה שנית."
                };
            }

            return View(model);
        }

        public ActionResult ScheduleConstrains()
        {
            List<StationDC> lsdc = clnt.GetStations(Entities.StationStatus.None);

            ScheduleConstrains model = new ScheduleConstrains();
            model.WSID = clnt.GetWSID();
            foreach (StationDC s in lsdc)
            {
                model.Stations.Add(s.TryCast<Station>());
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ScheduleConstrains(ScheduleConstrains model)
        {
            ScheduleConstrainsDC sc = new ScheduleConstrainsDC();
            sc.WSID = model.WSID;
            foreach (Station s in model.Stations)
            {
                sc.Stations.Add(s.TryCast<StationDC>());
            }

            if (clnt.AddStationConstrains(sc) == 1)
            {
                TempData["Success"] = true;
            }
            else
            {
                TempData["ErrorMessgae"] = new Message()
                {
                    Title = "אירעה שגיאה",
                    Content = "אירעה שגיאה בעת הזנת האילוצים, נסה שנית."
                };
            }

            return View(model);
        }
    }
}