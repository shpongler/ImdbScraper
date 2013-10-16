using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImdbScraper.Model
{
    public class CsvMovieItem
    {
        public string ImdbId { get; set; }
        public decimal UserRating { get; set; }
    }
}
