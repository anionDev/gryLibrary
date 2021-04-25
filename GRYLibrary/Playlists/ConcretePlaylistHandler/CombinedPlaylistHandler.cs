using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
{
    public class CombinedPlaylistHandler : IPlaylistFileHandler
    {
        public static IPlaylistFileHandler DefaultInstance { get; } = new CombinedPlaylistHandler();
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public CombinedPlaylistHandler() : this(new List<(string, string)>())
        {
        }
        public CombinedPlaylistHandler(IList<(string/*alias*/, string/*folder*/)> defaultMusicFolder)
        {
            this.DefaultMusicFolder = defaultMusicFolder;
        }
        public IList<(string/*alias*/, string/*folder*/)> DefaultMusicFolder { get; }
        private Dictionary<string, IPlaylistFileHandler> _PlaylistHandler = null;
        private Dictionary<string, IPlaylistFileHandler> ConcretePlaylistHandler
        {
            get
            {
                if (this._PlaylistHandler == null)
                {
                    this._PlaylistHandler = new Dictionary<string, IPlaylistFileHandler>
                    {
                        { ".m3u",new M3UHandler(){ Encoding = Encoding} },
                        { ".pls",new PLSHandler(){ Encoding = Encoding} },
                    };
                }
                return this._PlaylistHandler;
            }
        }

        public ISet<string> MusicFileExtensions = new HashSet<string>() { ".mp3", ".wma" };
        public ISet<string> PlaylistFileExtensions = new HashSet<string>() { ".m3u", ".pls" };
        public IList<string> AllowedFiletypesForMusicFiles { get; } = new List<string>();
        public IEnumerable<string> GetSongs(string playlistFile)
        {
            return this.GetSongs(playlistFile, true, false);
        }
        public IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            return this.GetSongs(musicFileOrPlaylistFileOrFolder, Directory.GetCurrentDirectory(), removeDuplicatedItems, addOnlyExistingMusicFiles);
        }
        private IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, string workingDirectory, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            musicFileOrPlaylistFileOrFolder = this.NormalizeItem(musicFileOrPlaylistFileOrFolder);
            if (Utilities.IsRelativePath(musicFileOrPlaylistFileOrFolder))
            {
                musicFileOrPlaylistFileOrFolder = Utilities.ResolveToFullPath(musicFileOrPlaylistFileOrFolder, workingDirectory);
            }
            List<string> result = new();

            if (this.IsMusicFile(musicFileOrPlaylistFileOrFolder))
            {
                if (File.Exists(musicFileOrPlaylistFileOrFolder) || !addOnlyExistingMusicFiles)
                {
                    result.Add(musicFileOrPlaylistFileOrFolder);
                }
            }
            if (this.IsPlaylistFile(musicFileOrPlaylistFileOrFolder))
            {
                if (File.Exists(musicFileOrPlaylistFileOrFolder))
                {
                    result.AddRange(this.GetSongsFromPlaylist(musicFileOrPlaylistFileOrFolder, removeDuplicatedItems, addOnlyExistingMusicFiles));
                }
            }

            if (Directory.Exists(musicFileOrPlaylistFileOrFolder))
            {
                IEnumerable<string> allFiles = Utilities.GetFilesOfFolderRecursively(musicFileOrPlaylistFileOrFolder);
                foreach (string musicFile in allFiles.Where(file => this.IsMusicFile(file) || this.IsPlaylistFile(file)))
                {
                    result.AddRange(this.GetSongs(musicFile));
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
            return this.ReplaceDefaultMusicFolder(item.Replace("\"", string.Empty)).Trim();
        }

        protected void NormalizePlaylist(string playlistFile)
        {

            this.GetConcretePlaylistHandler(playlistFile).NormalizePlaylist(playlistFile);
        }

        private string ReplaceDefaultMusicFolder(string line)
        {
            foreach ((string, string) defaultFolder in this.DefaultMusicFolder)
            {
                line = line.Replace(defaultFolder.Item1, defaultFolder.Item2);
            }
            return line;
        }
        private IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            Tuple<IEnumerable<string>, IEnumerable<string>> songsAndExcludedSongs = this.GetSongsAndExcludedSongs(playlistFile);
            IEnumerable<string> referencedFiles = songsAndExcludedSongs.Item1;
            IEnumerable<string> referencedExcludedFiles = songsAndExcludedSongs.Item2;
            string directory = Path.GetDirectoryName(playlistFile);
            referencedFiles = referencedFiles.SelectMany(item => this.GetSongs(item, directory, removeDuplicatedItems, addOnlyExistingMusicFiles)).ToList();
            referencedExcludedFiles = referencedExcludedFiles.SelectMany(item => this.GetSongs(item, directory, removeDuplicatedItems, addOnlyExistingMusicFiles)).ToList();
            return referencedFiles.Except(referencedExcludedFiles);
        }

        public void CreatePlaylist(string file)
        {
            this.ConcretePlaylistHandler[".m3u"].CreatePlaylist(file);
        }

        public IPlaylistFileHandler GetConcretePlaylistHandler(string file)
        {
            return this.ConcretePlaylistHandler[Path.GetExtension(file.ToLower())];
        }
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            this.GetConcretePlaylistHandler(playlistFile).AddSongsToPlaylist(playlistFile, newSongs);
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            this.GetConcretePlaylistHandler(playlistFile).DeleteSongsFromPlaylist(playlistFile, songsToDelete);
        }
        public bool IsMusicFile(string file)
        {
            return this.MusicFileExtensions.Contains(Path.GetExtension(file.ToLower()));
        }
        public bool IsPlaylistFile(string file)
        {
            return this.PlaylistFileExtensions.Contains(Path.GetExtension(file.ToLower()));
        }

        public Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsAndExcludedSongs(string playlistFile)
        {
            return this.GetConcretePlaylistHandler(playlistFile).GetSongsAndExcludedSongs(playlistFile);
        }

        void IPlaylistFileHandler.NormalizePlaylist(string playlistFile)
        {
            this.GetConcretePlaylistHandler(playlistFile).NormalizePlaylist(playlistFile);
        }

    }
}

