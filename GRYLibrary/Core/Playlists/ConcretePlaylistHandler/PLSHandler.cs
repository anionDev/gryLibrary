using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class PLSHandler : AbstractPlaylistHandler
    {
        public static PLSHandler Instance { get; } = new PLSHandler();
        private PLSHandler() { }

        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            int amountOfItems = this.GetAmountOfItems(playlistFile);
            if (!Utilities.FileIsEmpty(playlistFile) && !Utilities.FileEndsWithEmptyLine(playlistFile))
            {
                File.AppendAllText(playlistFile, Environment.NewLine, Encoding);
            }
            foreach (string newItem in newSongs)
            {
                amountOfItems += 1;
                File.AppendAllLines(playlistFile, new string[] { string.Empty, $"File{amountOfItems}={newItem}" }, Encoding);
            }
            this.SetAmountOfItems(playlistFile, amountOfItems);
        }

        public int GetAmountOfItems(string playlistFile)
        {
            foreach (string line in File.ReadLines(playlistFile))
            {
                if (line.ToLower().StartsWith("numberofentries="))
                {
                    return int.Parse(line.Split('=')[1].Trim());
                }
            }
            return this.GetSongsFromPlaylist(playlistFile, true, true).Count();
        }
        private void SetAmountOfItems(string playlistFile, int amount)
        {
            string newNumberOfEntriesLine = "NumberOfEntries=" + amount.ToString();
            string[] lines = File.ReadLines(playlistFile, Encoding).ToArray();
            bool numberOfEntriesLineFound = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].ToLower().StartsWith("numberofentries="))
                {
                    lines[i] = newNumberOfEntriesLine;
                    numberOfEntriesLineFound = true;
                }
            }
            if (!numberOfEntriesLineFound)
            {
                lines = lines.Concat(new[] { newNumberOfEntriesLine }).ToArray();
            }
            File.WriteAllLines(playlistFile, lines, Encoding);
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            string[] lines = File.ReadLines(playlistFile, Encoding).ToArray();
            List<string> result = new List<string>();
            foreach (string line in lines)
            {
                if (line.ToLower().StartsWith("file") && line.Contains("="))
                {
                    string item = line.Split('=')[1].Trim();
                    if (!songsToDelete.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }
            File.WriteAllText(playlistFile, string.Empty, Encoding);
            this.InitializePLSFile(playlistFile);
            this.AddSongsToPlaylistImplementation(playlistFile, result);
        }

        protected override Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsFromPlaylist(string playlistFile)
        {
            List<string> result = new List<string>();
            foreach (string line in File.ReadLines(playlistFile, Encoding))
            {
                try
                {
                    if (line.ToLower().StartsWith("file") && line.Contains("="))
                    {
                        result.Add(line.Split('=')[1].Trim());
                    }
                }
                catch
                {
                    Utilities.NoOperation();
                }
            }
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(result, Enumerable.Empty<string>());
        }

        public override void CreatePlaylist(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
                this.InitializePLSFile(file);
            }
        }

        private void InitializePLSFile(string file)
        {
            File.AppendAllLines(file, new string[] { "[playlist]", string.Empty, "NumberOfEntries=0" });
        }
    }
}