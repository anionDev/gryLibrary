using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class PLSHandler : IPlaylistFileHandler
    {
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            throw new NotImplementedException();
        }

        public void CreatePlaylist(string file)
        {
            throw new NotImplementedException();
        }

        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSongs(string playlistFile)
        {
            return this.GetSongsAndExcludedSongs(playlistFile).Item1;
        }

        public Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsAndExcludedSongs(string playlistFile)
        {
            throw new NotImplementedException();
        }

        public void NormalizePlaylist(string playlistFile)
        {
            throw new NotImplementedException();
        }
    }
}