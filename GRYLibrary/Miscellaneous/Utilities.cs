using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GRYLibrary
{
    public static class Utilities
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static bool EqualsIgnoringOrder<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return Enumerable.SequenceEqual(list1.OrderBy(item => item), list2.OrderBy(item => item));
        }
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
            return string.Format("{{{0}}}", string.Join(", ", types.Select((type) => type.Name)));
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

        public static bool IsHexDigit(this char @char)
        {
            return (@char >= '0' && @char <= '9') || (@char >= 'a' && @char <= 'f') || (@char >= 'A' && @char <= 'F');
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
        public static void RemoveContentOfFolder(string folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                fileInfo.Delete();
            }
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
            {
                subDirectoryInfo.Delete(true);
            }
        }

        /// <summary>
        /// Starts all <see cref="ThreadStart"/>-objects in <paramref name="threadStarts"/> concurrent and return all results which did not throw an exception.
        /// Warning: This function is not implemented yet.
        /// </summary>
        /// <returns>The results of the first finished <paramref name="threadStarts"/>-methods.</returns>
        /// <exception cref="ArgumentException">If <paramref name="threadStarts"/> is empty.</exception>
        public static ISet<Tuple<ThreadStart, object>> RunAllConcurrentAndReturnAllResults(ISet<ThreadStart> threadStarts)
        {
            if (threadStarts.Count == 0)
            {
                throw new ArgumentException();
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts all <see cref="ThreadStart"/>-objects in <paramref name="threadStarts"/> concurrent and return the result of the first execution which does not throw an exception.
        /// Warning: This function is not implemented yet.
        /// </summary>
        /// <returns>The result of the first finished <paramref name="threadStarts"/>-method.</returns>
        /// <exception cref="ArgumentException">If <paramref name="threadStarts"/> is empty.</exception>
        /// <exception cref="Exception">If every <paramref name="threadStarts"/>-method throws an exception.</exception>
        public static object RunAllConcurrentAndReturnFirstResult(ISet<ThreadStart> threadStarts)
        {
            if (threadStarts.Count == 0)
            {
                throw new ArgumentException();
            }
            throw new NotImplementedException();
        }

        public static ISet<string> ToCaseInsensitiveSet(ISet<string> input)
        {
            ISet<TupleWithSpecialEquals> tupleList = new HashSet<TupleWithSpecialEquals>(input.Select((item) => new TupleWithSpecialEquals(item, item.ToLower())));
            return new HashSet<string>((tupleList.Select((item) => item.Item1)));
        }
        private class TupleWithSpecialEquals : Tuple<string, string>
        {
            public TupleWithSpecialEquals(string item1, string item2) : base(item1, item2)
            {
            }

            public override bool Equals(object @object)
            {
                return this.Item2.Equals(((TupleWithSpecialEquals)@object).Item2);
            }

            public override int GetHashCode()
            {
                return this.Item2.GetHashCode();
            }
        }
        #region IsList and IsDictionary
        //see https://stackoverflow.com/a/17190236/3905529
        public static bool IsList(this object @object)
        {
            if (@object == null)
            {
                return false;
            }
            else
            {
                return @object is IList &&
                       @object.GetType().IsGenericType &&
                       @object.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
            }
        }

        public static bool IsDictionary(this object @object)
        {
            if (@object == null)
            {
                return false;
            }
            else
            {
                return @object is IDictionary &&
                   @object.GetType().IsGenericType &&
                   @object.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
            }
        }
        #endregion
        /// <summary>
        /// Warning: This function is not implemented yet.
        /// </summary>
        public static bool IsSet(this object @object)
        {
            throw new NotImplementedException();
        }

        //see https://stackoverflow.com/a/129395/3905529
        public static T DeepClone<T>(this T @object)
        {
            using (Stream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, @object);
                memoryStream.Position = 0;
                return (T)formatter.Deserialize(memoryStream);
            }
        }
        public static ISet<T> ToSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
        public static void AddAll<T>(this ISet<T> set, IEnumerable<T> newItems)
        {
            set.UnionWith(newItems);
        }
        public static SimpleObjectPersistence<T> Persist<T>(this T @object, string file) where T : new()
        {
            return @object.Persist(file, Encoding.UTF8);
        }
        public static SimpleObjectPersistence<T> Persist<T>(this T @object, string file, Encoding encoding) where T : new()
        {
            return new SimpleObjectPersistence<T>(file, encoding, @object);
        }
    }
}