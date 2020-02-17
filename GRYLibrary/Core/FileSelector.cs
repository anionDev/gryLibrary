using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core
{
    public class FileSelector
    {
        public IEnumerable<string> Files { get; private set; }
        private FileSelector()
        {
        }
        public static FileSelector SingleFile(string file)
        {
            return new FileSelector
            {
                Files = new string[] { file }
            };
        }
        public static FileSelector FileList(IEnumerable<string> files)
        {
            return new FileSelector
            {
                Files = files.Select((file) => Normalize(file))
            };
        }

        private static string Normalize(string file)
        {
            return file.Trim().ToLower();
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
                        list.Add(Normalize(file));
                    }
                }
            }
            result.Files = list;
            return result;
        }
    }
}
