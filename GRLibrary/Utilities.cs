using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GRLibrary
{
    public static class Utilities
    {

        public static IEnumerable<string> GetFilesOfFolderRecursively(string folder)
        {
            List<string> result = new List<string>();
            foreach (string subfolder in Directory.GetDirectories(folder))
            {
                result.AddRange(GetFilesOfFolderRecursively(subfolder));
            }
            foreach (string file in Directory.GetFiles(folder))
            {
                result.Add(file);
            }
            return result;
        }

        public static void NoOperation()
        {
            //nothing to do
        }
        public static Icon BitmapToIcon(Bitmap bitmap)
        {
            bitmap.MakeTransparent(Color.White);
            IntPtr intPtr = bitmap.GetHicon();
            return Icon.FromHandle(intPtr);
        }
        public static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
        public static string TypeArrayToString(Type[] types)
        {
            return "{" + string.Join(", ", types.Select((type) => type.Name)) + "}";
        }
        public static string GetCommandLineArgumentWithoutProgramPath()
        {
            return Environment.CommandLine.Substring(Environment.GetCommandLineArgs()[0].Length + 3);
        }
        public static void CopyFolderAcrossVolumes(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string destination = Path.Combine(destinationFolder, name);
                File.Copy(file, destination);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string destination = Path.Combine(destinationFolder, name);
                CopyFolderAcrossVolumes(folder, destination);
            }
        }
        public static void MoveFolderAcrossVolumes(string sourceFolder, string destinationFolder)
        {
            CopyFolderAcrossVolumes(sourceFolder, destinationFolder);
            Directory.Delete(sourceFolder, true);
        }
        public static void ForEachFileAndDirectoryTransitively(string directory, Action<string, object> directoryAction, Action<string, object> fileAction, bool ignoreErrors = false, object argumentForFileAction = null, object argumentForDirectoryAction = null)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                try
                {
                    fileAction?.Invoke(file, argumentForFileAction);
                }
                catch
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }
                }
            }
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ForEachFileAndDirectoryTransitively(subDirectory, directoryAction, fileAction, ignoreErrors, argumentForFileAction, argumentForDirectoryAction);
                try
                {
                    directoryAction?.Invoke(subDirectory, argumentForDirectoryAction);
                }
                catch
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }
                }
            }
        }

        public static DateTime GetDateTakenFromImage(string file)
        {
            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (Image image = Image.FromStream(fileStream, false, false))
            {
                PropertyItem propItem = image.GetPropertyItem(36867);
                string dateTaken = new Regex(":").Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        public static void CreateFileIfNotExist(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }
        }
    }
}