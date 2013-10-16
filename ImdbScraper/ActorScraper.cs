using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ImdbScraper
{
    public static class ActorScraper
    {
        private static string _sourceCode;
        private static string _actorId;

        public static void LoadActorInfo(string actorId)
        {
            _actorId = actorId;
            _sourceCode = WebScraper.ReadSourceCode("http://www.imdb.com/name/" + actorId + "/bio");
        }

        public static string Biography
        {
            get 
            {
                try
                {
                    int startIndex = _sourceCode.IndexOf("Mini Biography");
                    string scrap = _sourceCode.Substring(startIndex);
                    startIndex = scrap.IndexOf("<p>");
                    int endIndex = scrap.IndexOf("</p>");

                    return WebUtility.HtmlDecode(WebScraper.StripHtmlTags(scrap.Substring(startIndex + 3, endIndex)));
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public static DateTime DateOfBirth
        {
            get 
            {
                try
                {

                    int startIndex = _sourceCode.IndexOf("Date of Birth");
                    string scrap = _sourceCode.Substring(startIndex);
                    startIndex = scrap.IndexOf("\">");

                    scrap = scrap.Substring(startIndex + 2);

                    int endIndex = scrap.IndexOf("</a>");
                    string dayMonth = scrap.Substring(0, endIndex);

                    startIndex = scrap.IndexOf("\">");
                    string year = scrap.Substring(startIndex + 2, 4);

                    startIndex = dayMonth.IndexOf(" ");

                    int day = Convert.ToInt32(dayMonth.Substring(0, startIndex));
                    int yearInt = Convert.ToInt32(year);

                    dayMonth = dayMonth.Substring(startIndex + 1);

                    return DateTime.Parse(dayMonth + " " + day.ToString() + ", " + year.ToString());
                }
                catch (Exception)
                {
                    return new DateTime(1,1,1);
                }
            }
        }

        public static string FullName
        {
            get
            {
                int startIndex = _sourceCode.IndexOf("Biography for");
                string scrap = _sourceCode.Substring(startIndex);
                startIndex = scrap.IndexOf("/\"");
                scrap = scrap.Substring(startIndex + 3);
                int endIndex = scrap.IndexOf("</a>");

                return WebUtility.HtmlDecode(scrap.Substring(0, endIndex));

            }
        }

        public static string Name
        {
            get
            { 
                string fullName = FullName;

                try
                {
                    int startIndex = fullName.IndexOf(" ");

                    return fullName.Substring(0, startIndex);
                }
                catch (Exception)
                {
                    return fullName;
                }
            }
        }

        public static string Surname
        {
            get
            {
                try
                {
                    string fullName = FullName;

                    int startIndex = fullName.IndexOf(" ");

                    return fullName.Substring(startIndex + 1);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public static Bitmap Picture
        {
            get
            {
                try
                {
                    int startIndex = _sourceCode.IndexOf("<div class=\"photo\">");
                    string scrap = _sourceCode.Substring(startIndex);
                    scrap = Regex.Match(scrap, "http://.*jpg").Value;

                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(scrap);
                    Bitmap bitmap = new Bitmap(stream);

                    return bitmap;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }
}
