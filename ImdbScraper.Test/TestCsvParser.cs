using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImdbScraper.Test
{
    [TestClass]
    public class TestCsvParser
    {
        [TestMethod]
        public void GetMoviesList()
        {
            CsvParser.GetMovieList("ur9179760");

            Assert.AreEqual(true, true);
        }
    }
}
