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

            int wsid = clnt.GetWSID();

            List<ScheduleConstrainsDC> model = clnt.GetStationConstrains(wsid);

            ViewData["WSID"] = wsid;
            ViewData["Stations"] = lsdc;

            return View(model);
        }

        [HttpPost]
        public JsonResult AddSchduleConstrain(int wsid, int stationId, int day, int shiftTime)
        {
            int res = clnt.AddStationConstrains(stationId, wsid, day, shiftTime);

            if (res == 1)
            {

            }
            else
            {

            }

            return Json(new { });
        }

        [HttpPost]
        public JsonResult RemoveSchduleConstrain(int wsid, int stationId, int day, int shiftTime)
        {
            int res = clnt.RemoveStationConstrains(stationId, wsid, day, shiftTime);

            if (res == 1)
            {

            }
            else
            {

            }

            return Json(new { });
        }
    }
}