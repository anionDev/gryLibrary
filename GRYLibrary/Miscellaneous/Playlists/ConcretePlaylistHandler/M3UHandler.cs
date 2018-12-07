using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
namespace GRYLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : AbstractPlaylistHandler
    {
        public static M3UHandler Instance { get; } = new M3UHandler();
        private M3UHandler() { }
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            if (!Utilities.FileIsEmpty(playlistFile) && !Utilities.FileEndsWithEmptyLine(playlistFile))
            {
                File.AppendAllText(playlistFile, Environment.NewLine, Encoding);
            }
            File.AppendAllLines(playlistFile, newSongs, Encoding);
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            List<string> files = new List<string>();
            foreach (string item in File.ReadAllLines(playlistFile, Encoding))
            {
                if (!songsToDelete.Contains(item))
                {
                    files.Add(item);
                }
            }
            File.WriteAllLines(playlistFile, files, Encoding);
        }

        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            List<string> lines = File.ReadAllLines(playlistFile, Encoding).Select(line => line.Replace("\"", string.Empty).Trim()).Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))).ToList();
            List<string> result = new List<string>();
            foreach (string line in lines)
            {
                if (line.Contains("*"))
                {
                    result.Add(line.Split('*')[0]);
                }
                else
                {
                    result.Add(line);
                }
            }
            string m3uConfigurationFile = new FileInfo(playlistFile).Directory.FullName + ConfigurationFileInCurrentFolder;
            if (!this.SetResultAndApplayConfigurationFile(ref result, m3uConfigurationFile))
            {
                m3uConfigurationFile = new FileInfo(m3uConfigurationFile).Directory.Parent.FullName + ConfigurationFileInCurrentFolder;
                this.SetResultAndApplayConfigurationFile(ref result, m3uConfigurationFile);
            }
            return result;
        }
        private const string ConfigurationFileName = ".M3UConfiguration";
        public const string ConfigurationFileInCurrentFolder = "\\" + ConfigurationFileName;
        private bool SetResultAndApplayConfigurationFile(ref List<string> result, string m3uConfigurationFile)
        {
            if (File.Exists(m3uConfigurationFile))
            {
                M3UConfiguration configuration = new M3UConfiguration(m3uConfigurationFile);
                result = configuration.ApplyTo(result).ToList();
                return true;
            }
            return false;
        }

        public override void CreatePlaylist(string file)
        {
            Utilities.EnsureFileExists(file);
        }

        private class M3UConfiguration
        {
            private readonly Dictionary<string, string> _Replace = new Dictionary<string, string>();
            private readonly string _ConfigurationFile;
            public M3UConfiguration(string configurationFile)
            {
                this._ConfigurationFile = configurationFile;
                this.ReadConfigFile();
            }

            private void ReadConfigFile()
            {
                foreach (string line in File.ReadAllLines(this._ConfigurationFile))
                {
                    try
                    {
                        if (line.Contains(":"))
                        {
                            string optionKey = line.Split(':')[0].ToLower();
                            string optionValue = line.Substring(optionKey.Length + 1);
                            if (optionKey.Equals("replace"))
                            {
                                string[] splitted = optionValue.Split(';');
                                this._Replace.Add(splitted[0], splitted[1]);
                            }
                            //add other options if desired
                        }
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                }
            }

            internal IEnumerable<string> ApplyTo(IEnumerable<string> input)
            {
                List<string> result = new List<string>();
                foreach (string item in input)
                {
                    string newItem = item;
                    foreach (KeyValuePair<string, string> replacement in this._Replace)
                    {
                        newItem = newItem.Replace(replacement.Key, replacement.Value);
                    }
                    //process other options if available
                    result.Add(newItem);
                }
                return result;
            }
        }
    }
}
