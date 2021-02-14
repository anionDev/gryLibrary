using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class PLSHandler : AbstractPlaylistHandler
    {
        public static PLSHandler Instance { get; } = new PLSHandler();
        public PLSHandler() : base()
        {
        }
        public PLSHandler(IList<(string/*alias*/, string/*folder*/)> defaultMusicFolder) : base(defaultMusicFolder)
        {
        }

        public override void CreatePlaylist(string file)
        {
            throw new NotImplementedException();
        }

        protected override Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            throw new NotImplementedException();
        }

        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            throw new NotImplementedException();
        }

        protected override void NormalizePlaylistImplementation(string playlistFile)
        {
            throw new NotImplementedException();
        }
    }
}