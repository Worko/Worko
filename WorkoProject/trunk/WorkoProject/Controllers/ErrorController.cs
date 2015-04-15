using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkoProject.Models;

namespace WorkoProject.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AdminOnly()
        {
            Message message = new Message()
            {
                Id = "admin-only",
                Title = "שגיאת אבטחה",
                Content = "אינך מורשה להכנס לדף אליו ניסית להגיע"
            };

            return View("ErrorPage", message);
        }
    }
}