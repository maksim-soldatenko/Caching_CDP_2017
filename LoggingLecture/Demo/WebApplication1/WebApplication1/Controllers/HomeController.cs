using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Logging;
using log4net.Config;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog _log= LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            _log.Debug("Index called");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";


            _log.Debug("About called");

            return View();
        }

        public ActionResult Contact()
        {

            ViewBag.Message = "Your contact page.";

            FormatMessageHandler format =
                (s, args) => $"Formatted exception: Message:{s}. Details: {args[0]}, some words: {args[1]}, UserId: {args[2]}";

            _log.Fatal(format("Fatal error", "args0", "args1", "User Vasia"));

            return View();
        }
    }
}