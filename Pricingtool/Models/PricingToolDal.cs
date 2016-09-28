using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PricingTool.MVC.Models.Dal
{
    public class PricingToolDal
    {
        public static SelectList GetSources()
        {
            return new SelectList(new List<SelectListItem>{
                  new SelectListItem{ Selected = false,Text = "RentalCars", Value = "1"},
                  new SelectListItem {Selected = false, Text = "CarsTrawler", Value = "2"},
                  new SelectListItem {Selected = false, Text = "CarScanner", Value = "3"},
                  //new SelectListItem {Selected = false, Text = "AtlassChoise", Value = "4"},
            }, "Value", "Text");
        }

        public static SelectList GetCoutries()
        {
            return new SelectList(new List<SelectListItem>{
                  new SelectListItem{ Selected = false,Text = "Latvia", Value = "1"},
                  new SelectListItem {Selected = false, Text = "Lithuania", Value = "2"},
                  new SelectListItem{ Selected = false,Text = "Poland", Value = "3"},
            }, "Value", "Text");
        }

        public static SelectList GetLocations()
        {
            return new SelectList(new List<SelectListItem>(), "Value", "Text");
        }

        public static JsonResult GetLocations(int country)
        {
            List<SelectListItem> sl = new List<SelectListItem>();
            switch (country)
            {
                case 1:
                    sl.Add(new SelectListItem { Selected = true, Text = "Riga", Value = "3" });
                    break;
                case 2:
                    sl.Add(new SelectListItem { Selected = false, Text = "Vilnius", Value = "1" });
                    sl.Add(new SelectListItem { Selected = false, Text = "Kaunas", Value = "2" });
                    break;
                case 3:
                    sl.Add(new SelectListItem { Selected = false, Text = "Warsaw", Value = "4" });
                    break;
                default:
                    break;
            }
            return new JsonResult()
            {
                Data = new SelectList(sl, "Value", "Text"),
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
    }
}