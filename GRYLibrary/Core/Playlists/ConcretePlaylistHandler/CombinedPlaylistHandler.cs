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
            DefaultMusicFolder = defaultMusicFolder;
        }
        public IList<(string/*alias*/, string/*folder*/)> DefaultMusicFolder { get; }
        private Dictionary<string, IPlaylistFileHandler> _PlaylistHandler = null;
        private Dictionary<string, IPlaylistFileHandler> ConcretePlaylistHandler
        {
            get
            {
                if (_PlaylistHandler == null)
                {
                    _PlaylistHandler = new Dictionary<string, IPlaylistFileHandler>
                    {
                        { ".m3u",new M3UHandler(){ Encoding = this.Encoding} },
                        { ".pls",new PLSHandler(){ Encoding = this.Encoding} },
                    };
                }
                return _PlaylistHandler;
            }
        }

        public ISet<string> MusicFileExtensions = new HashSet<string>() { ".mp3", ".wma" };
        public ISet<string> PlaylistFileExtensions = new HashSet<string>() { ".m3u", ".pls" };
        public IList<string> AllowedFiletypesForMusicFiles { get; } = new List<string>();
        public IEnumerable<string> GetSongs(string playlistFile)
        {
            return GetSongs(playlistFile, true, false);
        }
        public IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            return GetSongs(musicFileOrPlaylistFileOrFolder, Directory.GetCurrentDirectory(), removeDuplicatedItems, addOnlyExistingMusicFiles);
        }
        private IEnumerable<string> GetSongs(string musicFileOrPlaylistFileOrFolder, string workingDirectory, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            musicFileOrPlaylistFileOrFolder = NormalizeItem(musicFileOrPlaylistFileOrFolder);
            if (Utilities.IsRelativePath(musicFileOrPlaylistFileOrFolder))
            {
                musicFileOrPlaylistFileOrFolder = Utilities.ResolveToFullPath(musicFileOrPlaylistFileOrFolder, workingDirectory);
            }
            List<string> result = new List<string>();

            if (IsMusicFile(musicFileOrPlaylistFileOrFolder))
            {
                if (File.Exists(musicFileOrPlaylistFileOrFolder) || !addOnlyExistingMusicFiles)
                {
                    result.Add(musicFileOrPlaylistFileOrFolder);
                }
            }
            if (IsPlaylistFile(musicFileOrPlaylistFileOrFolder))
            {
                if (File.Exists(musicFileOrPlaylistFileOrFolder))
                {
                    result.AddRange(GetSongsFromPlaylist(musicFileOrPlaylistFileOrFolder, removeDuplicatedItems, addOnlyExistingMusicFiles));
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

            GetConcretePlaylistHandler(playlistFile).NormalizePlaylist(playlistFile);
        }

        private string ReplaceDefaultMusicFolder(string line)
        {
            foreach ((string, string) defaultFolder in DefaultMusicFolder)
            {
                line = line.Replace(defaultFolder.Item1, defaultFolder.Item2);
            }
            return line;
        }
        private IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems, bool addOnlyExistingMusicFiles)
        {
            Tuple<IEnumerable<string>, IEnumerable<string>> songsAndExcludedSongs = GetSongsAndExcludedSongs(playlistFile);
            IEnumerable<string> referencedFiles = songsAndExcludedSongs.Item1;
            IEnumerable<string> referencedExcludedFiles = songsAndExcludedSongs.Item2;
            string directory = Path.GetDirectoryName(playlistFile);
            referencedFiles = referencedFiles.SelectMany(item => GetSongs(item, directory, removeDuplicatedItems, addOnlyExistingMusicFiles)).ToList();
            referencedExcludedFiles = referencedExcludedFiles.SelectMany(item => GetSongs(item, directory, removeDuplicatedItems, addOnlyExistingMusicFiles)).ToList();
            return referencedFiles.Except(referencedExcludedFiles);
        }

        public void CreatePlaylist(string file)
        {
            ConcretePlaylistHandler[".m3u"].CreatePlaylist(file);
        }

        public IPlaylistFileHandler GetConcretePlaylistHandler(string file)
        {
            return ConcretePlaylistHandler[Path.GetExtension(file.ToLower())];
        }
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            GetConcretePlaylistHandler(playlistFile).AddSongsToPlaylist(playlistFile, newSongs);
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            GetConcretePlaylistHandler(playlistFile).DeleteSongsFromPlaylist(playlistFile, songsToDelete);
        }
        public bool IsMusicFile(string file)
        {
            return MusicFileExtensions.Contains(Path.GetExtension(file.ToLower()));
        }
        public bool IsPlaylistFile(string file)
        {
            return PlaylistFileExtensions.Contains(Path.GetExtension(file.ToLower()));
        }

        public Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsAndExcludedSongs(string playlistFile)
        {
            return GetConcretePlaylistHandler(playlistFile).GetSongsAndExcludedSongs(playlistFile);
        }

        void IPlaylistFileHandler.NormalizePlaylist(string playlistFile)
        {
            GetConcretePlaylistHandler(playlistFile).NormalizePlaylist(playlistFile);
        }

    }
}

