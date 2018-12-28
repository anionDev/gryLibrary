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

        protected override Tuple<IEnumerable<string>, IEnumerable<string>> GetSongsFromPlaylist(string playlistFile)
        {
            string directory = Path.GetDirectoryName(playlistFile);
            List<string> lines = File.ReadAllLines(playlistFile, Encoding).Select(line => line.Replace("\"", string.Empty).Trim()).Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))).ToList();
            List<string> result = new List<string>();
            List<string> excludedItems = new List<string>();

            foreach (string line in lines)
            {
                string payload;
                if (line.Contains("*"))
                {
                    payload = line.Split('*')[0];
                }
                else
                {
                    payload = line;
                }
                if (payload.StartsWith("-"))
                {
                    excludedItems.Add(payload.Substring(1));
                }
                else
                {
                    result.Add(payload);
                }
            }
            this.TryToApplyConfigurationFile(playlistFile, ref result);
            this.TryToApplyConfigurationFile(playlistFile, ref excludedItems);
            this.ResolvePaths(ref result, directory);
            this.ResolvePaths(ref excludedItems, directory);
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(result, excludedItems);
        }

        private void ResolvePaths(ref List<string> items, string directory)
        {
            List<string> result = new List<string>();
            foreach (string item in items)
            {
                result.Add(this.ConvertToAbsolutePathIfPossible(directory, item));
            }
            items = result;
        }

        private string ConvertToAbsolutePathIfPossible(string pathBase, string path)
        {
            if (Utilities.IsRelativePath(path))
            {
                return Utilities.GetAbsolutePath(pathBase, path);
            }
            else
            {
                return path;
            }
        }

        private bool TryToApplyConfigurationFile(string playlistFile, ref List<string> result)
        {
            //TODO refactor this
            try
            {
                string m3uConfigurationFile = new FileInfo(playlistFile).Directory.FullName + ConfigurationFileInCurrentFolder;
                bool configurationAppliedFound = this.SetResultAndApplayConfigurationFile(ref result, m3uConfigurationFile);
                if (configurationAppliedFound)
                {
                    return configurationAppliedFound;
                }
                else
                {
                    m3uConfigurationFile = new FileInfo(m3uConfigurationFile).Directory.Parent.FullName + ConfigurationFileInCurrentFolder;
                   configurationAppliedFound = this.SetResultAndApplayConfigurationFile(ref result, m3uConfigurationFile);
                    if (configurationAppliedFound)
                    {
                        return configurationAppliedFound;
                    }
                    else
                    {
                        m3uConfigurationFile = new FileInfo(m3uConfigurationFile).Directory.Parent.FullName + ConfigurationFileInCurrentFolder;
                        return this.SetResultAndApplayConfigurationFile(ref result, m3uConfigurationFile);
                    }
                }
            }
            catch
            {
                return false;
            }
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
            private string _EqualsDefinitionsFile = null;
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
                            if (optionKey.Equals("equals"))
                            {
                                this._EqualsDefinitionsFile = optionValue;
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
                this.AddEqualsDefinitionsToReplaceDictionary();
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

            private void AddEqualsDefinitionsToReplaceDictionary()
            {
                if (!(this._EqualsDefinitionsFile == null) && File.Exists(this._EqualsDefinitionsFile))
                {
                    string[] equalsDefinitions = File.ReadAllLines(this._EqualsDefinitionsFile);
                    foreach (string equalsDefinition in equalsDefinitions)
                    {
                        if (equalsDefinition.Contains("*") && !equalsDefinition.StartsWith("#"))
                        {
                            string[] splitted = equalsDefinition.Split('*');
                            foreach (string value in splitted.Skip(1))
                            {
                                this._Replace.Add(splitted[0], value);
                            }
                        }
                    }
                }
            }
        }
    }
}
