using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class PLSHandler : AbstractPlaylistHandler
    {
        public static PLSHandler Instance { get; } = new PLSHandler();
        private PLSHandler() { }
        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            int amountOfItems = GetAmountOfItems(playlistFile);
            File.AppendAllText(playlistFile, Environment.NewLine, Encoding);
            foreach (string newItem in newSongs)
            {
                amountOfItems = amountOfItems + 1;
                File.AppendAllLines(playlistFile, new string[] { string.Empty, $"File{amountOfItems.ToString()}={newItem}" }, Encoding);
            }
            SetAmountOfItems(playlistFile, amountOfItems);
        }

        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        public int GetAmountOfItems(string playlistFile)
        {
            foreach (string line in File.ReadLines(playlistFile))
            {
                if (line.ToLower().StartsWith("numberofentries="))
                {
                    return int.Parse(line.Split('=')[1].Trim());
                }
            }
            return GetSongsFromPlaylistImplementation(playlistFile).Count();
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

        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            int amountOfItems = GetAmountOfItems(playlistFile);
            string[] lines = File.ReadLines(playlistFile, Encoding).ToArray();
            List<string> result = new List<string>();
            foreach (string line in lines)
            {
                foreach (string itemToDelete in songsToDelete)
                {
                    if (line.ToLower().StartsWith("file") && line.Contains("=") && !line.Split('=')[1].Trim().Equals(itemToDelete))
                    {
                        amountOfItems = amountOfItems - 1;
                        result.Add(line);
                    }
                }
            }
            File.WriteAllLines(playlistFile, lines, Encoding);
            SetAmountOfItems(playlistFile, amountOfItems);
        }

        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
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
            return result;
        }

        public override void CreatePlaylist(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
                File.AppendAllLines(file, new string[] { "[playlist]", string.Empty, "NumberOfEntries=0" });
            }
        }
    }
}