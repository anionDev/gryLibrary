using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Playlists
{
    public interface IPlaylistFileHandler
    {
        public Encoding Encoding { get; set; }
        public abstract void CreatePlaylist(string file);
        public abstract IEnumerable<string> GetSongs(string playlistFile);
        public abstract Tuple<IEnumerable<string>/*included files*/, IEnumerable<string>/*excluded files*/> GetSongsAndExcludedSongs(string playlistFile);
        public abstract void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs);
        public abstract void NormalizePlaylist(string playlistFile);
        public abstract void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete);
    }
}
