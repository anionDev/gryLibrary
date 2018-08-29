using GRYLibrary.Miscellaneous.Playlists;
using GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibraryTest
{
    [TestClass]
    public class PlaylistTest
    {
        private void CommonTest(string file, AbstractPlaylistHandler handler)
        {
            Assert.AreEqual(handler, AbstractPlaylistHandler.GetConcretePlaylistHandler(file));
            handler.CreatePlaylist(file);
            Assert.IsTrue(System.IO.File.Exists(file));
            List<string> currentExpectedContent = new List<string>();
            Assert.IsTrue(currentExpectedContent.SequenceEqual(handler.GetSongsFromPlaylist(file)));

        }
        [TestMethod]
        public void TestM3U()
        {
            CommonTest("Test.m3u", M3UHandler.Instance);
        }
        [Ignore]
        [TestMethod]
        public void TestWPL()
        {
            CommonTest("Test.wpl", WPLHandler.Instance);
        }
        [Ignore]
        [TestMethod]
        public void TestPLS()
        {
            CommonTest("Test.pls", PLSHandler.Instance);
        }
    }
}
