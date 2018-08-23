using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GRLibrary.Miscellaneous.Playlists
{
    public abstract class AbstractPlaylistHandler
    {
        public Encoding Encoding { get; set; }
        protected abstract IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile);
        protected abstract void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs);
        protected abstract void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete);

        public IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems = true, bool loadTransitively = true)
        {
            IEnumerable<string> result = GetSongsFromPlaylistImplementation(playlistFile).Where(item => IsAllowedAsPlaylistItem(item));
            if (loadTransitively)
            {
                //TODO: here is a bug: if the file "a.m3u" contains the line "a.m3u" (transitively) this operation may cause an endless-loop
                List<string> newList = new List<string>();
                foreach (string item in result)
                {
                    if (item.ToLower().EndsWith(".m3u"))
                    {
                        newList.AddRange(GetSongsFromPlaylist(playlistFile, removeDuplicatedItems, true));
                    }
                    else
                    {
                        newList.Add(item);
                    }
                }
                result = newList;
            }
            if (removeDuplicatedItems)
            {
                result = new HashSet<string>(result);
            }
            return result;
        }
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            AddSongsToPlaylistImplementation(playlistFile, newSongs.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public static bool IsAllowedAsPlaylistItem(string item)
        {
            return !item.Equals(string.Empty);
        }
    }
}

