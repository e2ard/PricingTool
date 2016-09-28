using PricingTool.MVC.Controllers.App_Code;
using PricingTool.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace getLayout.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            //if (User.Identity.IsAuthenticated)
            return RedirectToAction("SearchFilterPartial");
            //else
            //    return RedirectToAction("Login", "Account");
        }

        public ActionResult SearchFilterPartial()
        {
            SearchFilters sf = new SearchFilters();
            sf.Location = 3;
            return View(sf);
        }

        [HttpPost]
        public ActionResult SearchFilterPartial(SearchFilters searchFilters)
        {
            string fileName = "/pdf/rentalcars6-6Kaunas.pdf";
            try
            {
                if (ModelState.IsValid)
                {
                    switch (searchFilters.Source)
                    {
                        case 1:
                            fileName = GetRentalPdf(searchFilters);
                            break;
                        case 2:
                            fileName = GetCarTrawlerPdf(searchFilters);
                            break;
                        case 3:
                            fileName = GetScannerPdf(searchFilters);
                            break;
                        case 4:
                            fileName = GetAtlassPdfSync(searchFilters);
                            break;
                    }
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/" + fileName));
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName.Substring(5, fileName.Length - 5));
                }
            }
            catch (Exception exp)
            {
                //Debug.WriteLine(exp.InnerException);
                //Response.Write("<script>alert('Proxy Error');</script>");
            }
            return View(searchFilters);
        }

        public string GetRentalPdf(SearchFilters searchFilters)
        {
            DateTime sDate = (DateTime)searchFilters.PuDate;
            DateTime eDate = searchFilters.DoDate;

            Rental s = new Rental(Const.Locations[searchFilters.Location].Rental);

            s.SetTime(searchFilters.PuTime.Hours, searchFilters.PuTime.Minutes, searchFilters.DoTime.Hours, searchFilters.DoTime.Minutes);
            s.InitDate(sDate);
            using (PdfBuilder pdf = new PdfBuilder(s))
            {
                pdf.CreateHeaders();

                int numOfIterations = (eDate - sDate).Days;

                List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
                List<JOffer> minOffers = new List<JOffer>();

                Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();
                for (int i = 0; i < links.Count; i++)
                    offerMap.Add(links[i], new Dictionary<string, JOffer>());

                List<Thread> threads = new List<Thread>();
                //--- Start all threads
                for (int index = 0; index < links.Count; index++)
                {
                    Thread thread = new Thread(() =>
                    {
                        JSourceReader reader = new JSourceReader();
                        offerMap[Thread.CurrentThread.Name == null ? links.ElementAt(0) : Thread.CurrentThread.Name] =
                        reader.GetMap(reader.ExtractOffers(reader.GetResultGroup(Thread.CurrentThread.Name)));
                    });
                    thread.Name = links.ElementAt(index);
                    threads.Add(thread);
                    thread.Start();
                }

                //check if thread has done
                Boolean allCompleted = false;
                while (!allCompleted)
                {
                    int completed = links.Count;
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (!threads.ElementAt(i).IsAlive)
                            --completed;
                        else
                        {
                            Thread.Sleep(300);
                            break;
                        }
                    }
                    if (completed == 0)
                        break;
                }
                foreach (string link in links)
                {
                    Dictionary<string, JOffer> map = offerMap[link];
                    List<JOffer> offers = new List<JOffer>();
                    if (map.Count > 0)
                    {
                        foreach (Category item in Const.categories)
                        {
                            if (map.ContainsKey(item.Name))
                            {
                                if (map[item.Name] != null)
                                {
                                    map[item.Name].SetSiteName(link);
                                    offers.Add(map[item.Name]);
                                }
                            }
                            else
                                offers.Add(new JOffer());
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Map count 0");
                    }
                    pdf.addRow(offers.ToArray());
                }
                return pdf.fileName;
            }
        }

        public string GetCarTrawlerPdf(SearchFilters searchFilters)
        {
            DateTime sDate = searchFilters.PuDate;
            DateTime eDate = searchFilters.DoDate;

            Trawler s = new Trawler(Const.Locations[searchFilters.Location].CarTrawler);
            
            s.InitDate(sDate);
            PdfBuilder pdf = new PdfBuilder(s);
            pdf.CreateHeaders();

            int numOfIterations = (eDate - sDate).Days;

            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            List<JOffer> minOffers = new List<JOffer>();

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());


            List<Thread> threads = new List<Thread>();
            //--- Start all threads
            for (int index = 0; index < links.Count; index++)
            {
                Thread thread = new Thread(() =>
                {
                    JSourceReader reader = new JSourceReader();
                    offerMap[Thread.CurrentThread.Name == null ?
                        links.ElementAt(0) :
                        Thread.CurrentThread.Name] =
                            reader.GetMapNorwegian(reader.GetNorwRates(Thread.CurrentThread.Name));
                });
                thread.Name = links.ElementAt(index);
                threads.Add(thread);
                thread.Start();
            }

            //check if threads has done
            Boolean allCompleted = false;
            while (!allCompleted)
            {
                int completed = links.Count;
                for (int i = 0; i < links.Count; i++)
                {
                    if (!threads.ElementAt(i).IsAlive)
                        --completed;
                    else
                    {
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (completed == 0)
                    break;
            }

            foreach (string link in links)
            {
                Dictionary<string, JOffer> map = offerMap[link];
                List<JOffer> offers = new List<JOffer>();
                if (map.Count > 0)
                {
                    foreach (Category item in Const.categories)
                    {
                        if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                        {
                            map[item.Name].SetSiteName(link);
                            offers.Add(map[item.Name]);
                        }
                    }
                }
                //else
                //    System.Diagnostics.Debug.WriteLine("Map count 0");
                pdf.addRow(offers.ToArray());
            }
            pdf.Close();
            return pdf.fileName;
        }

        public string GetScannerPdf(SearchFilters searchFilters)
        {
            DateTime sDate = searchFilters.PuDate;
            DateTime eDate = searchFilters.DoDate;
            Scanner s = new Scanner(Const.Locations[searchFilters.Location].CarScanner);

            s.InitDate(sDate);
            PdfBuilder pdf = new PdfBuilder(s);
            pdf.CreateHeaders();

            int numOfIterations = (eDate - sDate).Days;

            List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
            List<JOffer> minOffers = new List<JOffer>();

            Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

            for (int i = 0; i < links.Count; i++)
                offerMap.Add(links[i], new Dictionary<string, JOffer>());


            List<Thread> threads = new List<Thread>();
            //--- Start all threads
            for (int index = 0; index < links.Count; index++)
            {
                Thread thread = new Thread(() =>
                {
                    JSourceReader reader = new JSourceReader();
                    offerMap[Thread.CurrentThread.Name == null ?
                        links.ElementAt(0) :
                        Thread.CurrentThread.Name] =
                            reader.GetMapNorwegian(reader.GetScannerRates(Thread.CurrentThread.Name));
                });
                thread.Name = links.ElementAt(index);
                threads.Add(thread);
                thread.Start();
            }

            //check if threads has done
            Boolean allCompleted = false;
            while (!allCompleted)
            {
                int completed = links.Count;
                for (int i = 0; i < links.Count; i++)
                {
                    if (!threads.ElementAt(i).IsAlive)
                        --completed;
                    else
                    {
                        Thread.Sleep(100);
                        break;
                    }
                }
                if (completed == 0)
                    break;
            }

            foreach (string link in links)
            {
                Dictionary<string, JOffer> map = offerMap[link];
                List<JOffer> offers = new List<JOffer>();
                if (map.Count > 0)
                {
                    foreach (Category item in Const.categories)
                    {
                        if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                        {
                            map[item.Name].SetSiteName(link);
                            offers.Add(map[item.Name]);
                        }
                    }
                }
                //else
                //    System.Diagnostics.Debug.WriteLine("Map count 0");
                pdf.addRow(offers.ToArray());
            }
            pdf.Close();
            return pdf.fileName;
        }

        public string GetAtlassPdfSync(SearchFilters searchFilters)
        {
            DateTime sDate = searchFilters.PuDate;
            DateTime eDate = searchFilters.DoDate;
            Atlass s = new Atlass(Const.Locations[searchFilters.Location].AtlasChoise);
            sDate = sDate.AddDays(1);
            eDate = eDate.AddDays(1);
            s.InitDate(sDate);
            string pdfFileName = string.Empty;
            PdfBuilder pdf = new PdfBuilder(s);
            {
                pdf.CreateHeaders();

                int numOfIterations = (eDate - sDate).Days;

                List<string> links = s.GetGeneratedLinksByDate(sDate, eDate);
                List<JOffer> minOffers = new List<JOffer>();

                Dictionary<string, Dictionary<string, JOffer>> offerMap = new Dictionary<string, Dictionary<string, JOffer>>();

                for (int i = 0; i < links.Count; i++)
                    offerMap.Add(links[i], new Dictionary<string, JOffer>());


                List<Thread> threads = new List<Thread>();
                for (int i = 0; i < links.Count; i++)
                {
                    JSourceReader reader = new JSourceReader();
                    List<JOffer> offers = reader.GetAtlassOffers(
                                    reader.RemoveSpecialCharacters(
                                        reader.GetAtlassSource(links.ElementAt(i))));

                    offerMap[links.ElementAt(i)] =
                            reader.GetMapNorwegian(offers);
                    if (i < links.Count - 1)
                        Thread.Sleep(8000);
                }

                foreach (string link in links)
                {
                    Dictionary<string, JOffer> map = offerMap[link];
                    List<JOffer> offers = new List<JOffer>();
                    if (map.Count > 0)
                    {
                        foreach (Category item in Const.categories)
                        {
                            if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                            {
                                map[item.Name].SetSiteName(link);
                                offers.Add(map[item.Name]);
                            }
                        }
                    }
                    //else
                    //    System.Diagnostics.Debug.WriteLine("Map count 0");
                    pdf.addRow(offers.ToArray());
                }
                pdf.Close();
                return pdf.fileName;
            }
        }

        protected void OpenPdf(string fileName)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=rentalcars6-6Kaunas.pdf");
            Response.TransmitFile(Server.MapPath("~/pdf/rentalcars6-6Kaunas.pdf"));
        }

        [HttpPost]
        public ActionResult OpenPdf()
        {
            string fileName = @"\\pdf\\rentalcars6-6Kaunas.pdf";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/" + fileName));

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName.Substring(5, fileName.Length - 5));
        }

        public JsonResult GetLocations(int? country)
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
                case 4:
                    sl.Add(new SelectListItem { Selected = false, Text = "London", Value = "5" });
                    break;
                default:
                    break;
            }
            IEnumerable<SelectListItem> data = new SelectList(sl, "Value", "Text");
            JsonResult jsonResult = new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return jsonResult;
        }
    

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}