using GRLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GRLibrary.Miscellaneous.Playlists
{
    public abstract class AbstractPlaylistHandler
    {
        public static Dictionary<string, AbstractPlaylistHandler> ExtensionsOfReadablePlaylists { get; } = new Dictionary<string, AbstractPlaylistHandler>() { { "m3u", M3UHandler.Instance }, { "pls", PLSHandler.Instance }, { "wpl", WPLHandler.Instance } };
        public static Encoding Encoding { get; set; } = Encoding.UTF8;
        protected abstract IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile);
        protected abstract void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs);
        protected abstract void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete);
        private IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems, bool loadTransitively, ISet<string> excludedPlaylistFiles)
        {
            string locationOfFile = Path.GetDirectoryName(playlistFile);
            IEnumerable<string> referencedFiles = GetSongsFromPlaylistImplementation(playlistFile).Where(item => IsAllowedAsPlaylistItem(item));
            List<string> newList = new List<string>();
            foreach (string item in referencedFiles)
            {
                try
                {
                    string playlistItem;
                    if (Path.IsPathRooted(item))
                    {
                        playlistItem = item;
                    }
                    else
                    {
                        playlistItem = Path.GetFullPath(Path.Combine(locationOfFile, item));
                    }
                    string playlistItemToLower = playlistItem.ToLower();
                    if (IsReadablePlaylist(playlistItemToLower))
                    {
                        if (loadTransitively && (!excludedPlaylistFiles.Contains(playlistItemToLower)))
                        {
                            excludedPlaylistFiles.Add(playlistItemToLower);
                            newList.AddRange(ExtensionsOfReadablePlaylists[Path.GetExtension(playlistItemToLower).Substring(1)].GetSongsFromPlaylist(playlistItem, removeDuplicatedItems, loadTransitively, excludedPlaylistFiles));
                        }
                    }
                    else
                    {
                        newList.Add(playlistItem);
                    }
                }
                catch
                {
                    Utilities.NoOperation();
                }
                referencedFiles = newList;
            }
            if (removeDuplicatedItems)
            {
                referencedFiles = new HashSet<string>(referencedFiles);
            }
            return referencedFiles;

        }
        public static AbstractPlaylistHandler GetConcretePlaylistHandler(string file)
        {
            return ExtensionsOfReadablePlaylists[Path.GetExtension(file.ToLower()).Substring(1)];
        }
        public IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems = true, bool loadTransitively = true)
        {
            return GetSongsFromPlaylist(playlistFile, removeDuplicatedItems, loadTransitively, new HashSet<string>());
        }

        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            AddSongsToPlaylistImplementation(playlistFile, newSongs.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public static bool IsReadablePlaylist(string file)
        {
            return ExtensionsOfReadablePlaylists.ContainsKey(Path.GetExtension(file.ToLower()).Substring(1));
        }
        public static bool IsAllowedAsPlaylistItem(string item)
        {
            return !item.Equals(string.Empty);
        }
    }
}

