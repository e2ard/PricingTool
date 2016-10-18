using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PricingTool.MVC.Controllers.App_Code
{

    /// <summary>
    /// Summary description for SurceReader
    /// </summary>
    public class JSourceReader
    {
        public static string ipStr = "lt-lv1.nordvpn.com";//"193.105.240.1";
        public static int port = 80;//8080
        public static bool addProxy = true;//if true then add
        public static string user = "edvard.naus@gmail.com";
        public static string pass = "421c3421c3";
        
        public JSourceReader(){}
        
        public JOffer GetMinOffer(List<JOffer> offers)
        {
            float minPrice = 999999;
            String minSupplier = null;
            float minGMPrice = 9999999;
            String minGMSupplier = null;
            foreach (JOffer of in offers)
            {
                String supplier = of.GetSupplier().ToLower();
                if (!(supplier.Equals("green motion"))
                     && (of.GetPrice() < minPrice))
                {

                    minPrice = of.GetPrice();
                    minSupplier = of.GetSupplier();
                }
                else if (supplier.Equals("green motion")
                    && (of.GetPrice() < minGMPrice))
                {
                    minGMPrice = of.GetPrice();
                    minGMSupplier = "GM";
                }

            }
            JOffer offer = new JOffer();
            offer.SetPrice(minPrice);
            offer.SetSupplier(minSupplier);
            offer.SetGMPrice(minGMPrice);
            offer.SetGM(minGMSupplier);
            offer.SetSiteName(offer.GetSiteName());
            return offer;
        }

        public HtmlNodeCollection GetResultGroup(string site)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string source = client.DownloadString(site);
                    //retrieve page source tags
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(source);

                    string[] carOfferStrgs = { "car-result group ", "search-result txt-grey-7 ", "carResultDiv " };
                    HtmlNodeCollection offersFound = null;
                    for(int i = 0; i < carOfferStrgs.Count() && offersFound == null; i++)
                    {
                        offersFound = doc.DocumentNode.SelectNodes(".//div[contains(@class,'" + carOfferStrgs[i] + "')]");
                    }
                    return offersFound;
                }
            }
            catch
            {
                Debug.WriteLine("Offers not found");
                return null;
            }
        }

        public List<JOffer> ExtractOffers(HtmlNodeCollection resultGroups)
        {
            List<JOffer> offers = new List<JOffer>();
            if (resultGroups != null)
            {
                foreach (HtmlNode mainNode in resultGroups)
                {
                    //price ------------------------------------------------
                    string[] priceStrgs = { ".//p[@class='now ']", ".//p[@class='now']", ".//span[@class='carResultRow_Price-now']" };
                    HtmlNode priceNode = null;
                    string price = string.Empty;
                    for (int i = 0; i < priceStrgs.Count() && priceNode == null; i++)
                    {
                        priceNode = mainNode.SelectSingleNode(priceStrgs[i]);
                        if (priceNode != null)
                            price = priceNode.InnerText;
                    }
                    if (priceNode == null)
                        Debug.WriteLine("--------------->Price not pursed----------------");
                    //supplier ------------------------------------------------
                    string supplier = string.Empty;
                    string[] supplierStrgs = { ".//div[@class='supplier_id']/img", ".//div[@class='col dbl info-box supplier']/img", ".//div[@class='carResultRow_OfferInfo_Supplier-wrap']/img" };
                    HtmlNode supplierNode = null;
                    for (int i = 0; i < supplierStrgs.Count() && supplierNode == null; i++)
                    {
                        supplierNode = mainNode.SelectSingleNode(supplierStrgs[i]);
                        if (supplierNode != null)
                            supplier = supplierNode.Attributes["title"].Value;
                    }
                    if (supplierNode == null)
                        Debug.WriteLine("Suppliernot pursed -------------------------------");
                    //category ------------------------------------------------
                    string category = string.Empty;
                    string[] categoryStrs = { ".//p[contains(@class,'bg-yellow-5')]", ".//span[contains(@class,'class mini')]", ".//span[contains(@class,'carResultRow_CarSpec_CarCategory')]"};
                    HtmlNode categoryNode = null;
                    for(int i = 0; i <= categoryStrs.Count() && categoryNode == null; i++)
                    {
                        categoryNode = mainNode.SelectSingleNode(categoryStrs[i]);
                        if (categoryNode != null)
                            category = categoryNode.InnerText;
                    }
                    if (categoryNode == null)
                        Debug.WriteLine("-------------> category not pursed");
                    //transmission ------------------------------------------------
                    string transm = string.Empty;
                    string[] transmStrgs = { ".//li[contains(@class,'result_trans')]", ".//span[contains(@class,'class mini')]", ".//ul[contains(@class, 'carResultRow_CarSpec-tick')]/li[last()]" };
                    HtmlNode transmNode = null;
                    for(int i = 0; i < transmStrgs.Count() && transmNode == null; i++)
                    {
                        transmNode = mainNode.SelectSingleNode(transmStrgs[i]);
                        if (transmNode != null)
                            transm = transmNode.InnerText;
                    }
                    if (transmNode == null)
                        Debug.WriteLine("-------------> transm not pursed");
                    //seats
                    string seats = string.Empty;
                    string[] seatsStrgs = { ".//li[contains(@class,'result_seats')]", ".//span[contains(@class,'class mini')]", ".//li[contains(@class,'carResultRow_CarSpec_Seats')]" };
                    HtmlNode seatsNode = null;
                    for(int i = 0; i < seatsStrgs.Count() && seatsNode == null; i++)
                    {
                        seatsNode = mainNode.SelectSingleNode(seatsStrgs[i]);
                        if (seatsNode != null)
                            seats = seatsNode.InnerText;
                    }
                    if (seatsNode == null)
                        Debug.WriteLine("-------------> seats not pursed");

                    JOffer o = setOfferValues(supplier, price, category, transm, seats);
                    offers.Add(o);
                }
            }
            else
                Debug.WriteLine("JSOURCEREDAER 0 offer number ");

            return offers;
        }

        public JOffer setOfferValues(string supplier, string price, string category, string transm, string seats)
        {
            JOffer o = new JOffer(supplier, price);

            o.category = category;
            o.transmission = transm.Trim().Split(' ')[0].Substring(0, 1);
            o.seats = seats.Trim().Substring(0, 1);
            return o;
        }

        public Dictionary<string, JOffer> GetMap(List<JOffer> offers)
        {
            Dictionary<string, JOffer> dayOffers = new Dictionary<string, JOffer>();
            foreach (JOffer o in offers)
            {
                string offerKey = o.category + o.transmission;
                if (offerKey.Equals("People CarrierM") && !o.seats.Equals("9"))
                {
                    //Debug.WriteLine("-----> dayOffers " + o.GetOffer() + " " + o.seats.Equals("9"));
                    continue;
                }

                if (dayOffers.ContainsKey(offerKey))
                {
                    if (o.GetPrice() < dayOffers[offerKey].GetPrice() && o.GetPrice() != 0 || dayOffers[offerKey].GetPrice() == 0)
                    {
                        dayOffers[offerKey].SetPrice(o.GetPrice());
                        dayOffers[offerKey].SetSupplier(o.GetSupplier());
                    }
                    if (o.GetGMPrice() < dayOffers[offerKey].GetGMPrice() && o.GetGMPrice() != 0 || dayOffers[offerKey].GetGMPrice() == 0)
                    {
                        dayOffers[offerKey].SetGMPrice(o.GetGMPrice());
                        dayOffers[offerKey].SetGM(o.GetGM());
                    }
                    if (o.GetCRPrice() < dayOffers[offerKey].GetCRPrice() && o.GetCRPrice() != 0 || dayOffers[offerKey].GetCRPrice() == 0)
                    {
                        dayOffers[offerKey].SetCRPrice(o.GetCRPrice());
                        dayOffers[offerKey].SetCR(o.GetCR());
                    }
                }
                else
                    dayOffers[offerKey] = o; // if not initialized add new offer
            }
            return dayOffers;
        }

        public Dictionary<string, JOffer> GetMapNorwegian(List<JOffer> offers)
        {
            Dictionary<string, JOffer> dayOffers = new Dictionary<string, JOffer>();
            foreach (JOffer o in offers)
            {
                string offerKey = o.category;
                if (o.category.Equals("skip"))
                {
                    //Debug.WriteLine("-----> dayOffers " + o.GetOffer() + " " + o.seats.Equals("9"));
                    continue;
                }

                if (dayOffers.ContainsKey(o.category))
                {
                    if (o.GetPrice() < dayOffers[offerKey].GetPrice() && o.GetPrice() != 0 || dayOffers[offerKey].GetPrice() == 0)
                    {
                        dayOffers[offerKey].SetPrice(o.GetPrice());
                        dayOffers[offerKey].SetSupplier(o.GetSupplier());
                    }
                    if (o.GetGMPrice() < dayOffers[offerKey].GetGMPrice() && o.GetGMPrice() != 0 || dayOffers[offerKey].GetGMPrice() == 0)
                    {
                        dayOffers[offerKey].SetGMPrice(o.GetGMPrice());
                        dayOffers[offerKey].SetGM(o.GetGM());
                    }
                    if (o.GetCRPrice() < dayOffers[offerKey].GetCRPrice() && o.GetCRPrice() != 0 || dayOffers[offerKey].GetCRPrice() == 0)
                    {
                        dayOffers[offerKey].SetCRPrice(o.GetCRPrice());
                        dayOffers[offerKey].SetCR(o.GetCR());
                    }
                }
                else
                    dayOffers[offerKey] = o; // if not initialized add new offer
            }
            return dayOffers;
        }

        public List<JOffer> GetNorwRates(string url)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);

            // If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            if (addProxy)
            {
                WebProxy proxy = new WebProxy(ipStr, port);
                proxy.Credentials = new NetworkCredential(user, pass);
                request.Proxy = proxy;
            }
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.

            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3;

            string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

            JToken entireJson = JToken.Parse(subJson);

            JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers
            Console.WriteLine(vehVendorAvails.Count);

            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                foreach (var vehicle in item["VehAvails"])
                {
                    string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                    string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                    JOffer offer = new JOffer(supplier, price);
                    offer.SetCategory(category);
                    offers.Add(offer);
                }
            }

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return offers;
        }

        public List<JOffer> GetScannerRates(string url)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            if (addProxy)
            {
                WebProxy proxy = new WebProxy(ipStr, port);
                proxy.Credentials = new NetworkCredential(user, pass);
                request.Proxy = proxy;
            }
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.

            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            int remove = Regex.Match(responseFromServer, "(\\w)*\\[\\],").Captures[0].Index + 3; ;

            string subJson = "{" + responseFromServer.Substring(remove, responseFromServer.Length - 2 - remove);

            JToken entireJson = JToken.Parse(subJson);

            JArray vehVendorAvails = entireJson["VehAvailRSCore"]["VehVendorAvails"].Value<JArray>();// Get suppliers
            Console.WriteLine(vehVendorAvails.Count);

            List<JOffer> offers = new List<JOffer>();
            foreach (var item in vehVendorAvails)
            {
                string supplier = item["Vendor"]["@CompanyShortName"].ToString();
                foreach (var vehicle in item["VehAvails"])
                {
                    string category = vehicle["VehAvailCore"]["Vehicle"]["@Code"].ToString();// category
                    string price = vehicle["VehAvailCore"]["TotalCharge"]["@RateTotalAmount"].ToString();//price
                    JOffer offer = new JOffer(supplier, price);
                    offer.SetCategory(category);
                    offers.Add(offer);
                    //if (offer.category.Equals("skip"))
                    //    Debug.WriteLine("Model" + vehicle["VehAvailCore"]["Vehicle"]["VehMakeModel"]["@Name"] + "Cateory " + category);//Model


                }
            }

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return offers;
        }

        public string GetAtlassSource(string site)
        {
            Console.WriteLine("-->RequestStarted");
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://2014.atlaschoice.com/vehicles/search/");
                if (addProxy)
                {
                    WebProxy proxy = new WebProxy(ipStr, port);
                    proxy.Credentials = new NetworkCredential(user, pass);
                    request.Proxy = proxy;

                }
                
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers.Add("Origin", @"http://2014.atlaschoice.com");
                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Referer = "http://2014.atlaschoice.com/vehicles/?country=88&return=same&drop-country=0&city=1511&drop-city=0&location=2342&drop-location=0&start=1473508800&end=1473595200&residence=lt&age=0&promo=&src=atlaschoice.us&lang=en&lang=en&external=1";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = site.Split('?')[1];
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();


                return ReadResponse(response);

            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)we.Response;
                //else return false;
            }
            catch (Exception e)
            {
                if (response != null) response.Close();
                //return false;
            }
            Console.WriteLine("--<RequestFinished");
            return null;
        }

        public string GetAtlasSourceStatic()
        {
            return System.IO.File.ReadAllText(@"C:\Users\Edvard\Documents\Visual Studio 2013\Projects\ConsoleApplication1\ConsoleApplication1\response.txt");
        }

        public string RemoveSpecialCharacters(string s)
        {
            Console.WriteLine("-->RemoveSpecialCharactersStarted");
            //StringBuilder sb = new StringBuilder();
            string sb = string.Empty;
            try
            {
                if (s != null)
                {
                    int len = s.Length;
                    char[] s2 = new char[len];
                    int i2 = 0;
                    for (int i = 0; i < len; i++)
                    {
                        char c = s[i];
                        if (c != '\r' && c != '\n' && c != '\t')
                            s2[i2++] = c;

                    }
                    sb = new String(s2, 0, i2);
                }
            }
            catch (Exception e)
            {
                Console.Write("EXCEPTION");

            }

            sb = sb.Replace(@"\", "");

            Console.WriteLine("--<RemoveSpecialCharactersFinished");
            return sb.ToString();
        }

        public string ReadResponse(HttpWebResponse response)
        {
            Console.WriteLine("-->ReadResponseStarted");
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
                    Console.WriteLine("--<ReadResponse Finished ");
                    return streamReader.ReadToEnd();
                }
            }
        }

        public List<JOffer> GetAtlassOffers(string siteSource)
        {
            Console.WriteLine("-->GetOfferCount Started");

            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(siteSource);
                Console.WriteLine("--<GetOffer attr Started");
                HtmlNodeCollection offersFound = doc.DocumentNode.SelectNodes(".//div[@data-price]");

                //HtmlNode offerFound = doc.DocumentNode.SelectNodes(".//div[@data-price]");

                List<JOffer> offers = new List<JOffer>();
                if (offersFound != null)
                {
                    foreach (HtmlNode divNode in offersFound)
                    {
                        HtmlAttribute price = divNode.Attributes["data-price"];
                        HtmlAttribute category = divNode.Attributes["data-acriss"];
                        //HtmlAttribute supplier = supplierInfo.Attributes["data-acriss"];
                        string supplier = divNode.SelectNodes(".//div[@class='overlay']//ul//li//a").ElementAt(2).Attributes["href"].Value.Split('/').Last();

                        JOffer offer = new JOffer(Atlass.GetSupplier(supplier), price.Value);

                        offer.SetCategory(category.Value);
                        offers.Add(offer);
                    }
                    Console.WriteLine("--<GetOffer attr Finished");
                    return offers;
                }
                else
                {
                    return offers;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Offers not found");
                return null;
            }
        }
    }
}