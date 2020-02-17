using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace GRYLibrary.Core.Playlists.ConcretePlaylistHandler
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
                try
                {
                    result.Add(this.ConvertToAbsolutePathIfPossible(directory, item));
                }
                catch
                {
                    Utilities.NoOperation();
                }
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

        internal class M3UConfiguration
        {
            internal class M3UConfigurationPerPC
            {
                public Dictionary<string, string> Replace { get; } = new Dictionary<string, string>();
                public string EqualsDefinitionsFile { get; set; }
                public string PCName { get; }
                public M3UConfigurationPerPC(string pcName)
                {
                    this.PCName = pcName;
                }

            }
            internal IList<M3UConfigurationPerPC> ConfigurationItems { get; } = new List<M3UConfigurationPerPC>();
            private readonly string _ConfigurationFile;
            public M3UConfiguration(string configurationFile)
            {
                this._ConfigurationFile = configurationFile;
                this.ReadConfigFile();
            }

            private void ReadConfigFile()
            {
                M3UConfigurationPerPC currentConfiguration = null;
                foreach (string line in File.ReadAllLines(this._ConfigurationFile))
                {
                    try
                    {
                        string trimmedLine = line.Trim();
                        if ((!trimmedLine.StartsWith("#")) && (!string.IsNullOrWhiteSpace(trimmedLine)))
                        {
                            if (trimmedLine.Contains(":"))
                            {
                                string optionKey = trimmedLine.Split(':')[0].ToLower();
                                string optionValue = trimmedLine.Substring(optionKey.Length + 1);
                                if (optionKey.Equals("on"))
                                {
                                    currentConfiguration = new M3UConfigurationPerPC(trimmedLine.Split(':')[1].ToUpper());
                                    this.ConfigurationItems.Add(currentConfiguration);
                                }
                                if (optionKey.Equals("replace"))
                                {
                                    string[] splitted = optionValue.Split(';');
                                    currentConfiguration.Replace.Add(splitted[0], splitted[1]);
                                }
                                if (optionKey.Equals("equals"))
                                {
                                    currentConfiguration.EqualsDefinitionsFile = optionValue;
                                }
                                //add other options if desired
                            }
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
                M3UConfigurationPerPC configuration = this.GetApplicableConfiguration();
                List<string> result = new List<string>();
                this.AddEqualsDefinitionsToReplaceDictionary(configuration);
                foreach (string item in input)
                {
                    string newItem = item;
                    foreach (System.Collections.Generic.KeyValuePair<string, string> replacement in configuration.Replace)
                    {
                        newItem = newItem.Replace(replacement.Key, replacement.Value);
                    }
                    //process other options if available
                    result.Add(newItem);
                }
                return result;
            }

            private M3UConfigurationPerPC GetApplicableConfiguration()
            {
                foreach (M3UConfigurationPerPC item in this.ConfigurationItems)
                {
                    if (item.PCName.ToUpper().Equals(Environment.MachineName.ToUpper()))
                    {
                        return item;
                    }
                }
                foreach (M3UConfigurationPerPC item in this.ConfigurationItems)
                {
                    if (item.PCName.ToUpper().Equals("all".ToUpper()))
                    {
                        return item;
                    }
                }
                throw new KeyNotFoundException();
            }

            private void AddEqualsDefinitionsToReplaceDictionary(M3UConfigurationPerPC configuration)
            {
                if (!(configuration.EqualsDefinitionsFile == null) && File.Exists(configuration.EqualsDefinitionsFile))
                {
                    string[] equalsDefinitions = File.ReadAllLines(configuration.EqualsDefinitionsFile);
                    foreach (string equalsDefinition in equalsDefinitions)
                    {
                        if (equalsDefinition.Contains("*") && !equalsDefinition.StartsWith("#"))
                        {
                            string[] splitted = equalsDefinition.Split('*');
                            foreach (string value in splitted.Skip(1))
                            {
                                configuration.Replace.Add(splitted[0], value);
                            }
                        }
                    }
                }
            }
        }
    }
}
