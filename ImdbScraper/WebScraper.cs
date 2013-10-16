using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ImdbScraper
{
    public static class WebScraper
    {

        public static string ReadSourceCode(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string source = sr.ReadToEnd();
                sr.Close();
                response.Close();

                return source;
            }
            catch (WebException)
            {
                return "";
            }

        }

        public static string StripHtmlTags(string source)
        {
            return Regex.Replace(source, @"<[^>]*>", string.Empty);
        }

        public static int ParseNumber(string inString)
        {
            return Convert.ToInt32(Regex.Replace(inString, "[^0-9]", string.Empty));
        }

        public static List<string> ParseActors(string inScrap)
        {
            List<string> outList = new List<string>();

            foreach (Match match in Regex.Matches(inScrap, @"nm[0-9]{7}"))
            {
                outList.Add(match.Value);
            }

            return outList.Distinct().ToList();
        }

        public static List<string> RemoveEmpty(List<string> inList)
        {
            List<string> outList = new List<string>();

            foreach (string item in inList)
            {
                if(!string.IsNullOrWhiteSpace(item) && !string.IsNullOrEmpty(item)) outList.Add(item.Trim());
            }

            return outList;
        }
    }
}
