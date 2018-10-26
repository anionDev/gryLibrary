using GRYLibrary;
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
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
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


            System.IO.File.Delete(file);
        }
        [TestMethod]
        public void CommonTestM3U()
        {
            CommonTest("Test.m3u", M3UHandler.Instance);
        }
        [Ignore]
        [TestMethod]
        public void CommonTestWPL()
        {
            CommonTest("Test.wpl", WPLHandler.Instance);
        }
        [TestMethod]
        public void CommonTestPLS()
        {
            CommonTest("Test.pls", PLSHandler.Instance);
        }
        [TestMethod]
        public void CommonTestM3USubfolder()
        {
            string directoryName = "test";
            string configurationFile = ".m3uconfiguration";
            string m3uFile1 = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            Utilities.EnsureDirectoryExists("test");
            Utilities.EnsureFileExists(configurationFile);
            System.IO.File.WriteAllText(configurationFile, @"replace:{DefaultPath};C:\Data\Music", System.Text.Encoding.UTF8);
            M3UHandler.Instance.CreatePlaylist(m3uFile1);
            M3UHandler.Instance.CreatePlaylist(m3uFile2);
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile1, new string[] { "trackA", nameOfm3ufile2, "{DefaultPath}\\trackB" });
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackC", "{DefaultPath}\\trackD" });

            HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile1, true, true));
            Assert.IsTrue(playlistItems.SetEquals(new string[] { "trackA", @"C:\Data\Music\trackB", "trackC", @"C:\Data\Music\trackD" }));

            System.IO.File.Delete(m3uFile1);
            System.IO.File.Delete(m3uFile2);
            System.IO.Directory.Delete(configurationFile);
            System.IO.File.Delete(directoryName);
        }
    }
}
