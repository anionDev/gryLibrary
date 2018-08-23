using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
namespace GRLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler
{
    public class M3UHandler : AbstractPlaylistHandler
    {

        protected override void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs)
        {
            File.AppendAllLines(playlistFile, newSongs, this.Encoding);
        }

        protected override void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete)
        {
            List<string> files = new List<string>();
            foreach (string item in File.ReadAllLines(playlistFile, this.Encoding))
            {
                if (!songsToDelete.Contains(item))
                {
                    files.Add(item);
                }
            }
            File.WriteAllLines(playlistFile, songsToDelete, this.Encoding);
        }

        protected override IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile)
        {
            List<string> items = File.ReadAllLines(playlistFile, this.Encoding).Select(line => line.Replace("\"", string.Empty).Trim()).Where(line => !(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))).ToList();
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

            return result;
        }
    }
}
