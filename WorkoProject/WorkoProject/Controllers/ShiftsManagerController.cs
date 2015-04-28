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
            WorkoAlgorithm.GenerateWorkSchedule();
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
            ViewData["Constrains"] = model ;

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

    }
}
