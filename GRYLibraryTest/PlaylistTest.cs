using GRYLibrary;
using GRYLibrary.Miscellaneous.Playlists;
using GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRYLibraryTest
{
    [TestClass]
    public class PlaylistTest
    {
        private void CommonTest(string file, AbstractPlaylistHandler handler)
        {
            this.Clean(file);
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
                currentExpectedContent.Add(@"X:\a\d.Ogg");
                currentExpectedContent.Add(@"http://player.example.com/stream.mp3");
                handler.AddSongsToPlaylist(file, currentExpectedContent);
                List<string> actual = handler.GetSongsFromPlaylist(file).ToList();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(actual));

                currentExpectedContent.Remove(@"X:/a/d.Ogg");
                handler.DeleteSongsFromPlaylist(file, new string[] { @"X:/a/d.Ogg" });
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));

                handler.DeleteSongsFromPlaylist(file, currentExpectedContent);
                currentExpectedContent.Clear();
                List<string> items = handler.GetSongsFromPlaylist(file).ToList();
                Assert.AreEqual(0, items.Count);
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(items));

                string[] newTracks = new string[] { @"X:\a\d.Ogg" };
                handler.AddSongsToPlaylist(file, newTracks);

                byte[] contentBefore = System.IO.File.ReadAllBytes(file);
                handler.AddSongsToPlaylist(file, newTracks, true);
                byte[] contentAfter = System.IO.File.ReadAllBytes(file);
                CollectionAssert.AreEqual(contentBefore, contentAfter);
            }
            finally
            {

                this.Clean(file);
            }
        }

        private void Clean(string file)
        {
            Utilities.EnsureFileDoesNotExist(file);
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
        public void CommonTestM3UConfigurationWithRelativePath1()
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
            System.IO.File.WriteAllText(configurationFile, @"replace:{DefaultPath};C:\Data\Music", new UTF8Encoding(false));
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
        public void CommonTestM3UConfigurationWithRelativePath2()
        {
            Encoding encoding = new UTF8Encoding(false);
            Dictionary<string, string[]> filesWithTheirContent = new Dictionary<string, string[]>();
            string defaultMusicFolder = @"C:\Data\MyMusicFolder";
            string mainPlaylistFile = "m3utest/dir1/t1.m3u";
            filesWithTheirContent.Add("m3utest/.m3uconfiguration", new string[] { @"replace:{DefaultPath};" + defaultMusicFolder });
            filesWithTheirContent.Add(mainPlaylistFile, new string[] { @"myTrack1.mp3", @"{DefaultPath}\myTrack2.mp3", @"notWanted1.mp3", @"notWanted2.mp3", @"notWanted3.mp3", @"notWanted4.mp3", @"t1_2.m3u", @"-../dir4/tn4_1.m3u", @"wanted.mp3" });
            filesWithTheirContent.Add("m3utest/dir1/t1_2.m3u", new string[] { @"myTrack3.mp3", @"{DefaultPath}\myTrack4.mp3", @"../dir2/t2.m3u" });
            filesWithTheirContent.Add("m3utest/dir2/t2.m3u", new string[] { @"myTrack5.mp3", @"{DefaultPath}\myTrack6.mp3", @"../dir3/t3.m3u", @"notWanted5.mp3", "-../dir4/tn4_3.m3u" });
            filesWithTheirContent.Add("m3utest/dir3/t3.m3u", new string[] { @"myTrack7.mp3", @"{DefaultPath}\myTrack8.mp3", @"-notWanted6.mp3" });
            filesWithTheirContent.Add("m3utest/dir4/tn4_1.m3u", new string[] { @"notWanted1.mp3", @"notWanted2.mp3", @"wanted.mp3", @"-tn4_2.m3u" });
            filesWithTheirContent.Add("m3utest/dir4/tn4_2.m3u", new string[] { @"wanted.mp3" });
            filesWithTheirContent.Add("m3utest/dir4/tn4_3.m3u", new string[] { @"notWanted5.mp3" });

            this.EnsureFilesAreDeleted(filesWithTheirContent.Keys);
            try
            {
                foreach (KeyValuePair<string, string[]> file in filesWithTheirContent)
                {
                    Utilities.EnsureFileExists(file.Key, true);
                    System.IO.File.WriteAllLines(file.Key, file.Value, encoding);
                }
                IEnumerable<string> playlistItems = M3UHandler.Instance.GetSongsFromPlaylist(mainPlaylistFile);
                throw new NotImplementedException();
            }
            finally
            {
                this.EnsureFilesAreDeleted(filesWithTheirContent.Keys);
            }
        }

        private void EnsureFilesAreDeleted(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Utilities.EnsureFileDoesNotExist(file);
            }
        }

        [TestMethod]
        public void CommonTestM3UConfigurationWithAbsolutePath()
        {
            string directoryName = System.IO.Directory.GetCurrentDirectory() + "\\test";
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
            System.IO.File.WriteAllText(configurationFile, @"replace:{DefaultPath};C:\Data\Music", new UTF8Encoding(false));
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
