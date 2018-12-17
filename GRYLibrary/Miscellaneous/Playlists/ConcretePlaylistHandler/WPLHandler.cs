using System;
using System.Collections.Generic;
namespace GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class WPLHandler : AbstractPlaylistHandler
    {
        public static WPLHandler Instance { get; } = new WPLHandler();
        private WPLHandler() { }
        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        protected override IEnumerable<string> GetSongsFromPlaylist(string playlistFile)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        public override void CreatePlaylist(string file)
        {
            throw new NotImplementedException();
        }
    }
}
