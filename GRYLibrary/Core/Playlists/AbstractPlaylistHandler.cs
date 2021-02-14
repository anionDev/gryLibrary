using GRYLibrary.Core.Playlists.ConcretePlaylistHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Playlists
{
    public abstract class AbstractPlaylistHandler
    {

        public AbstractPlaylistHandler() : this(new List<(string, string)>())
        {
        }
        public AbstractPlaylistHandler(IList<(string/*alias*/, string/*folder*/)> defaultMusicFolder)
        {
            DefaultMusicFolder = defaultMusicFolder;
        }
        public IList<(string/*alias*/, string/*folder*/)> DefaultMusicFolder { get; }
        private static Dictionary<string, AbstractPlaylistHandler> _ExtensionsOfReadablePlaylists = null;
        public static Dictionary<string, AbstractPlaylistHandler> ExtensionsOfReadablePlaylists
        {
            get
            {
                if (_ExtensionsOfReadablePlaylists == null)
                {
                    _ExtensionsOfReadablePlaylists = new Dictionary<string, AbstractPlaylistHandler>
                    {
                        { ".m3u", M3UHandler.Instance },
                        { ".pls", PLSHandler.Instance }
                    };
                }
                return _ExtensionsOfReadablePlaylists;
            }
        }
        public static Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public IList<string> AllowedFiletypesForMusicFiles { get; } = new List<string>();
        public abstract void CreatePlaylist(string file);
        protected abstract Tuple<IEnumerable<string>/*included files*/, IEnumerable<string>/*excluded files*/> GetSongsFromPlaylistImplementation(string playlistFile);
        protected abstract void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs);
        protected abstract void NormalizePlaylistImplementation(string playlistFile);
        protected abstract void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete);
        public IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, bool removeDuplicatedItems = true)
        {
            return GetSongs(musicFileOrPlaylistFileOrFolder, Directory.GetCurrentDirectory(), removeDuplicatedItems);
        }
        private IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, string workingDirectory, bool removeDuplicatedItems = true)
        {
            musicFileOrPlaylistFileOrFolder = Utilities.ResolveToFullPath(NormalizeItem(musicFileOrPlaylistFileOrFolder), workingDirectory).Trim();
            List<string> result = new List<string>();
            if (File.Exists(musicFileOrPlaylistFileOrFolder))
            {
                if (IsMusicFile(musicFileOrPlaylistFileOrFolder))
                {
                    result.Add(musicFileOrPlaylistFileOrFolder);
                }
                if (IsPlaylistFile(musicFileOrPlaylistFileOrFolder))
                {
                    result.AddRange(GetSongsFromPlaylist(musicFileOrPlaylistFileOrFolder, removeDuplicatedItems));
                }
            }
            if (Directory.Exists(musicFileOrPlaylistFileOrFolder))
            {
                IEnumerable<string> allFiles = Utilities.GetFilesOfFolderRecursively(musicFileOrPlaylistFileOrFolder);
                foreach (string musicFile in allFiles.Where(file => IsMusicFile(file) || IsPlaylistFile(file)))
                {
                    result.AddRange(GetSongs(musicFile));
                }
            }
            if (removeDuplicatedItems)
            {
                return result.Distinct();
            }
            else
            {
                return result;
            }
        }

        private string NormalizeItem(string item)
        {
            return ReplaceDefaultMusicFolder(item.Replace("\"", string.Empty)).Trim();
        }

        protected void NormalizePlaylist(string playlistFile)
        {
            NormalizePlaylistImplementation(playlistFile);
        }

        private string ReplaceDefaultMusicFolder(string line)
        {
            foreach ((string, string) defaultFolder in DefaultMusicFolder)
            {
                line = line.Replace(defaultFolder.Item1, defaultFolder.Item2);
            }
            return line;
        }
        private IEnumerable<string> GetSongsFromPlaylist(string musicFileOrPlaylistFile, bool removeDuplicatedItems)
        {
            Tuple<IEnumerable<string>, IEnumerable<string>> songsAndExcludedSongs = this.GetSongsFromPlaylistImplementation(musicFileOrPlaylistFile);
            IEnumerable<string> referencedFiles = songsAndExcludedSongs.Item1;
            IEnumerable<string> referencedExcludedFiles = songsAndExcludedSongs.Item2;
            string directory = Path.GetDirectoryName(musicFileOrPlaylistFile);
            referencedFiles = referencedFiles.SelectMany(item => GetSongs(musicFileOrPlaylistFile, directory, removeDuplicatedItems)).ToList();
            referencedExcludedFiles = referencedExcludedFiles.SelectMany(item => GetSongs(musicFileOrPlaylistFile, directory, removeDuplicatedItems)).ToList();
            return referencedFiles.Except(referencedExcludedFiles);
        }

        public static AbstractPlaylistHandler GetConcretePlaylistHandler(string file)
        {
            return ExtensionsOfReadablePlaylists[Path.GetExtension(file.ToLower())];
        }
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            this.AddSongsToPlaylistImplementation(playlistFile, newSongs);
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            this.DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete);
        }
        protected bool IsMusicFile(string file)
        {
            var extension = Path.GetExtension(file);
            return extension == ".mp3"
                || extension == ".wav"
                || extension == ".wma";
        }
        protected bool IsPlaylistFile(string file)
        {
            string extension = Path.GetExtension(file);
            return extension == ".m3u"
                || extension == ".pls";
        }
    }
}

