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

            ViewData["NextWeekStartDate"] = clnt.GetWeekStartDate();

            return View(model);
        }

        [HttpPost]
        public ActionResult ConstrainsSubmission(ShiftsConstrains model)
        {
            ShiftsConstrains sc = new ShiftsConstrains()
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
            List<Station> lsdc = clnt.GetStations(Entities.StationStatus.None);

            int wsid = clnt.GetWSID();

            List<ScheduleConstrains> model = clnt.GetStationConstrains(wsid);

            ViewData["WSID"] = wsid;
            ViewData["Constrains"] = model;
            ViewData["NextWeekStartDate"] = clnt.GetWeekStartDate();

            return View(lsdc);
        }

        [HttpPost]
        public ActionResult ScheduleConstrains(int wsid)
        {
            List<Station> lsdc = clnt.GetStations(Entities.StationStatus.None);
            List<ScheduleConstrains> model = new List<ScheduleConstrains>();

            clnt.RemoveAllStationConstrains(wsid);

            ViewData["WSID"] = wsid;
            ViewData["Stations"] = lsdc;

            return View(model);
        }


        [HttpPost]
        public JsonResult SetShiftSchduleConstrain(int wsid, int stationId, int day, int shiftTime, int status, int numOfWorkers, int priority)
        {
            int res = clnt.AddStationConstrains(stationId, wsid, day, shiftTime, status, numOfWorkers, priority);

            if (res == 1)
            {

            }
            else
            {

            }

            return Json(new { });
        }
        
        [HttpPost]
        public ActionResult WeeklyStationsConstrains(Station s)
        {
            return View();
        }

        [AdminOnlyFilter]
        public ActionResult Schedule()
        {
            WorkoAlgorithm.GenerateWorkSchedule();
            ViewData["Stations"] = clnt.GetStations(StationStatus.None);
            SessionManager.CurrentWorkSchedule = WorkoAlgorithm.workSchedule;
            return View(WorkoAlgorithm.workSchedule);
        }

        public ActionResult WeeklySchedule()
        {
            if (SessionManager.CurrentWorkSchedule == null)
            {
                var ws = clnt.GetWeeklySchedule(clnt.GetWSID());
                SessionManager.CurrentWorkSchedule = ws;
            }
            ViewData["Stations"] = clnt.GetStations(StationStatus.None);
            TempData["Watch"] = true;
            return View("Schedule", SessionManager.CurrentWorkSchedule);
        }

        [AdminOnlyFilter]
        [HttpPost]
        public ActionResult SubmitScheduleConstrains()
        {
            clnt.SetNextWeek();
            return RedirectToAction("ScheduleConstrains");
        }

        [AdminOnlyFilter]
        [HttpPost]
        public ActionResult CreateWeeklySchedule()
        {
            var ws = SessionManager.CurrentWorkSchedule;

            clnt.CreateWorkSchedule(ws);

            return RedirectToAction("Index");
        }

    }
}
