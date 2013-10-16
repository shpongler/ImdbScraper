using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImdbScraper;

namespace ImdbScraper.Test
{
    [TestClass]
    public class TestMovies
    {
        [TestMethod]
        public void GetTitle01()
        {
            MovieScraper.LoadMovieInfo("tt0054215");

            Assert.AreEqual("Psiho (1960)", MovieScraper.Title);
        }

        [TestMethod]
        public void GetTitle02()
        {
            MovieScraper.LoadMovieInfo("tt0116922");

            Assert.AreEqual("Lost Highway (1997)", MovieScraper.Title);
        }
    }
}
