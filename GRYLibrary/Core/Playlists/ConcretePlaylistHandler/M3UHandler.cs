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
            if (!File.Exists(playlistFile))
            {
                throw new FileNotFoundException(playlistFile);
            }
            string directoryOfPlaylistfile = new DirectoryInfo(playlistFile).Parent.FullName;
            List<string> lines = File.ReadAllLines(playlistFile, Encoding).Select(line => line.Replace("\"", string.Empty).Trim()).Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))).ToList();
            string directory = Path.GetDirectoryName(playlistFile);
            List<string> includedItems = new List<string>();
            List<string> excludedItems = new List<string>();

            foreach (string line in lines)
            {
                if (line.StartsWith("-"))
                {
                    excludedItems.Add(line[1..]);
                }
                else
                {
                    includedItems.Add(line);
                }
            }
            this.TryToApplyConfigurationFile(playlistFile, ref includedItems);
            this.TryToApplyConfigurationFile(playlistFile, ref excludedItems);
            // problem here: content like "./referencedfolder" will get resolved to "C:\...\GRYLibrary\GRYLibraryTests\bin\Debug\net5.0\./referencedfolder"
            this.ResolvePaths(ref includedItems, directory);
            this.ResolvePaths(ref excludedItems, directory);
            return new Tuple<IEnumerable<string>, IEnumerable<string>>(includedItems, excludedItems);
        }
        /**
         * This function returns <param name="items"/> but with file-paths resolved relative to directory.
         */
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

        private string ConvertToAbsolutePathIfPossible(string pathBase, string file)
        {
            if (Utilities.IsRelativePath(file))
            {
                return Utilities.GetAbsolutePath(pathBase, file);
            }
            else
            {
                return file;
            }
        }

        private bool TryToApplyConfigurationFile(string playlistFile, ref List<string> listOfItems)
        {
            try
            {
                string m3uConfigurationFile = new FileInfo(playlistFile).Directory.FullName + ConfigurationFileInCurrentFolder;
                bool configurationAppliedFound = this.SetResultAndApplayConfigurationFile(ref listOfItems, m3uConfigurationFile);
                if (configurationAppliedFound)
                {
                    return configurationAppliedFound;
                }
                else
                {
                    m3uConfigurationFile = new FileInfo(m3uConfigurationFile).Directory.Parent.FullName + ConfigurationFileInCurrentFolder;
                    configurationAppliedFound = this.SetResultAndApplayConfigurationFile(ref listOfItems, m3uConfigurationFile);
                    if (configurationAppliedFound)
                    {
                        return configurationAppliedFound;
                    }
                    else
                    {
                        m3uConfigurationFile = new FileInfo(m3uConfigurationFile).Directory.Parent.FullName + ConfigurationFileInCurrentFolder;
                        return this.SetResultAndApplayConfigurationFile(ref listOfItems, m3uConfigurationFile);
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
                                string optionValue = trimmedLine[(optionKey.Length + 1)..];
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
                    foreach (KeyValuePair<string, string> replacement in configuration.Replace)
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
