using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WorkoProject.Models;
using Entities;
using System.IO;
using WorkoProject.Utils;
using System.Collections.Generic;

namespace WorkoProject.Controllers
{
    public class AccountController : Controller
    {
        private DBService.DB clnt = new DBService.DB();

        public AccountController()
        {
        }

        [AdminOnlyFilter]
        public ActionResult Register()
        {
            return View();
        }

        [AdminOnlyFilter]
        [HttpPost]
        public ActionResult Register(Worker model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string path = null;

                if (file != null && file.ContentLength > 0)
                {
                    path = "Images/ProfileImages/" + model.IdNumber + Path.GetExtension(file.FileName);
                    string mapPath = Server.MapPath("~/" + path);
                    file.SaveAs(mapPath);

                }



                WorkerDC worker = model.TryCast<WorkerDC>();

                var res = clnt.AddWorker(worker);

                if (res == 0)
                {
                    return RedirectToAction("Index", "ShiftsManager");
                }
                else
                {
                    var message = new Message()
                    {
                        Title = "שגיאת הרשמה",
                        Content = "תעודת הזהות כבר קיימת במערכת.",
                        Id = "error"
                    };
                    TempData["ErrorMessgae"] = message;
                    return View();
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult AddWorker(Worker worker)
        {
            string state = string.Empty;
            string message = string.Empty;

            try
            {
                WorkerDC wdc = worker.TryCast<WorkerDC>();
                int res = clnt.AddWorker(wdc);

                if (res == 0)
                {
                    message = "העובד נוסף בהצלחה";
                    state = "success";
                }
                else
                {
                    message = "כבר קיים במערכת עובד עם ת.ז: " + worker.IdNumber;
                    state = "error";
                }
            }
            catch (Exception)
            {
                message = "אירעה שגיאה במהלך הוספת העובד, אנא נסה שוב";
                state = "error";
            }

            return Json(new { message = message, state = state });
        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            SessionManager.HasConnectedUser = false;
            SessionManager.HasAdminConnected = false;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(Login worker)
        {
            var w = clnt.Login(worker.IdNumber, worker.Password);
            
            if (w == null)
            {
                var message = new Message()
                {
                    Title = "שגיאת התחברות",
                    Content = "תעודת הזהות או הסיסמא שגויים.",
                    Id = "error"
                };
                TempData["ErrorMessgae"] = message;
                return View();
            }

            SessionManager.CurrentWorker = w.TryCast<Worker>();
            SessionManager.HasConnectedUser = true;
            SessionManager.HasAdminConnected = w.IsAdmin;
            if (worker.RememberME)
            {
                TempData["stay_logged_in"] = true;
                TempData["stay_logged_in_id"] = worker.IdNumber;
            }

            return RedirectToAction("Index", "ShiftsManager");
        }

        [HttpPost]
        public JsonResult AutoLogin(string workerId)
        {

            WorkerDC w = null;
            string message = string.Empty;
            string url = string.Empty;

            if (!string.IsNullOrEmpty(workerId))
            {
                w = clnt.AutoLogin(workerId);
            }

            if (w != null)
            {
                SessionManager.CurrentWorker = w.TryCast<Worker>();
                SessionManager.HasConnectedUser = true;
                SessionManager.HasAdminConnected = w.IsAdmin;

                message = "success";
                url = Url.Action("Index", "ShiftsManager");
            }
            else
            {
                SessionManager.HasConnectedUser = false;
                SessionManager.HasAdminConnected = false;

                message = "error";
                url = Url.Action("Login", "Account");
            }

            return Json(new { Url = url, Message = message });
        }

        [AdminOnlyFilter]
        public ActionResult WorkersList()
        {
            List<WorkerDC> workersDc = clnt.GetWorkers();
            List<Worker> workers = new List<Worker>();

            foreach (var w in workersDc)
            {
                workers.Add(w.TryCast<Worker>());
            }

            return View(workers);
        }


        [HttpPost]
        public JsonResult UpdateWorker(HttpPostedFileBase userPic, string fname, string lname, string phone, string email, string id)
        {
            Message message = new Message()
            {
                Id = "update-worker-modal",
                Title = "עדכון פרטי עובד"
            };

            string path = null;

            if (userPic != null && userPic.ContentLength > 0)
            {
                path = "Images/ProfileImages/" + id + Path.GetExtension(userPic.FileName);
                string mapPath = Server.MapPath("~/" + path);
                userPic.SaveAs(mapPath);
            }

            WorkerDC worker = new WorkerDC()
            {
                IdNumber = id,
                FirstName = fname,
                LastName = lname,
                Phone = phone,
                Email = email,
                Picture = path
            };

            try
            {
                int res = clnt.UpdateWorker(worker);

                if (res == 0)
                {
                    message.Content = "פרטי העובד עודכנו בהצלחה";
                }
                else
                {
                    message.Content = "כבר קיימת במערכת עובד בשם " + fname +  ' ' + lname;
                }
            }
            catch (Exception)
            {
                userPic = null;
                message.Content = "אירעה שגיאה במהלך עדכון פרטי העובד, אנא נסה שוב";
            }


            var html = this.RenderPartialToString("Partials/_ModalMessage", message);

            return Json(new { html = html });
        }

        [HttpPost]
        [AdminOnlyFilter]
        public JsonResult DeleteWorker(string workerId)
        {
            Message message = new Message()
            {
                Id = "delete-worker-modal",
                Title = "מחיקת עובד"
            };

            try
            {
                clnt.DeleteWorker(workerId);
                message.Content = "העובד נמחק בהצלחה";
            }
            catch (Exception)
            {
                message.Content = "אירעה שגיאה במהלך מחיקת העובד, אנא נסה שוב";
            }

            var html = this.RenderPartialToString("Partials/_ModalMessage", message);

            return Json(new { html = html });
        }
    }
}