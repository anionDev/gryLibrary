using System;
using System.Collections.Generic;
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
        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            throw new NotImplementedException();
        }
    }
}