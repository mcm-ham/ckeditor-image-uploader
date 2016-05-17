using CkeditorSampleSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CkeditorSampleSite.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new PageModel();
            model.HtmlText = "<h1>Test</h1>Content.";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PageModel postedValues)
        {
            var html = postedValues.HtmlText;

            return View(postedValues);
        }
    }
}