using PricingTool.MVC.Controllers.App_Code;
using PricingTool.MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace getLayout.Models
{
    class PricingHelper
    {
        string RequestBody;
        List<JOffer> Offers;
        PricingModel Sf;
        //private Dictionary<string, Dictionary<string, JOffer>> rates;

        string[] Categories;
        string[] Links;
        public PricingHelper(PricingModel Sf, string[] categories, string[] links)
        {
            this.Sf = Sf;
            Categories = categories;
            Links = links;

            if (Categories.Count() != Links.Count())
                throw new Exception("Not equal categories ant links");
        }

        public string Excecute()
        {
            Dictionary<string, Dictionary<string, JOffer>> rates = null;
            string brokerName = "";
            string brokerTemp = "";
            SiteBase site = null;
            Controllers.HomeController hm = new Controllers.HomeController();
            switch (Sf.Source)
            {
                case 1:
                    rates = hm.GetRentalPdf(Sf, out site);
                    brokerName = "JIG";
                    break;
                case 2:
                    rates = hm.GetCarTrawlerPdf(Sf, out site);
                    brokerName = "CTR";
                    break;
                case 3:
                    rates =hm.GetScannerPdf(Sf, out site);
                    brokerName = "CTR";
                    break;
            }

            if (brokerName.Equals("JIG"))
                brokerTemp = "CTR";

            if (brokerName.Equals("CTR"))
                brokerTemp = "JIG";

            for (int i = 0; i < Categories.Count(); i++)
            {
                List<JOffer> categoryOffers = GetMiniRatesList(rates, Categories[i]);
                
                CsvHelper csvHelper = new CsvHelper(Links[i]);
                List<float> fuseRates = csvHelper.GetFuseRates(brokerName);

                List<float> priceList = CalculatePrices(categoryOffers, fuseRates, brokerName);
                
                string body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerName, priceList);
                
                fuseRates = csvHelper.GetFuseRates(brokerTemp);
                body = csvHelper.GenerateRateString(Sf.PuDate, Sf.DoDate, brokerTemp, fuseRates, body);

                HttpWebResponse response;
                string responseText = string.Empty;

                if (Request_gmlithuania_fusemetrix_com(Links[i], body, Categories[i], out response))
                {
                    responseText = ReadResponse(response);

                    response.Close();
                }
            }
            return hm.CreatePdf(site,rates);
        }

        private List<float> CalculatePrices(List<JOffer> categoryOffers, List<float> fuseRates, string brokerName)
        {
            if (fuseRates.Count() == 0)
                return null;
            List<float> priceList = new List<float>();
            for (int i = 0; i < categoryOffers.Count(); i++)
            {
                if(brokerName.Equals("JIG"))
                    priceList.Add(CalculatePriceJIG(categoryOffers.ElementAt(i).gmPrice, categoryOffers.ElementAt(i).price, fuseRates.ElementAt(i)));
                if (brokerName.Equals("CTR"))
                    priceList.Add(CalculatePriceCTR(categoryOffers.ElementAt(i).gmPrice, categoryOffers.ElementAt(i).price, fuseRates.ElementAt(i)));
            }
            return priceList;
        }

        //public void MakeRequests(string linkId)
        //{
        //    HttpWebResponse response;
        //    string responseText;

        //    if (Request_gmlithuania_fusemetrix_com(linkId, out response))
        //    {
        //        responseText = ReadResponse(response);

        //        response.Close();
        //    }
        //}

        private static string ReadResponse(HttpWebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                Stream streamToRead = responseStream;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                }

                using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public List<JOffer> GetMiniRatesList(Dictionary<string, Dictionary<string, JOffer>> rates, string categoryName)
        {
            List<JOffer> miniOffers = new List<JOffer>();
            foreach (string link in rates.Keys.ToList())
            {
                Dictionary<string, JOffer> map = rates[link];
                if (map.Count > 0)
                {
                    Category item = Const.categories.FirstOrDefault(f => f.Name.Equals(categoryName));
                    {
                        if ((map.ContainsKey(item.Name)) && (map[item.Name] != null))
                            miniOffers.Add(map[item.Name]);
                        else
                            miniOffers.Add(new JOffer());
                    }
                }
            }
            return miniOffers;
        }

        private bool Request_gmlithuania_fusemetrix_com(string linkId, string body, string category, out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gmlithuania.fusemetrix.com/bespoke/rate_manager/old_price_override/overrideprice_edit.php?id=" + linkId);

                request.KeepAlive = true;
                request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
                request.Headers.Add("Origin", @"http://gmlithuania.fusemetrix.com");
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Referer = "http://gmlithuania.fusemetrix.com/bespoke/rate_manager/old_price_override/overrideprice_edit.php?id=295233";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
                request.Headers.Set(HttpRequestHeader.Cookie, @"PHPSESSID=96kd0kooju89aglvs25l646g32; mwoid=24; mwornd=327915990");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return false;
            }

            return true;
        }
        private float CalculatePriceCTR(float gmPrice, float price, float fusePrice)
        {
            float overridePrice = price;
            float priceDiff = gmPrice - price;
            
            overridePrice = price - 5 * price/95;

            if (gmPrice < fusePrice)
                System.Diagnostics.Debug.WriteLine("wait some more, not updated");

            if ((price == 0 || gmPrice == 0) || (price > gmPrice) && (price - gmPrice < 1.5f) && (price - gmPrice > 0))
                return fusePrice;

            if (overridePrice < price * 0.5f || fusePrice - priceDiff * 1.5f < 10 || fusePrice <= 0)
                return price - Math.Abs(priceDiff) * 0.2f;

            return overridePrice;
        }

        private float CalculatePriceJIG(float gmPrice, float price, float fusePrice)
        {
            float overridePrice = price;
            float priceDiff = gmPrice - price;
            
            if (priceDiff >= 0)
            {
                if (priceDiff < 5)
                {
                    if (priceDiff < 3)
                    {
                        if (priceDiff < 2)
                        {
                            if (priceDiff < 1)
                            {
                                if (priceDiff == 0)
                                    overridePrice = fusePrice - 0.01f;//if price are equal
                                else
                                    overridePrice = fusePrice - priceDiff * 0.97f;
                            }
                            else
                                overridePrice = fusePrice - priceDiff * 0.94f;
                        }
                        else
                            overridePrice = fusePrice - priceDiff * 0.71f;
                    }
                    else
                        overridePrice = fusePrice - priceDiff * 0.55f;
                }
                else
                    overridePrice = fusePrice - priceDiff * 0.51f;
            }
            else
            {
                if (Math.Abs(priceDiff) < 5)
                {
                    if (Math.Abs(priceDiff) < 3)
                        if (priceDiff > 1.5f)
                            overridePrice = fusePrice + Math.Abs(priceDiff)* 0.55f;
                        else
                            overridePrice = fusePrice + Math.Abs(priceDiff) * 0.3f;
                    else
                        overridePrice = fusePrice + Math.Abs(priceDiff) * 0.45f;
                    //overridePrice = fusePrice - priceDiff * gmPrice / fusePrice * 0.5f;
                }
                else
                    overridePrice = fusePrice + Math.Abs(priceDiff) * 0.3f;
            }

            if (price == 0 || gmPrice == 0)
                return 0;



            if (price > gmPrice && price - gmPrice < 1.5f && price - gmPrice > 0)
                return fusePrice;

            if (overridePrice < price * 0.5f || fusePrice - priceDiff * 1.5f < 10 || fusePrice <= 0)
                return price - price * 0.28f;

            return overridePrice;
        }

    }
}
