﻿using GRYLibrary.Core.Playlists.ConcretePlaylistHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Playlists
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
                    _ExtensionsOfReadablePlaylists = new Dictionary<string, AbstractPlaylistHandler>
                    {
                        { "m3u", M3UHandler.Instance },
                        { "pls", PLSHandler.Instance }
                    };
                }
                return _ExtensionsOfReadablePlaylists;
            }
        }
        public static Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public IList<string> AllowedFiletypesForMusicFiles { get; } = new List<string>();
        public abstract void CreatePlaylist(string file);
        protected abstract Tuple<IEnumerable<string>/*included files*/, IEnumerable<string>/*included files*/> GetSongsFromPlaylist(string playlistFile);
        protected abstract void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs);
        protected abstract void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete);
        private IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFileName, bool removeDuplicatedItems, bool loadTransitively, ISet<string> excludedPlaylistFiles, string workingDirectory)
        {
            playlistFileName = this.NormalizePath(playlistFileName);
            Tuple<IEnumerable<string>, IEnumerable<string>> songsAndExcludedSongs = this.GetSongsFromPlaylist(Path.Combine(workingDirectory, playlistFileName));
            IEnumerable<string> referencedFiles = songsAndExcludedSongs.Item1.Where(item => this.IsAllowedAsPlaylistItem(item));
            IEnumerable<string> referencedExcludedFiles = songsAndExcludedSongs.Item2.Where(item => this.IsAllowedAsPlaylistItem(item));
            referencedFiles = this.ProcessList(referencedFiles, removeDuplicatedItems, loadTransitively, excludedPlaylistFiles, workingDirectory, playlistFileName);
            referencedExcludedFiles = this.ProcessList(referencedExcludedFiles, removeDuplicatedItems, loadTransitively, excludedPlaylistFiles, workingDirectory, playlistFileName);
            return referencedFiles.Except(referencedExcludedFiles);
        }

        private IEnumerable<string> ProcessList(IEnumerable<string> referencedFiles, bool removeDuplicatedItems, bool loadTransitively, ISet<string> alreadyProcessedPlaylistFiles, string workingDirectory, string playlistFileName)
        {
            List<string> newList = new List<string>();
            foreach (string item in referencedFiles)
            {
                try
                {
                    string playlistItem = null;
                    try
                    {
                        if (new Uri(item).IsFile)
                        {
                            if (Utilities.IsAbsolutePath(item))
                            {
                                playlistItem = item;
                            }
                            else if (Utilities.IsRelativePath(item))
                            {
                                playlistItem = Utilities.GetAbsolutePath(Path.GetDirectoryName(playlistFileName), item);
                            }
                            else
                            {
                                throw new Exception(item + " has an unknown format.");
                            }
                        }
                        else
                        {
                            playlistItem = item;
                        }
                    }
                    catch
                    {
                        playlistItem = new Uri(item, UriKind.Relative).OriginalString;
                    }
                    string playlistItemToLower = playlistItem.ToLower();
                    if (IsReadablePlaylist(playlistItemToLower))
                    {
                        if (loadTransitively && (!alreadyProcessedPlaylistFiles.Contains(playlistItemToLower)))
                        {
                            alreadyProcessedPlaylistFiles.Add(playlistItemToLower);
                            newList.AddRange(ExtensionsOfReadablePlaylists[Path.GetExtension(playlistItemToLower)[1..]].GetSongsFromPlaylistImplementation(playlistItem, removeDuplicatedItems, loadTransitively, alreadyProcessedPlaylistFiles, workingDirectory));
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

        private string NormalizePath(string path)
        {
            return path.Replace("/", "\\").Trim();
        }

        public static AbstractPlaylistHandler GetConcretePlaylistHandler(string file)
        {
            return ExtensionsOfReadablePlaylists[Path.GetExtension(file.ToLower())[1..]];
        }
        public IEnumerable<string> GetSongs(string file, bool removeDuplicatedItems = true, bool loadTransitively = true)
        {
            // TODO move logic from m3u handler to here
            string workingDirectory;
            if (Utilities.IsAbsolutePath(file))
            {
                workingDirectory = Path.GetDirectoryName(file);
            }
            else if (Utilities.IsRelativePath(file))
            {
                workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(file));
            }
            else
            {
                throw new Exception(file + " has an unknown format.");
            }
            return this.GetSongsFromPlaylist(new FileInfo(file).Name, workingDirectory, removeDuplicatedItems, loadTransitively);
        }
        public IEnumerable<string> GetSongsFromPlaylist(string playlistFile, string workingDirectory, bool removeDuplicatedItems = true, bool loadTransitively = true)
        {
            return this.GetSongsFromPlaylistImplementation(playlistFile, removeDuplicatedItems, loadTransitively, new HashSet<string>(), workingDirectory);
        }

        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs, bool addOnlyNotExistingSongs = true)
        {
            playlistFile = this.NormalizePath(playlistFile);
            newSongs = newSongs.Where(item => this.IsAllowedAsPlaylistItem(item));
            if (newSongs.Count() > 0)
            {
                if (addOnlyNotExistingSongs)
                {
                    HashSet<string> newSongsAsSet = new HashSet<string>(newSongs);
                    IEnumerable<string> alreadyExistingItems = this.GetSongs(playlistFile, true, false);
                    foreach (string alreadyExistingItem in alreadyExistingItems)
                    {
                        newSongsAsSet.Remove(alreadyExistingItem);
                    }
                    newSongs = newSongsAsSet;
                }
                this.AddSongsToPlaylistImplementation(playlistFile, newSongs);
            }
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            playlistFile = this.NormalizePath(playlistFile);
            this.DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete.Where(item => this.IsAllowedAsPlaylistItem(item)));
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
        public bool IsAllowedAsPlaylistItem(string item)
        {
            return (!string.IsNullOrWhiteSpace(item)) && this.HasAllowedExtension(item);
        }
        private bool HasAllowedExtension(string item)
        {
            string extension = Path.GetExtension(item);
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }
            else
            {
                if (this.AllowedFiletypesForMusicFiles.Count == 0)
                {
                    return true;
                }
                else
                {
                    extension = extension[1..].ToLower();
                    return (this.AllowedFiletypesForMusicFiles.Contains(extension) || ExtensionsOfReadablePlaylists.ContainsKey(extension)) && File.Exists(item);
                }
            }
        }
        protected bool IsSingleMusicFile(string file)
        {
            var fileLower = file.ToLower();
            return fileLower.EndsWith(".mp3")
                || fileLower.EndsWith(".wav")
                || fileLower.EndsWith(".wma");
        }
    }
}

