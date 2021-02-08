using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : AbstractPlaylistHandler
    {
        public static M3UHandler Instance { get; } = new M3UHandler();
        private M3UHandler() { }
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            if (!Utilities.FileIsEmpty(playlistFile) && !Utilities.FileEndsWithEmptyLine(playlistFile))
            {
                File.AppendAllText(playlistFile, Environment.NewLine, Encoding);
            }
            File.AppendAllLines(playlistFile, newSongs, Encoding);
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            List<string> files = new List<string>();
            foreach (string item in File.ReadAllLines(playlistFile, Encoding))
            {
                if (!songsToDelete.Contains(item))
                {
                    files.Add(item);
                }
            }
            File.WriteAllLines(playlistFile, files, Encoding);
        }

        protected override Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsFromPlaylist(string playlistFile)
        {
            if (!File.Exists(playlistFile))
            {
                throw new FileNotFoundException(playlistFile);
            }
            string directoryOfPlaylistfile = new DirectoryInfo(playlistFile).Parent.FullName;
            string directory = Path.GetDirectoryName(playlistFile);
            List<string> lines = File.ReadAllLines(playlistFile, Encoding)
                .Select(line => line.Replace("\"", string.Empty).Trim())
                .Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#")))
                .Select(line => Utilities.ResolveToFullPath(line, directoryOfPlaylistfile))
                .ToList();
            List<string> includedItems = new List<string>();
            List<string> excludedItems = new List<string>();

            foreach (string line in lines)
            {
                if (line.StartsWith("-"))
                {
                    excludedItems.Add(line[1..]);
                }
                else
                {
                    includedItems.Add(line);
                }
            }
            includedItems = includedItems.SelectMany(item => Resolve(item)).ToList();
            excludedItems = includedItems.SelectMany(item => Resolve(item)).ToList();
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(includedItems, excludedItems);
        }

        private IEnumerable<string> Resolve(string item)
        {
            List<string> resultList = new List<string>();
            if (item.ToLower().EndsWith(".m3u"))
            {
                return this.GetSongs(item, true/*TODO this value must be inherited*/, true/*TODO this value must be inherited*/);
            }
            else if (IsSingleMusicFile(item))
            {
                resultList.Add(item);
            }
            else if (Directory.Exists(item))
            {
                Utilities.ForEachFileAndDirectoryTransitively(item,
                    (string currentDirectory, object argument/*resultList*/) =>
                    {
                        List<string> resultListTyped = (List<string>)argument;
                        resultListTyped.AddRange(this.GetSongs(currentDirectory, true/*TODO this value must be inherited*/, true/*TODO this value must be inherited*/));
                    }, (string currentFile, object argument/*resultList*/) =>
                    {
                        List<string> resultListTyped = (List<string>)argument;
                        resultListTyped.AddRange(this.GetSongs(currentFile, true/*TODO this value must be inherited*/, true/*TODO this value must be inherited*/));
                    }, false, resultList, resultList);
            }
            else
            {
                Utilities.NoOperation();
            }
            return resultList;
        }

        public override void CreatePlaylist(string file)
        {
            Utilities.EnsureFileExists(file);
        }

    }
}
