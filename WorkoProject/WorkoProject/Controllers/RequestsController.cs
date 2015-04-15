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
    [VerifyUserFilter]
    public class RequestsController : Controller
    {
        private DBService.DB clnt = new DBService.DB();
        
        public ActionResult SendRequest()
        {
            return View(new Request());
        }

        [HttpPost]
        public ActionResult SendRequest(Request request)
        {
            request.Date = DateTime.Now;

            var req = request.TryCast<RequestDC>();

            if(clnt.AddWorkerRequest(req) == 1)
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

            return View(new Request());
        }
    }
}