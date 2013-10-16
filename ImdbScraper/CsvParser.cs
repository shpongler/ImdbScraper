using ImdbScraper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImdbScraper
{
    public static class CsvParser
    {
        public static List<CsvMovieItem> GetMovieList(string imdbUserId)
        {
            string csvLink = "http://www.imdb.com/list/export?list_id=ratings&author_id=" + imdbUserId;

            WebClient client = new WebClient();

            string csvData = client.DownloadString(csvLink);

            List<CsvMovieItem> outList = new List<CsvMovieItem>();

            List<string> lines = new List<string>(csvData.Split('\n'));

            lines.RemoveAt(0); // Remove headers

            foreach (string item in lines)
            {
                try
                {
                    string[] fields = item.Split(',');

                    CsvMovieItem movieItem = new CsvMovieItem();
                    movieItem.ImdbId = fields[1].Replace("\"", String.Empty);
                    movieItem.UserRating = Convert.ToDecimal(fields[8].Replace("\"", String.Empty));

                    outList.Add(movieItem);
                }
                catch
                {

                }
            }

            return outList;
        }
    }
}
