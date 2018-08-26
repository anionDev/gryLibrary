using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
namespace GRLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : AbstractPlaylistHandler
    {
        public static M3UHandler Instance { get; } = new M3UHandler();
        private M3UHandler() { }
        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
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
            File.WriteAllLines(playlistFile, songsToDelete, Encoding);
        }

        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            List<string> items = File.ReadAllLines(playlistFile, Encoding).Select(line => line.Replace("\"", string.Empty).Trim()).Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))).ToList();
            List<string> result = new List<string>();
            foreach (string item in items)
            {
                if (item.Contains("*"))
                {
                    result.Add(item.Split('*')[0]);
                }
                else
                {
                    result.Add(item);
                }
            }
            string m3uConfigurationFile = new FileInfo(playlistFile).Directory.FullName + "\\.M3UConfiguration";
            if (File.Exists(m3uConfigurationFile))
            {
                M3UConfiguration configuration = new M3UConfiguration(m3uConfigurationFile);
                result = configuration.ApplyTo(result).ToList();
            }
            return result;
        }
        private class M3UConfiguration
        {
            private readonly Dictionary<string, string> _Replace = new Dictionary<string, string>();
            private readonly string _ConfigurationFile;
            public M3UConfiguration(string configurationFile)
            {
                this._ConfigurationFile = configurationFile;
                ReadConfigFile();
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
