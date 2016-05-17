using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CkeditorSampleSite.Models
{
    public class PageModel
    {
        [AllowHtml]
        public string HtmlText { get; set; }
    }
}