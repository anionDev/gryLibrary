using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : AbstractPlaylistHandler
    {
        public static M3UHandler Instance { get; } = new M3UHandler();
        public M3UHandler() : base()
        {
        }
        public M3UHandler(IList<(string/*alias*/, string/*folder*/)> defaultMusicFolder) : base(defaultMusicFolder)
        {
        }

        public override void CreatePlaylist(string file)
        {
            File.Create(file).Close();
        }

        protected override Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            IEnumerable<string> lines = File.ReadAllLines(playlistFile, Encoding)
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"));
            List<string> includedItems = new List<string>();
            List<string> excludedItems = new List<string>();
            foreach (string line in lines)
            {
                if (line.StartsWith("-"))
                {
                    excludedItems.Add(line);
                }
                else
                {
                    includedItems.Add(line);
                }
            }
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(includedItems, excludedItems);
        }

        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            File.AppendAllLines(playlistFile, newSongs, Encoding);
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            File.AppendAllLines(playlistFile, songsToDelete.Select(song => "-" + song), Encoding);
        }

        protected override void NormalizePlaylistImplementation(string playlistFile)
        {
            throw new NotImplementedException();
        }
    }
}
