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
    public class RequestsController : Controller
    {
        private DBService.DB clnt = new DBService.DB();
        
        public ActionResult SendRequest()
        {
            return View(new Request() { WorkerId = SessionManager.CurrentWorker.IdNumber });
        }

        [HttpPost]
        public ActionResult SendRequest(Request request)
        {
            request.Date = DateTime.Now;

            if (clnt.AddWorkerRequest(request) == 1)
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

            return View(new Request() { WorkerId = SessionManager.CurrentWorker.IdNumber });
        }

        [AdminOnlyFilter]
        public ActionResult GetUnreadWorkersRequests()
        {
            return Json(new { requests = clnt.GetUnreadWorkersRequests().ToList()});
        }

        public ActionResult WorkersRequestsList()
        {
            List<Request> requests = clnt.GetUnreadWorkersRequests();
            List<Worker> workers = clnt.GetWorkers();
            ViewData["workers"] = workers;

            return View(requests);
        }

        [HttpPost]
        public ActionResult UpdateWorkerRequest(string requestId)
        {
            try
            {
                clnt.UpdateWorkerRequest(requestId);
            }
            catch
            {

            }
            return Json(new { });
        }
    }
}