﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkoProject.Models;
using WorkoProject.Utils;

namespace WorkoProject.Controllers
{
    [AdminOnlyFilter]
    public class StationsController : Controller
    {
        private DBService.DB clnt = new DBService.DB();

        #region actions
        
        public ActionResult Index()
        {
            List<WorkerDC> workersDc = clnt.GetWorkers();
            List<Worker> workers = new List<Worker>();

            foreach (var w in workersDc)
            {
                workers.Add(w.TryCast<Worker>());
            }

            ViewData["Workers"] = workers;

            return View(GetStationByStatus(StationStatus.None));
        }

        [HttpPost]
        public JsonResult AddStation(Station station)
        {
            station.Status = StationStatus.Active;

            string state = string.Empty;
            string message = string.Empty;

            try
            {
                StationDC sdc = station.TryCast<StationDC>();
                int res = clnt.AddStation(sdc);

                if (res == 0)
                {
                    message = "העמדה נוספה בהצלחה";
                    state = "success";
                }
                else
                {
                    message = "כבר קיימת במערכת עמדה בשם " + station.Name;
                    state = "error";
                }
            }
            catch (Exception)
            {
                message = "אירעה שגיאה במהלך הוספת העמדה, אנא נסה שוב";
                state = "error";
            }

            return Json(new { message = message, state = state });
        }

        [HttpPost]
        public JsonResult GetStations()
        {
            return Json(new { html = this.RenderPartialToString("Partials/_StationsList", GetStationByStatus(StationStatus.None)) });
        }
        
        [HttpPost]
        public JsonResult LinkWorkerToStation(int workerID, int stationID)
        {
            string msg;

            try
            {
                clnt.LinkWorkerToStation(workerID, stationID);

                msg = "success";
            }
            catch (Exception)
            {
                msg = "error";
            }


            return Json(new { msg = msg });
        }

        [HttpPost]
        public JsonResult UnLinkWorkerToStation(int workerID, int stationID)
        {
            string msg;

            try
            {
                clnt.UnLinkWorkerToStation(workerID, stationID);

                msg = "success";
            }
            catch (Exception)
            {
                msg = "error";
            }


            return Json(new { msg = msg });
        }

        [HttpPost]
        public JsonResult GetWorkersByStationId(int stationID)
        {
            return Json(new { workers = clnt.GetWorkersByStationID(stationID) });
        }


        [HttpPost]
        public JsonResult UpdateStation(string name, string description, int status, int id)
        {
            Message message = new Message()
            {
                Id = "update-station-modal",
                Title = "עדכון פרטי עמדה"
            };

            StationDC station = new StationDC()
            {
                Id = id,
                Name = name, 
                Description = description,
                Status = (StationStatus)status
            };

            try
            {
                int res = clnt.UpdateStation(station);

                if (res == 0)
                {
                    message.Content = "פרטי העמדה עודכנו בהצלחה";
                }
                else
                {
                    message.Content = "כבר קיימת במערכת עמדה בשם " + name;
                }
            }
            catch (Exception)
            {
                message.Content = "אירעה שגיאה במהלך עדכון פרטי העמדה, אנא נסה שוב";
            }


            var html = this.RenderPartialToString("Partials/_ModalMessage", message);

            return Json(new { html = html });
        }

        #endregion

        #region private methods

        private List<Station> GetStationByStatus(StationStatus status)
        {
            List<StationDC> sdc = clnt.GetStations(status);
            List<Station> stations = new List<Station>();

            foreach (StationDC s in sdc)
            {
                stations.Add(s.TryCast<Station>());
            }

            return stations;
        }

        #endregion
    }
}