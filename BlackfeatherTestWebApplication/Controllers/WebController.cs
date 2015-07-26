using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace BlackfeatherTestWebApplication.Controllers
{
    public class WebController : Controller
    {
        //
        // GET: /Web/
        public ActionResult Index()
        {
            Session.Add("TestKey", "TestValue");

            return View();
        }
	}
}