using System;
using System.Collections.Generic;

namespace GRYLibrary.Miscellaneous
{
    public class FileSelector
    {
        private FileSelector()
        {

        }
        public static FileSelector SingleFile(string file)
        {
            FileSelector result = new FileSelector();
            result.Files = new string[] { file };
            return result;
        }
        public static FileSelector FilesInFolder(string folder, bool deepSearch = true)
        {
            return FilesInFolder(folder, (string file) => true, deepSearch);
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
            result.Files = list;
            return result;
        }
        public IEnumerable<string> Files { get; private set; }
    }
}
