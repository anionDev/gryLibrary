using GRYLibrary.Core;
using GRYLibrary.Core.Playlists;
using GRYLibrary.Core.Playlists.ConcretePlaylistHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class PlaylistTest
    {
        private void CommonTest(string file, AbstractPlaylistHandler handler)
        {
            this.CleanFile(file);
            try
            {
                Assert.AreEqual(handler, AbstractPlaylistHandler.GetConcretePlaylistHandler(file));
                handler.CreatePlaylist(file);
                Assert.IsTrue(File.Exists(file));
                List<string> currentExpectedContent = new List<string>();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));
                currentExpectedContent.Add(@"C:\a\b.mp3");
                currentExpectedContent.Add(@"C:\A\c.unknownextension");
                handler.AddSongsToPlaylist(file, currentExpectedContent);
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongsFromPlaylist(file).ToList()));

                currentExpectedContent.Add(@"\\ComputerName\SharedFolder\Resource.mp3");
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

                byte[] contentBefore = File.ReadAllBytes(file);
                handler.AddSongsToPlaylist(file, newTracks, true);
                byte[] contentAfter = File.ReadAllBytes(file);
                CollectionAssert.AreEqual(contentBefore, contentAfter);
            }
            finally
            {

                this.CleanFile(file);
            }
        }

        private void CleanFile(string file)
        {
            Utilities.EnsureFileDoesNotExist(file);
        }

        [TestMethod]
        public void CommonTestM3U()
        {
            this.CommonTest("Test.m3u", M3UHandler.Instance);
        }
        [TestMethod]
        public void CommonTestPLS()
        {
            this.CommonTest("Test.pls", PLSHandler.Instance);
        }
        [TestMethod]
        public void CommonTestM3UReferencedPlaylist()
        {
            string directoryName = @"test\test";
            string m3uFile = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                Utilities.EnsureFileDoesNotExist(m3uFile);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);

                Utilities.EnsureDirectoryExists(directoryName);
                M3UHandler.Instance.CreatePlaylist(m3uFile);
                M3UHandler.Instance.CreatePlaylist(m3uFile2);
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile, new string[] { "trackA.mp3", nameOfm3ufile2 });
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackB.mp3" });

                HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile, true, true));
                Assert.IsTrue(playlistItems.SetEquals(new string[] { Path.Combine(currentDirectory, directoryName + @"\trackA.mp3"), Path.Combine(currentDirectory, directoryName + @"\trackB.mp3") }));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(m3uFile);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);

            }
        }
        [TestMethod]
        public void M3UConfigurationWithRelativePath1()
        {
            string directoryName = "test";
            string configurationFile = ".m3uconfiguration";
            string m3uFile1 = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                Utilities.EnsureFileDoesNotExist(m3uFile1);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);
                Utilities.EnsureFileDoesNotExist(configurationFile);
                Utilities.EnsureDirectoryExists("test");
                Utilities.EnsureFileExists(configurationFile);
                File.WriteAllText(configurationFile, "on:all" + Environment.NewLine + @"replace:{DefaultPath};C:\Data\Music", new UTF8Encoding(false));
                M3UHandler.Instance.CreatePlaylist(m3uFile1);
                M3UHandler.Instance.CreatePlaylist(m3uFile2);
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile1, new string[] { "trackA.mp3", nameOfm3ufile2, "{DefaultPath}\\trackB.mp3" });
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackC.mp3", "{DefaultPath}\\trackD.mp3" });

                HashSet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile1, true, true));
                string[] expected = new string[] { System.IO.Path.Combine(currentDirectory, @"test\trackA.mp3"), @"C:\Data\Music\trackB.mp3", System.IO.Path.Combine(currentDirectory, @"test\trackC.mp3"), @"C:\Data\Music\trackD.mp3" };
                Assert.IsTrue(playlistItems.SetEquals(expected));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(m3uFile1);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);
                Utilities.EnsureFileDoesNotExist(configurationFile);

            }
        }

        [TestMethod]
        public void M3UConfigurationWithRelativePath2()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            Encoding encoding = new UTF8Encoding(false);
            Dictionary<string, string[]> filesWithTheirContent = new Dictionary<string, string[]>();
            string defaultMusicFolder = @"C:\Data\MyMusicFolder";
            string mainPlaylistFile = "m3utest/dir1/t1.m3u";
            filesWithTheirContent.Add("m3utest/.m3uconfiguration", new string[] { "on:all", @"replace:{DefaultPath};" + defaultMusicFolder });
            filesWithTheirContent.Add(mainPlaylistFile, new string[] {
                @"myTrack1.mp3", @"{DefaultPath}\myTrack2.mp3",
                @"..\notwanted\notWanted1.mp3",
                @"..\notwanted\notWanted2.mp3",
                @"..\notwanted\notWanted3.mp3",
                @"..\notwanted\notWanted4.mp3",
                @"t1_2.m3u",
                @"-../dir4/tn4_1.m3u",
                @"..\notwanted\wanted.mp3",
                @"{DefaultPath}\notWanted7.mp3",
                @"..\wanted\wanted2.mp3",
            });
            filesWithTheirContent.Add("m3utest/dir1/t1_2.m3u", new string[] {
                @"myTrack3.mp3",
                @"{DefaultPath}\myTrack4.mp3",
                @"../dir2/t2.m3u",
            });
            filesWithTheirContent.Add("m3utest/dir2/t2.m3u", new string[] {
                @"myTrack5.mp3",
                @"{DefaultPath}\myTrack6.mp3",
                @"../dir3/t3.m3u",
                @"../notwanted/notWanted5.mp3",
                @"-{DefaultPath}\notWanted8.mp3",
                "-../dir4/tn4_3.m3u",
            });
            filesWithTheirContent.Add("m3utest/dir3/t3.m3u", new string[] {
                @"myTrack7.mp3",
                @"{DefaultPath}\myTrack8.mp3",
                @"-notWanted6.mp3",
                @"{DefaultPath}\notWanted8.mp3",
            });
            filesWithTheirContent.Add("m3utest/dir4/tn4_1.m3u", new string[] {
                @"..\notwanted\notWanted1.mp3",
                @"..\notwanted\notWanted2.mp3",
                @"..\notwanted\wanted.mp3", @"-tn4_2.m3u",
                @"{DefaultPath}\notWanted7.mp3",
                @"..\notwanted\notWanted3.mp3",
                @"..\notwanted\notWanted4.mp3",
            });
            filesWithTheirContent.Add("m3utest/dir4/tn4_2.m3u", new string[] {
                @"..\notwanted\wanted.mp3"
            });
            filesWithTheirContent.Add("m3utest/dir4/tn4_3.m3u", new string[] {
                @"../notwanted/notWanted5.mp3",
            });

            this.EnsureFilesAreDeleted(filesWithTheirContent.Keys);
            try
            {
                foreach (KeyValuePair<string, string[]> file in filesWithTheirContent)
                {
                    Utilities.EnsureFileExists(file.Key, true);
                    File.WriteAllLines(file.Key, file.Value, encoding);
                }
                ISet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(mainPlaylistFile));
                string[] expected = new string[] {
                    Path.Combine(currentDirectory, @"m3utest\dir1\myTrack1.mp3"),
                    @"C:\Data\MyMusicFolder\myTrack2.mp3",
                    Utilities.GetAbsolutePath(Path.Combine(currentDirectory,@"m3utest"), @"notwanted\wanted.mp3"),
                    Path.Combine(currentDirectory, @"m3utest\dir1\myTrack3.mp3"),
                    @"C:\Data\MyMusicFolder\myTrack4.mp3",
                    Path.Combine(currentDirectory, @"m3utest\dir2\myTrack5.mp3"),
                    @"C:\Data\MyMusicFolder\myTrack6.mp3",
                    Path.Combine(currentDirectory, @"m3utest\dir3\myTrack7.mp3"),
                    @"C:\Data\MyMusicFolder\myTrack8.mp3",
                    Utilities.GetAbsolutePath(Path.Combine(currentDirectory,@"m3utest"),@"wanted\wanted2.mp3"),
                };
                Assert.IsTrue(playlistItems.SetEquals(expected));
            }
            finally
            {
                this.EnsureFilesAreDeleted(filesWithTheirContent.Keys);
                Utilities.DeleteAllEmptyFolderTransitively("m3utest", true);
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
            string directoryName = Directory.GetCurrentDirectory() + "\\test";
            string configurationFile = ".m3uconfiguration";
            string m3uFile1 = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            try
            {
                Utilities.EnsureFileDoesNotExist(m3uFile1);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);
                Utilities.EnsureFileDoesNotExist(configurationFile);
                Utilities.EnsureDirectoryExists("test");
                Utilities.EnsureFileExists(configurationFile);
                File.WriteAllText(configurationFile, "on:all" + Environment.NewLine + @"replace:{DefaultPath};C:\Data\Music", new UTF8Encoding(false));
                M3UHandler.Instance.CreatePlaylist(m3uFile1);
                M3UHandler.Instance.CreatePlaylist(m3uFile2);
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile1, new string[] { "trackA.mp3", nameOfm3ufile2, "{DefaultPath}\\trackB.mp3" });
                M3UHandler.Instance.AddSongsToPlaylist(m3uFile2, new string[] { "trackC.mp3", "{DefaultPath}\\trackD.mp3" });

                ISet<string> playlistItems = new HashSet<string>(M3UHandler.Instance.GetSongsFromPlaylist(m3uFile1, true, true));
                Assert.IsTrue(playlistItems.SetEquals(new string[] { Path.Combine(directoryName, "trackA.mp3"), @"C:\Data\Music\trackB.mp3", Path.Combine(directoryName, "trackC.mp3"), @"C:\Data\Music\trackD.mp3" }));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(m3uFile1);
                Utilities.EnsureFileDoesNotExist(m3uFile2);
                Utilities.EnsureDirectoryDoesNotExist(directoryName);
                Utilities.EnsureFileDoesNotExist(configurationFile);

            }
        }
    }
}
