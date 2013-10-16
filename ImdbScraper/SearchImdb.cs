using System;
using System.Text;
using System.Net;
using System.IO;

namespace ImdbScraper
{
    public static class SearchImdb
    {

        public static string GetImdbId(string searchString)
        {
            
            searchString = searchString.Replace(' ', '+');
            string sourceCode = WebScraper.ReadSourceCode("http://www.imdb.com/find?s=all&q=" + searchString);

            string scrap = "";

            try
            {
                int startIndex = sourceCode.IndexOf("<table class=\"findList\">");
                scrap = sourceCode.Substring(startIndex);
                startIndex = scrap.IndexOf("/title/");
                scrap = scrap.Substring(startIndex + 7);
                scrap = scrap.Substring(0, 9);
            }

            catch (ArgumentOutOfRangeException)
            {
                return "tt0348529";
            }



            return scrap;
        }

    }

    public class SearchImdbExceptionMovieNotFound : Exception
    {
        
    }
}
