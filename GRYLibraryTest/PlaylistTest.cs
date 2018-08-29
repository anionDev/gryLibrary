using GRYLibrary;
using GRYLibrary.Miscellaneous.Playlists;
using GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GRYLibraryTest
{
    [TestClass]
    public class PlaylistTest
    {
        private void CommonTest(string file, AbstractPlaylistHandler handler)
        {
            try
            {
                Assert.AreEqual(handler, AbstractPlaylistHandler.GetConcretePlaylistHandler(file));
                handler.CreatePlaylist(file);
                Assert.IsTrue(System.IO.File.Exists(file));
                List<string> currentExpectedContent = new List<string>();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));
                currentExpectedContent.Add(@"C:\a\b.mp3");
                currentExpectedContent.Add(@"C:\A\c.unknownextension");
                handler.AddSongsToPlaylist(file, currentExpectedContent);
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));

                currentExpectedContent.Add(@"\\ComputerName\SharedFolder\Resource");
                currentExpectedContent.Add(@"X:/a/d.Ogg");
                currentExpectedContent.Add(@"http://player.example.com/stream.mp3");
                handler.AddSongsToPlaylist(file, currentExpectedContent);
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));

                currentExpectedContent.Remove(@"X:/a/d.Ogg");
                handler.DeleteSongsFromPlaylist(file, new string[] { @"X:/a/d.Ogg" });
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));

                handler.DeleteSongsFromPlaylist(file, currentExpectedContent);
                currentExpectedContent.Clear();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));
            }
            finally
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

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
