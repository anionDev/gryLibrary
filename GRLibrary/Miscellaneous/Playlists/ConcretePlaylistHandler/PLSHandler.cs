using System;
using System.Collections.Generic;
namespace GRLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class PLSHandler : AbstractPlaylistHandler
    {

        public override string GetExtension()
        {
            return "pls";
        }
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            throw new NotImplementedException();
        }
    }
}