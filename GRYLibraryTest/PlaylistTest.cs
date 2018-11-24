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
            List<string> items = handler.GetSongsFromPlaylist(file).ToList();
            Assert.AreEqual(0,items.Count);
            Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(items));

            string[] newTracks = new string[] { @"X:/a/d.Ogg" };
            handler.AddSongsToPlaylist(file,newTracks );

            byte[] contentBefore = System.IO.File.ReadAllBytes(file);
            handler.AddSongsToPlaylist(file, newTracks,true);
            byte[] contentAfter = System.IO.File.ReadAllBytes(file);
            Assert.AreEqual(contentBefore, contentAfter);
            System.IO.File.Delete(file);
        }
        [TestMethod]
        public void CommonTestM3U()
        {
            this.CommonTest("Test.m3u", M3UHandler.Instance);
        }
        [Ignore]
        [TestMethod]
        public void CommonTestWPL()
        {
            this.CommonTest("Test.wpl", WPLHandler.Instance);
        }
        [TestMethod]
        public void CommonTestPLS()
        {
            this.CommonTest("Test.pls", PLSHandler.Instance);
        }
        [TestMethod]
        public void CommonTestM3UReferencedPlaylist()
        {
            string directoryName = "test\\test";
            string m3uFile = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            Utilities.EnsureFileDoesNotExist(m3uFile);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);

            Utilities.EnsureDirectoryExists(directoryName);
            M3UHandler.Instance.CreatePlaylist(m3uFile);
            M3UHandler.Instance.CreatePlaylist(m3uFile2);
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile, new string[] { "trackA.mp3", nameOfm3ufile2 });
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackB.mp3" });

            HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile, true, true));
            Assert.IsTrue(playlistItems.SetEquals(new string[] { "trackA.mp3", "trackB.mp3" }));
            Utilities.EnsureFileDoesNotExist(m3uFile);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);
        }
        [TestMethod]
        public void CommonTestM3UConfigurationWithRelativePath()
        {
            string directoryName = "test";
            string configurationFile = ".m3uconfiguration";
            string m3uFile1 = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            Utilities.EnsureFileDoesNotExist(m3uFile1);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);
            Utilities.EnsureFileDoesNotExist(configurationFile);
            Utilities.EnsureDirectoryExists("test");
            Utilities.EnsureFileExists(configurationFile);
            System.IO.File.WriteAllText(configurationFile, @"replace:{DefaultPath};C:\Data\Music", System.Text.Encoding.UTF8);
            M3UHandler.Instance.CreatePlaylist(m3uFile1);
            M3UHandler.Instance.CreatePlaylist(m3uFile2);
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile1, new string[] { "trackA.mp3", nameOfm3ufile2, "{DefaultPath}\\trackB.mp3" });
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackC.mp3", "{DefaultPath}\\trackD.mp3" });

            HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile1, true, true));
            Assert.IsTrue(playlistItems.SetEquals(new string[] { "trackA.mp3", @"C:\Data\Music\trackB.mp3", "trackC.mp3", @"C:\Data\Music\trackD.mp3" }));
            Utilities.EnsureFileDoesNotExist(m3uFile1);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);
            Utilities.EnsureFileDoesNotExist(configurationFile);
        }
        [TestMethod]
        public void CommonTestM3UConfigurationWithAbsolutePath()
        {
            string directoryName =System.IO.Directory.GetCurrentDirectory()+ "\\test";
            string configurationFile = ".m3uconfiguration";
            string m3uFile1 = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            Utilities.EnsureFileDoesNotExist(m3uFile1);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);
            Utilities.EnsureFileDoesNotExist(configurationFile);
            Utilities.EnsureDirectoryExists("test");
            Utilities.EnsureFileExists(configurationFile);
            System.IO.File.WriteAllText(configurationFile, @"replace:{DefaultPath};C:\Data\Music", System.Text.Encoding.UTF8);
            M3UHandler.Instance.CreatePlaylist(m3uFile1);
            M3UHandler.Instance.CreatePlaylist(m3uFile2);
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile1, new string[] { "trackA.mp3", nameOfm3ufile2, "{DefaultPath}\\trackB.mp3" });
            M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackC.mp3", "{DefaultPath}\\trackD.mp3" });

            HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile1, true, true));
            Assert.IsTrue(playlistItems.SetEquals(new string[] { "trackA.mp3", @"C:\Data\Music\trackB.mp3", "trackC.mp3", @"C:\Data\Music\trackD.mp3" }));
            Utilities.EnsureFileDoesNotExist(m3uFile1);
            Utilities.EnsureFileDoesNotExist(m3uFile2);
            Utilities.EnsureDirectoryDoesNotExist(directoryName);
            Utilities.EnsureFileDoesNotExist(configurationFile);
        }
    }
}
