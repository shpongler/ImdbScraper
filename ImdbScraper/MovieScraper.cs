using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


namespace ImdbScraper
{
    /// <summary>
    /// imdb scraper 
    /// v0.1 30.10.2011.
    /// by tema
    /// </summary>
    public static class MovieScraper
    {

        static string _sourceCodeMovie;
        static string _sourceCodeCast;
        private static string _imdbId;


        public static void LoadMovieInfo(string imdbId)
        {

            _sourceCodeMovie = WebScraper.ReadSourceCode("http://www.imdb.com/title/" + imdbId);
            _sourceCodeCast = WebScraper.ReadSourceCode("http://www.imdb.com/title/" + imdbId + "/fullcredits#cast");
            _imdbId = imdbId;
        }

        public static Dictionary<string, string> Cast
        {
            get
            {
                try
                {
                    int startIndex = _sourceCodeCast.IndexOf("Cast</a><span>");
                    int endIndex = _sourceCodeCast.IndexOf("Produced by</a>");
                    string scrap = _sourceCodeCast.Substring(startIndex, endIndex - startIndex);
                    Dictionary<string, string> roles = new Dictionary<string, string>();
                    string actorId, roleName;
                    bool odd = true;

                    while (true)
                    {

                        startIndex = scrap.IndexOf("<td class=\"hs\">");
                        if (odd)
                        {
                            endIndex = scrap.IndexOf("<tr class=\"even\"><td class=\"hs\">");
                            odd = false;
                        }
                        else
                        {
                            endIndex = scrap.IndexOf("<tr class=\"odd\"><td class=\"hs\">");
                            odd = true;
                        }
                        if (startIndex == -1) break;
                        if (endIndex == -1) endIndex = scrap.IndexOf("action=\"/character/create\"");

                        string newScrap = scrap.Substring(startIndex, endIndex - startIndex);
                        scrap = scrap.Substring(endIndex);


                        startIndex = newScrap.IndexOf("/name/nm") + 6;
                        actorId = newScrap.Substring(startIndex, 9);

                        startIndex = newScrap.IndexOf("<td class=\"char\">") + 17;
                        roleName = WebUtility.HtmlDecode(newScrap.Substring(startIndex));
                        startIndex = roleName.IndexOf("</td></tr>");
                        roleName = roleName.Substring(0, startIndex);
                        if (roleName.Contains("</a>"))
                        {
                            startIndex = roleName.IndexOf(">") + 1;
                            endIndex = roleName.IndexOf("</a>");
                            roleName = roleName.Substring(startIndex, endIndex - startIndex);
                        }
                        roles.Add(actorId, roleName);

                    }

                    return roles;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<string> Country
        {
            get
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("<h2>Details</h2>");
                    string scrap = _sourceCodeMovie.Substring(startIndex);

                    startIndex = scrap.IndexOf("Country:");
                    scrap = scrap.Substring(startIndex + 8);

                    int endIndex = scrap.IndexOf("</div>");
                    scrap = scrap.Substring(0, endIndex);
                    scrap = WebUtility.HtmlDecode(scrap);
                    scrap = WebScraper.StripHtmlTags(scrap);

                    List<string> outCountry = new List<string>( scrap.Split(new char[] {'\n', '|'}, StringSplitOptions.RemoveEmptyEntries) );

                    outCountry = WebScraper.RemoveEmpty(outCountry);

                    return outCountry;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<string> Director
        {
            get
            {
                List<string> outList = new List<string>();

                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("Director:</h4>");
                    string scrap = _sourceCodeMovie.Substring(startIndex);
                    startIndex = scrap.IndexOf("/name/nm");
                    scrap = scrap.Substring(startIndex + 6, 9);

                    outList.Add(scrap);

                    return outList;
                }
                catch (Exception)
                {
                    try
                    {
                        int startIndex = _sourceCodeMovie.IndexOf("Directors:</h4>");
                        string scrap = _sourceCodeMovie.Substring(startIndex);

                        int endIndex = scrap.IndexOf("</div>");
                        scrap = scrap.Substring(0, endIndex);

                        outList = WebScraper.ParseActors(scrap);

                        return outList;
                    }
                    catch (Exception)
                    {
                        return outList;
                    }
                }
            }
        }

        public static List<string> Genre
        {
            get 
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("<div class=\"infobar\">");
                    string scrap = _sourceCodeMovie.Substring(startIndex);
                    int endIndex = scrap.IndexOf("</div>");
                    startIndex = scrap.IndexOf("</time>");
                    scrap = scrap.Substring(startIndex);
                    scrap = scrap.Substring(0, endIndex);
                    scrap = WebScraper.StripHtmlTags(scrap);
                    scrap = WebUtility.HtmlDecode(scrap);

                    startIndex = scrap.IndexOf("-");
                    scrap = scrap.Substring(startIndex + 1);
                    if (scrap.Contains("-"))
                    {
                        endIndex = scrap.IndexOf("-");
                        scrap = scrap.Substring(0, endIndex);
                    }

                    List<string> genreList =
                        new List<string>(scrap.Split(new char[] {' ', '|'}, StringSplitOptions.RemoveEmptyEntries));
                    List<string> output = new List<string>();

                    genreList.ForEach(p => output.Add(p.Trim()));

                    output = WebScraper.RemoveEmpty(output);

                    return output;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string ImdbId
        {
            get { return _imdbId; }
        }

        public static List<string> Language
        {
            get
            {
                List<string> outList = new List<string>();

                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("Language:</h4>");
                    string scrap = _sourceCodeMovie.Substring(startIndex + 14);
                    int endIndex = scrap.IndexOf("</div>");

                    scrap = scrap.Substring(0, endIndex);
                    scrap = WebUtility.HtmlDecode(scrap);
                    scrap = WebScraper.StripHtmlTags(scrap);

                    outList = new List<string>(scrap.Split(new char[] { '\n', '|' }, StringSplitOptions.RemoveEmptyEntries));
                    outList = WebScraper.RemoveEmpty(outList);

                    return outList;
                }
                catch (Exception)
                {
                    return outList;
                }
            }
        }

        public static Bitmap MoviePoster
        {
            get
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("title=\"" + Title + "\"");
                    string scrap = _sourceCodeMovie.Substring(startIndex);

                    startIndex = scrap.IndexOf("src=\"");
                    scrap = scrap.Substring(startIndex + 5);

                    int endIndex = scrap.IndexOf("\"");
                    scrap = scrap.Substring(0, endIndex);

                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(scrap);
                    Bitmap bitmap = new Bitmap(stream);

                    return bitmap;

                }
                catch (Exception)
                {
                    try
                    {
                        int startIndex = _sourceCodeMovie.IndexOf("id=\"img_primary\"");
                        string scrap = _sourceCodeMovie.Substring(startIndex);

                        startIndex = scrap.IndexOf("img src=");
                        scrap = scrap.Substring(startIndex + 9);

                        int endIndex = scrap.IndexOf("\"");
                        scrap = scrap.Substring(0, endIndex);

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

        public static string Plot
        {
            get
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("itemprop=\"description\"");
                    int endIndex = _sourceCodeMovie.IndexOf("Director:");
                    string scrap = _sourceCodeMovie.Substring(startIndex, endIndex - startIndex);
                    startIndex = scrap.IndexOf(">");
                    endIndex = scrap.IndexOf("</p>");
                    scrap = scrap.Substring(startIndex + 1, endIndex - startIndex - 1);

                    scrap = WebUtility.HtmlDecode(scrap);
                    scrap = WebScraper.StripHtmlTags(scrap);

                    return scrap;
                }
                catch (Exception)
                {
                    return String.Empty;
                }
            }
        }

        public static decimal Rating
        {
            get
            {
                try
                {

                    int startIndex = _sourceCodeMovie.IndexOf("ratingValue\">");
                    string scrap = _sourceCodeMovie.Substring(startIndex + 13);
                    startIndex = scrap.IndexOf("Value\">");
                    scrap = Regex.Match(scrap, @"[0-9].[0-9]").Value;

                    return Convert.ToDecimal(scrap)/10;
                }
                catch (Exception)
                {
                    return 0m;
                }
            }
        }

        public static int Runtime
        {
            get 
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("Runtime:");
                    int endIndex = _sourceCodeMovie.IndexOf("Sound Mix:");
                    string scrap = _sourceCodeMovie.Substring(startIndex + 8, endIndex - startIndex - 8);
                    startIndex = scrap.IndexOf("\">");
                    endIndex = scrap.IndexOf("</time>");
                    scrap = scrap.Substring(startIndex, endIndex - startIndex);

                    return WebScraper.ParseNumber(scrap);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static string Storyline
        {
            get
            {
                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("<h2>Storyline</h2>");
                    string scrap = _sourceCodeMovie.Substring(startIndex);
                    startIndex = scrap.IndexOf("<p>");
                    scrap = scrap.Substring(startIndex + 3);
                    int endIndex = scrap.IndexOf("<");

                    scrap = scrap.Substring(0, endIndex);

                    return WebUtility.HtmlDecode(scrap);

                }
                catch (Exception)
                {
                    return String.Empty;
                }
            }
        }

        public static string Title
        {
            get
            {
                try
                {
                    string startTag = "<title>";
                    string endTag = "</title>";
                    string filterWord = " - IMDb";
                    int startIndex = _sourceCodeMovie.IndexOf(startTag);
                    int endIndex = _sourceCodeMovie.IndexOf(endTag);

                    string scrapCode = _sourceCodeMovie.Substring(startIndex + startTag.Length,
                                                                 endIndex - startIndex - startTag.Length);
                    scrapCode = scrapCode.Substring(0, scrapCode.Length - filterWord.Length);
                    scrapCode = WebUtility.HtmlDecode(scrapCode);

                    return scrapCode;
                }
                catch(Exception)
                {
                    return "ERROR";
                }

            }
        }

        public static string TitleWithoutYear
        {
            get
            {
                string title = Title;
                int startIndex = title.IndexOf(" (");
                title = title.Substring(0, startIndex);

                title = WebUtility.HtmlDecode(title);

                return title;
            }
        }

        public static List<string> Writer
        {
            get
            {
                List<string> outList = new List<string>();

                try
                {
                    int startIndex = _sourceCodeMovie.IndexOf("Writer:</h4>");
                    string scrap = _sourceCodeMovie.Substring(startIndex);
                    startIndex = scrap.IndexOf("/name/nm");
                    scrap = scrap.Substring(startIndex + 6, 9);

                    outList.Add(scrap);

                    return outList;
                }
                catch (Exception)
                {
                    try
                    {
                        int startIndex = _sourceCodeMovie.IndexOf("Writers:</h4>");
                        string scrap = _sourceCodeMovie.Substring(startIndex);

                        int endIndex = scrap.IndexOf("</div>");
                        scrap = scrap.Substring(0, endIndex);

                        outList = WebScraper.ParseActors(scrap);

                        return outList;
                    }
                    catch (Exception)
                    {
                        return outList;
                    }
                }
            }
        }

        public static int Year
        {
            get
            {
                try
                {
                    string year = Title;
                    int startIndex = year.IndexOf(" (");
                    year = year.Substring(startIndex + 2, 4);

                    return Convert.ToInt32(year);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }


    }

    public class MovieScraperExceptionDirectorNotResolved : Exception
    {
        
    }
}
