using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Miscellaneous
{
    public class FileSelector
    {
        private IEnumerable<string> _Files;

        private FileSelector()
        {

        }
        public static FileSelector SingleFile(string file)
        {
            FileSelector result = new FileSelector();
            result._Files = new string[] { file };
            return result;
        }
        public static FileSelector FilesInFolder(string folder)
        {
            return FilesInFolder(folder, (string file) => true);
        }
        public static FileSelector FilesInFolder(string folder, Func<string, bool> filter)
        {
            FileSelector result = new FileSelector();
            List<string> list = new List<string>();
            Utilities.ForEachFileAndDirectoryTransitively(folder, null, (string file, object argument) => { if (filter(file)) { list.Add(file); } }, false, null, null);
            result._Files = list;
            return result;
        }
        public IEnumerable<string> Files
        {
            get
            {
                return this._Files;
            }
        }
    }
}
