using GRYLibrary.Core.Miscellaneous;
using GRYLibrary.Core.Playlists.ConcretePlaylistHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class PlaylistTest
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void CommonTest()
        {
            string playlistFile = "test.m3u";
            this.Clean(playlistFile);
            List<string> currentExpectedContent = new();
            try
            {
                CombinedPlaylistHandler handler = new();
                handler.CreatePlaylist(playlistFile);
                Assert.IsTrue(File.Exists(playlistFile));
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongs(playlistFile).ToList()));
                this.CreateFile(currentExpectedContent, @"a.mp3");
                this.CreateFile(currentExpectedContent, @"b.mp3");
                handler.AddSongsToPlaylist(playlistFile, currentExpectedContent);
                List<string> actualContent = handler.GetSongs(playlistFile).ToList();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(actualContent));

                handler.DeleteSongsFromPlaylist(playlistFile, currentExpectedContent);
                currentExpectedContent.Clear();
                actualContent = handler.GetSongs(playlistFile).ToList();
                Assert.AreEqual(0, actualContent.Count);

                currentExpectedContent.Add(@"https://example.com/stream.mp3");
                currentExpectedContent.Add(@"\\ComputerName\SharedFolder\Resource.mp3");
                currentExpectedContent.Add(@"X:\a\d.wma");
                currentExpectedContent.Add(@"http://player.example.com/stream.mp3");
                handler.AddSongsToPlaylist(playlistFile, currentExpectedContent);
                actualContent = handler.GetSongs(playlistFile).ToList();
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(actualContent));

                currentExpectedContent.Remove(@"X:/a/d.Ogg");
                handler.DeleteSongsFromPlaylist(playlistFile, new string[] { @"X:/a/d.Ogg" });
                Assert.IsTrue(currentExpectedContent.EqualsIgnoringOrder(handler.GetSongs(playlistFile).ToList()));

                handler.DeleteSongsFromPlaylist(playlistFile, currentExpectedContent);
                currentExpectedContent.Clear();
                actualContent = handler.GetSongs(playlistFile).ToList();
                Assert.AreEqual(0, actualContent.Count);
            }
            finally
            {
                this.Clean(currentExpectedContent);
                this.Clean(playlistFile);
            }
        }

        private void CreateFile(List<string> list, string file)
        {
            file = Core.Miscellaneous.Utilities.ResolveToFullPath(file);
            list.Add(file);
            Core.Miscellaneous.Utilities.EnsureFileExists(file);
        }

        private void Clean(string file)
        {
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(file);
        }
        private void Clean(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                this.Clean(file);
            }
        }
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void CommonTestM3UReferencedPlaylist()
        {
            string directoryName = @"test\test";
            string m3uFile = directoryName + "\\" + "test1.m3u";
            string nameOfm3ufile2 = "test2.m3u";
            string m3uFile2 = directoryName + "\\" + nameOfm3ufile2;
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(m3uFile);
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(m3uFile2);
                Core.Miscellaneous.Utilities.EnsureDirectoryDoesNotExist(directoryName);

                Core.Miscellaneous.Utilities.EnsureDirectoryExists(directoryName);
                new M3UHandler().CreatePlaylist(m3uFile);
                new CombinedPlaylistHandler().CreatePlaylist(m3uFile2);
                new CombinedPlaylistHandler().AddSongsToPlaylist(m3uFile, new string[] { "trackA.mp3", nameOfm3ufile2 });
                new CombinedPlaylistHandler().AddSongsToPlaylist(m3uFile2, new string[] { "trackB.mp3" });

                HashSet<string> playlistItems = new(CombinedPlaylistHandler.DefaultInstance.GetSongs(m3uFile));
                Assert.IsTrue(playlistItems.SetEquals(new string[] { Path.Combine(currentDirectory, directoryName + @"\trackA.mp3"), Path.Combine(currentDirectory, directoryName + @"\trackB.mp3") }));
            }
            finally
            {
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(m3uFile);
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(m3uFile2);
                Core.Miscellaneous.Utilities.EnsureDirectoryDoesNotExist(directoryName);

            }
        }
        private void EnsureFilesAreDeleted(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(file);
            }
        }

    }
}
