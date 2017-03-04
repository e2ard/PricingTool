using getLayout.Models;
using PricingTool.MVC.Controllers.App_Code;
using PricingTool.MVC.Models;
using PricingTool.MVC.Models.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace getLayout.Controllers
{
    public class PricingController : Controller
    {
        // GET: Pricing
        public ActionResult Index()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        public IEnumerable<SelectListItem> GetLocations()
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
            sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
            sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
            sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
            sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
            sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
            //sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });


            return new SelectList(sl, "Value", "Text");
        }
        public string GeneratePrices(SearchFilters searchFilters)
        {
            string linkNr = string.Empty;
            if (searchFilters.Location == 1)
                linkNr = "291345";

            if(searchFilters.Location == 12)
                linkNr = "290494";

            if (string.IsNullOrEmpty(linkNr))
                throw new Exception();

            PricingHelper pr = new PricingHelper(searchFilters, new string[] { "MiniM"/*, "EconomyM", "EconomyA", "CompactM", "CompactA"*/}, new string[] { linkNr/*, "283487", "283488", "283489",  "283490"*/});
            return "ok";
        }
    }
}