using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCore.Models;

namespace DotNetCore.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new PageModel();
            model.Content = "<h1>Test</h1>Content.";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PageModel postedValues)
        {
            var html = postedValues.Content;

            return View(postedValues);
        }
    }
}
