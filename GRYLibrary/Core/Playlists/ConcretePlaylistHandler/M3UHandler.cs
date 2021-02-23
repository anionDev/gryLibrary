using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : IPlaylistFileHandler
    {
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);

        public void CreatePlaylist(string file)
        {
            File.Create(file).Close();
        }

        public Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsAndExcludedSongs(string playlistFile)
        {
            IEnumerable<string> lines = File.ReadAllLines(playlistFile, this.Encoding)
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"));
            List<string> includedItems = new List<string>();
            List<string> excludedItems = new List<string>();
            foreach (string line in lines)
            {
                if (line.StartsWith("-"))
                {
                    excludedItems.Add(line.Substring(1));
                }
                else
                {
                    includedItems.Add(line);
                }
            }
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(includedItems, excludedItems);
        }

        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            File.AppendAllLines(playlistFile, newSongs, this.Encoding);
        }

        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            File.AppendAllLines(playlistFile, songsToDelete.Select(song => "-" + song), this.Encoding);
        }

        public void NormalizePlaylist(string playlistFile)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSongs(string playlistFile)
        {
            return GetSongsAndExcludedSongs(playlistFile).Item1;
        }
    }
}
