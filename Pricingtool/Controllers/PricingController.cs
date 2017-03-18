using getLayout.Models;
using PricingTool.MVC.Controllers.App_Code;
using PricingTool.MVC.Models;
using PricingTool.MVC.Models.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            ViewBag.Sources = new PricingToolDal().GetSources();
            PricingModel pm = new PricingModel();
            pm.AvailableClasses = GetClasses();
            return View(pm);
        }

        [HttpPost]
        public string Index(PricingModel searchFilters)
        {
            string miniM = string.Empty, ecoM = string.Empty, ecoA = string.Empty, compM = string.Empty, compA = string.Empty;
            //if (searchFilters.Location == 1)
            //{
            //    miniM = "291345";
            //    ecoM = "292822";
            //    ecoA = "292846";
            //    compM = "292855";
            //    compA = "292870";
            //}

            //if(searchFilters.Location == 12)
            //{
            //    miniM = "290494";
            //    ecoM = "292827";
            //    ecoA = "292851";
            //    compM = "292862";
            //    compA = "292866";
            //}

            if (searchFilters.Location == 9)
            {
                miniM = "295233";
                ecoM = "295235";
                ecoA = "295234";
                //compM = "292862";
                //compA = "292866";

                switch (searchFilters.IntervalNum)
                {
                    case 1:
                        miniM = "295233";
                        ecoM = "295235";
                        ecoA = "295234";
                        break;
                    case 2:
                        miniM = "296332";
                        ecoM = "296334";
                        ecoA = "296336";
                        break;
                    case 3:
                        miniM = "296333";
                        ecoM = "296335";
                        ecoA = "296337";
                        break;
                }
            }

            if (string.IsNullOrEmpty(miniM)/* || string.IsNullOrEmpty(ecoM)*/)
                throw new Exception();

            PricingHelper pr = new PricingHelper(searchFilters, new string[] { "MiniM", "EconomyM", "EconomyA"/*, "CompactM", "CompactA"*/}, new string[] { miniM, ecoM, ecoA/*, compM, compA*/});


            return pr.Excecute();
        }

        public IEnumerable<SelectListItem> GetLocations()
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            //sl.Add(new SelectListItem { Selected = false, Text = "Gdansk", Value = "12" });
            //sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Krakow", Value = "11" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Chopin)", Value = "4" });
            sl.Add(new SelectListItem { Selected = false, Text = "Warsaw (Modlin)", Value = "9" });
            //sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Fiumicino", Value = "6" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Rome", Value = "7" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Bologna", Value = "8" });
            //sl.Add(new SelectListItem { Selected = false, Text = "Prague", Value = "10" });


            return new SelectList(sl, "Value", "Text");
        }
        public string GeneratePrices(PricingModel searchFilters)
        {
            string miniM = string.Empty, ecoM = string.Empty, ecoA = string.Empty, compM = string.Empty, compA = string.Empty;
            //if (searchFilters.Location == 1)
            //{
            //    miniM = "291345";
            //    ecoM = "292822";
            //    ecoA = "292846";
            //    compM = "292855";
            //    compA = "292870";
            //}

            //if(searchFilters.Location == 12)
            //{
            //    miniM = "290494";
            //    ecoM = "292827";
            //    ecoA = "292851";
            //    compM = "292862";
            //    compA = "292866";
            //}

            if (searchFilters.Location == 9)
            {
                miniM = "295233";
                ecoM = "295235";
                ecoA = "295234";
                //compM = "292862";
                //compA = "292866";

                switch(searchFilters.IntervalNum)
                {
                    case 1:
                        miniM = "295233";
                        ecoM = "295235";
                        ecoA = "295234";
                        break;
                    case 2:
                        miniM = "296332";
                        ecoM = "296334";
                        ecoA = "296336";
                        break;
                    case 3:
                        miniM = "296333";
                        ecoM = "296335";
                        ecoA = "296337";
                        break;
                }
            }

            if (string.IsNullOrEmpty(miniM)/* || string.IsNullOrEmpty(ecoM)*/)
                throw new Exception();
            
            PricingHelper pr = new PricingHelper(searchFilters, new string[] { "MiniM", "EconomyM", "EconomyA"/*, "CompactM", "CompactA"*/}, new string[] { miniM, ecoM, ecoA/*, compM, compA*/});

            
            return pr.Excecute();
        }

        private List<SelectListItem> GetClasses()
        {
            return new List<SelectListItem>
        {
            new SelectListItem {Text = "Mini", Value = "MiniM"},
            new SelectListItem {Text = "ECMR", Value = "EcoM"},
            new SelectListItem {Text = "ECAR", Value = "EcoA"},
        };
        }
    }
}