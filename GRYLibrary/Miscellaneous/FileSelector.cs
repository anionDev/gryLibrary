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
        public static FileSelector FilesInFolder(string folder, Func<string, bool> filter, bool deepSearch = true)
        {
            FileSelector result = new FileSelector();
            List<string> list = new List<string>();
            if (deepSearch)
            {
                Utilities.ForEachFileAndDirectoryTransitively(folder, null, (string file, object argument) => { if (filter(file)) { list.Add(file); } }, false, null, null);
            }
            else
            {
                foreach (string file in System.IO.Directory.GetFiles(folder))
                {
                    if (filter(file))
                    {
                        list.Add(file);
                    }
                }
            }
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
