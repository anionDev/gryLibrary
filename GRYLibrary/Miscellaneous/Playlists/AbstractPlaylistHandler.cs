using GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Miscellaneous.Playlists
{
    public abstract class AbstractPlaylistHandler
    {
        private static Dictionary<string, AbstractPlaylistHandler> _ExtensionsOfReadablePlaylists = null;
        public static Dictionary<string, AbstractPlaylistHandler> ExtensionsOfReadablePlaylists
        {
            get
            {
                if (_ExtensionsOfReadablePlaylists == null)
                {
                    _ExtensionsOfReadablePlaylists = new Dictionary<string, AbstractPlaylistHandler>();
                    _ExtensionsOfReadablePlaylists.Add("m3u", M3UHandler.Instance);
                    _ExtensionsOfReadablePlaylists.Add("pls", PLSHandler.Instance);
                    _ExtensionsOfReadablePlaylists.Add("wpl", WPLHandler.Instance);
                }
                return _ExtensionsOfReadablePlaylists;
            }
        }
        public static Encoding Encoding { get; set; } = Encoding.UTF8;
        public abstract void CreatePlaylist(string file);
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
                    if (new Uri(item).IsFile)
                    {
                        if (Path.IsPathRooted(item))
                        {
                            playlistItem = item;
                        }
                        else
                        {
                            playlistItem = Path.GetFullPath(Path.Combine(locationOfFile, item));
                        }
                    }
                    else
                    {
                        playlistItem = item;
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

        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs, bool addOnlyNotExistingSongs = true)
        {
            newSongs = newSongs.Where(item => IsAllowedAsPlaylistItem(item));
            if (newSongs.Count() > 0)
            {
                if (addOnlyNotExistingSongs)
                {
                    HashSet<string> newSongsAsSet = new HashSet<string>(newSongs);
                    IEnumerable<string> alreadyExistingItems = GetSongsFromPlaylist(playlistFile, true, false);
                    foreach (string alreadyExistingItem in alreadyExistingItems)
                    {
                        newSongsAsSet.Remove(alreadyExistingItem);
                    }
                    newSongs = newSongsAsSet;
                }
                AddSongsToPlaylistImplementation(playlistFile, newSongs);
            }
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public static bool IsReadablePlaylist(string file)
        {
            file = file.ToLower();
            foreach (KeyValuePair<string, AbstractPlaylistHandler> keyValuePair in ExtensionsOfReadablePlaylists)
            {
                if (file.EndsWith("." + keyValuePair.Key))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsAllowedAsPlaylistItem(string item)
        {
            return !item.Equals(string.Empty);
        }
    }
}

