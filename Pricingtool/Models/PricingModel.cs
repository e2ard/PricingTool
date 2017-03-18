using PricingTool.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PricingTool.MVC.Models
{
    public class PricingModel : SearchFilters
    {
        public List<string> Classes { get; set;  }
        public List<SelectListItem> AvailableClasses { get; set; }
    }
}