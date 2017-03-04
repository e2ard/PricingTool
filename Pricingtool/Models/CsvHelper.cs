using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace getLayout.Models
{
    public class CsvHelper
    {
        private string MakeRequests(string linkId)
        {
            HttpWebResponse response;
            string responseText = string.Empty;

            if (Request_gmlithuania_fusemetrix_com(linkId, out response))
            {
                responseText = ReadResponse(response);

                response.Close();
            }
            return responseText;
        }

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

        private bool Request_gmlithuania_fusemetrix_com(string linkId, out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gmlithuania.fusemetrix.com/bespoke/rate_manager/old_price_override/overrideprice_edit.php?id=" + linkId + "&export=true");

                request.KeepAlive = true;
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Referer = "http://gmlithuania.fusemetrix.com/bespoke/overrideprice_edit.php?id=280134";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
                request.Headers.Set(HttpRequestHeader.Cookie, @"PHPSESSID=lf6jrphkqu7kpl0k3nu3dbuoc1; mwoid=24; mwornd=327915990");

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
       
        private int GetBrokerIndex(string[] brokerArray, string brokerName){
            for(int i = 0; i < brokerArray.Count(); i++)
            {
                if (brokerArray.ElementAt(i).Equals(brokerName))
                    return i;
            }
            return 0;
        }
        public List<float> GetFuseSetetrixRates(string linkId)
        {
            string fileList = MakeRequests(linkId);
            string[] fileInLines = fileList.Split('\n');
            string[] tempStr;

            tempStr = fileList.Split(',');
            int brokerIndex = 0;
            brokerIndex = GetBrokerIndex(fileInLines[0].Split(','), "CTR");

            List<float> brokerValues = new List<float>();
            foreach (string line in fileInLines)
            {
                tempStr = line.Split(',');
                if (tempStr.Count() < brokerIndex)
                    break;
                 
                float parsedValue;
                List<string> splitted = new List<string>();

                foreach (string item in tempStr)
                {
                    
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        splitted.Add(item);
                    }
                }
                if (float.TryParse(splitted.ElementAt(brokerIndex), out parsedValue))
                    brokerValues.Add(parsedValue);
            }
            return brokerValues;
        }

        public List<float> GetFuseSetetrixRatesJIG(string linkId)
        {
            string fileList = MakeRequests(linkId);
            string[] fileInLines = fileList.Split('\n');
            string[] tempStr;

            tempStr = fileList.Split(',');
            int brokerIndex = 0;
            brokerIndex = GetBrokerIndex(fileInLines[0].Split(','), "JIG");

            List<float> brokerValues = new List<float>();
            foreach (string line in fileInLines)
            {
                tempStr = line.Split(',');
                if (tempStr.Count() < brokerIndex)
                    break;

                float parsedValue;
                List<string> splitted = new List<string>();

                foreach (string item in tempStr)
                {

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        splitted.Add(item);
                    }
                }
                if (float.TryParse(splitted.ElementAt(brokerIndex), out parsedValue))
                    brokerValues.Add(parsedValue);
            }
            return brokerValues;
        }
    }
}