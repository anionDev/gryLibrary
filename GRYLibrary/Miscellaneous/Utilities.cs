using System;
using GRYLibrary.Miscellaneous;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.DirectoryServices;

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
        public static Icon ToIcon(this Bitmap bitmap)
        {
            bitmap.MakeTransparent(Color.White);
            IntPtr intPtr = bitmap.GetHicon();
            return Icon.FromHandle(intPtr);
        }

        public static void WriteToConsoleAsASCIITable(IList<IList<string>> columns)
        {
            TableGenerator tableGenerator = new TableGenerator
            {
                Encoding = new UTF8Encoding(false)
            };
            string[] table = tableGenerator.Generate(TableGenerator.GetTableContentFromColumnList(columns), "Encoding-table", true, false);
            foreach (string line in table)
            {
                Console.WriteLine(line);
            }

        }

        public static void EnsureFileExists(string path, bool createDirectoryIfRequired = false)
        {
            if (createDirectoryIfRequired)
            {
                EnsureDirectoryExists(Path.GetDirectoryName(path));
            }
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void EnsureDirectoryDoesNotExist(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static void EnsureFileDoesNotExist(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static string TypeArrayToString(Type[] types)
        {
            return string.Format("{{{0}}}", string.Join(", ", types.Select((type) => type.Name)));
        }
        public static string GetCommandLineArgumentWithoutProgramPath()
        {
            string executableFile = Environment.GetCommandLineArgs()[0];
            return Environment.CommandLine.Remove(Environment.CommandLine.IndexOf(executableFile), executableFile.Length).TrimStart('"').Substring(1).Trim();
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

        public static void DeleteAllEmptyFolderTransitively(string folder, bool deleteFolderItselfIfAlsoEmpty = false)
        {
            ForEachFileAndDirectoryTransitively(folder, DeleteAllEmptyFolderTransitivelyDirectoryAction, (string file, object argument) => { }, false, null, null);
            if (deleteFolderItselfIfAlsoEmpty && DirectoryIsEmpty(folder))
            {
                Directory.Delete(folder);
            }
        }

        private static void DeleteAllEmptyFolderTransitivelyDirectoryAction(string directory, object argument)
        {
            if (DirectoryIsEmpty(directory))
            {
                Directory.Delete(directory);
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
                string dateTaken = new Regex(":").Replace(new UTF8Encoding(false).GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
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
        /// Starts all <see cref="Func{object}"/>-objects in <paramref name="functions"/> concurrent and return all results which did not throw an exception.
        /// </summary>
        /// <returns>The results of all finished <paramref name="functions"/>-methods with their results.</returns>
        public static ISet<Tuple<Func<T>, T, Exception>> RunAllConcurrentAndReturnAllResults<T>(this ISet<Func<T>> functions, int maximalDegreeOfParallelism = 4)
        {
            ConcurrentBag<Tuple<Func<T>, T, Exception>> result = new ConcurrentBag<Tuple<Func<T>, T, Exception>>();
            Parallel.ForEach(functions, new ParallelOptions { MaxDegreeOfParallelism = maximalDegreeOfParallelism }, (function) =>
            {
                try
                {
                    result.Add(new Tuple<Func<T>, T, Exception>(function, function(), null));
                }
                catch (Exception exception)
                {
                    result.Add(new Tuple<Func<T>, T, Exception>(function, default, exception));
                }
            });
            return new HashSet<Tuple<Func<T>, T, Exception>>(result);
        }

        /// <summary>
        /// Starts all <see cref="ThreadStart"/>-objects in <paramref name="functions"/> concurrent and return the result of the first execution which does not throw an exception.
        /// Warning: This function is not implemented yet.
        /// </summary>
        /// <returns>The result of the first finished <paramref name="functions"/>-method.</returns>
        /// <exception cref="ArgumentException">If <paramref name="functions"/> is empty.</exception>
        /// <exception cref="Exception">If every <paramref name="functions"/>-method throws an exception.</exception>
        public static T RunAllConcurrentAndReturnFirstResult<T>(this ISet<Func<T>> functions, int maximalDegreeOfParallelism = 4)
        {
            return new RunAllConcurrentAndReturnFirstResultHelper<T>(maximalDegreeOfParallelism).RunAllConcurrentAndReturnFirstResult(functions);
        }
        private class RunAllConcurrentAndReturnFirstResultHelper<T>
        {
            private T _Result = default;
            private bool _ResultSet = false;
            public readonly object _LockObject = new object();
            private int _AmountOfRunningFunctions = 0;
            private int _MaximalDegreeOfParallelism { get; }

            public RunAllConcurrentAndReturnFirstResultHelper(int maximalDegreeOfParallelism)
            {
                this._MaximalDegreeOfParallelism = maximalDegreeOfParallelism;
            }

            private T Result
            {
                get
                {
                    lock (this._LockObject)
                    {
                        return this._Result;
                    }
                }
                set
                {
                    lock (this._LockObject)
                    {
                        if (!this.ResultSet)
                        {
                            this._Result = value;
                            this.ResultSet = true;
                        }
                    }
                }
            }
            private bool ResultSet
            {
                get
                {
                    lock (this._LockObject)
                    {
                        return this._ResultSet;
                    }
                }
                set
                {
                    lock (this._LockObject)
                    {
                        this._ResultSet = value;
                    }
                }
            }
            public T RunAllConcurrentAndReturnFirstResult(ISet<Func<T>> functions)
            {
                if (functions.Count == 0)
                {
                    throw new ArgumentException();
                }
                Parallel.ForEach(functions, new ParallelOptions { MaxDegreeOfParallelism = _MaximalDegreeOfParallelism }, new Action<Func<T>, ParallelLoopState>((Func<T> function, ParallelLoopState state) =>
                {
                    try
                    {
                        Interlocked.Increment(ref this._AmountOfRunningFunctions);
                        T result = function();
                        this.Result = result;
                        state.Stop();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref this._AmountOfRunningFunctions);
                    }
                }));
                SpinWait.SpinUntil(() => this.ResultSet || this._AmountOfRunningFunctions == 0);
                if (this._AmountOfRunningFunctions == 0 && !this.ResultSet)
                {
                    throw new Exception("No result was calculated");
                }
                else
                {
                    return this.Result;
                }
            }
        }
        public static ISet<string> ToCaseInsensitiveSet(this ISet<string> input)
        {
            ISet<TupleWithValueComparisonEquals<string, string>> tupleList = new HashSet<TupleWithValueComparisonEquals<string, string>>(input.Select((item) => new TupleWithValueComparisonEquals<string, string>(item, item.ToLower())));
            return new HashSet<string>((tupleList.Select((item) => item.Item1)));
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
            return @object.Persist(file, new UTF8Encoding(false));
        }
        public static SimpleObjectPersistence<T> Persist<T>(this T @object, string file, Encoding encoding) where T : new()
        {
            return new SimpleObjectPersistence<T>(file, encoding, @object);
        }
        public static SimpleObjectPersistence<T> Load<T>(this string file) where T : new()
        {
            return new SimpleObjectPersistence<T>(file);
        }
        public static SimpleObjectPersistence<T> Load<T>(this string file, Encoding encoding) where T : new()
        {
            return new SimpleObjectPersistence<T>(file, encoding);
        }

        //see https://stackoverflow.com/a/51284316/3905529
        public static string GetCommandLineArguments()
        {
            string exe = Environment.GetCommandLineArgs()[0];
            string rawCmd = Environment.CommandLine;
            return rawCmd.Remove(rawCmd.IndexOf(exe), exe.Length).TrimStart('"').Substring(1);
        }

        //see https://codereview.stackexchange.com/a/112844
        public static string ToPascalCase(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            IEnumerable<string> words = input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(word => word.Substring(0, 1).ToUpper() +
                                         word.Substring(1).ToLower());

            string result = string.Concat(words);
            return result;
        }
        public static string ToCamelCase(this string input)
        {
            string pascalCase = input.ToPascalCase();
            return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
        }

        //see https://stackoverflow.com/a/448225/3905529
        public static bool IsAllUpper(this string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsAllLower(this string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsLower(input[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsNegative(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks < 0;
        }
        public static bool IsPositive(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks > 0;
        }
        public static string ToOnlyFirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            if (input.Length == 1)
            {
                return input.ToUpper();
            }
            return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
        }
        private static readonly char[] Whitespace = new char[] { ' ' };
        private static readonly char[] WhitespaceAndPartialWordIndicators = new char[] { ' ', '_', '-' };
        public static string ToOnlyFirstCharOfEveryWordToUpper(this string input)
        {
            return ToOnlyFirstCharOfEveryWordToUpper(input, (lastCharacter) => Whitespace.Contains(lastCharacter));
        }
        public static string ToOnlyFirstCharOfEveryWordOrPartialWordToUpper(this string input)
        {
            return ToOnlyFirstCharOfEveryWordToUpper(input, (lastCharacter) => WhitespaceAndPartialWordIndicators.Contains(lastCharacter));
        }
        public static string ToOnlyFirstCharOfEveryNewLetterSequenceToUpper(this string input)
        {
            return ToOnlyFirstCharOfEveryWordToUpper(input, (lastCharacter) => !char.IsLetter(lastCharacter));
        }
        public static string ToOnlyFirstCharOfEveryWordToUpper(this string input, Func<char, bool> printCharUppercaseDependentOnPreviousChar)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            char[] splitted = input.ToCharArray();
            char lastChar = default;
            for (int i = 0; i < splitted.Length; i++)
            {
                if (0 == i)
                {
                    splitted[i] = splitted[i].ToString().ToUpper().First();
                }
                if (0 < i)
                {
                    if (printCharUppercaseDependentOnPreviousChar(lastChar))
                    {
                        splitted[i] = splitted[i].ToString().ToUpper().First();
                    }
                    else
                    {
                        splitted[i] = splitted[i].ToString().ToLower().First();
                    }
                }
                lastChar = splitted[i];
            }
            return new string(splitted);
        }

        public static bool FileEndsWithEmptyLine(string file)
        {
            return File.ReadAllBytes(file).Last().Equals(10);
        }
        public static bool FileIsEmpty(string file)
        {
            return File.ReadAllBytes(file).Count().Equals(0);
        }
        public static bool AppendFileDoesNotNeedNewLineCharacter(string file)
        {
            return FileIsEmpty(file) || FileEndsWithEmptyLine(file);
        }
        public static bool AppendFileDoesNeedNewLineCharacter(string file)
        {
            return !AppendFileDoesNotNeedNewLineCharacter(file);
        }
        public static bool IsRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || path.Length > 255)
            {
                return false;
            }
            return Uri.TryCreate(path, UriKind.Relative, out Uri @out);
        }
        public static bool IsAbsolutePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || path.Length > 255 || !Path.IsPathRooted(path))
            {
                return false;
            }
            string pathRoot = Path.GetPathRoot(path).Trim();
            return pathRoot.Length <= 2 && pathRoot != "/"
                ? false
                : !(pathRoot == path && pathRoot.StartsWith(@"\\") && pathRoot.IndexOf('\\', 2) == -1);
        }
        public static string GetAbsolutePath(string basePath, string relativePath)
        {
            if (basePath == null && relativePath == null)
            {
                Path.GetFullPath(".");
            }
            if (relativePath == null)
            {
                return basePath.Trim();
            }
            if (basePath == null)
            {
                basePath = Path.GetFullPath(".");
            }
            relativePath = relativePath.Trim();
            basePath = basePath.Trim();
            string finalPath = null;
            if (!Path.IsPathRooted(relativePath) || @"\".Equals(Path.GetPathRoot(relativePath)))
            {
                if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    finalPath = Path.Combine(Path.GetPathRoot(basePath), relativePath.TrimStart(Path.DirectorySeparatorChar));
                }
                else
                {
                    finalPath = Path.Combine(basePath, relativePath);
                }
            }
            else
            {
                finalPath = relativePath;
            }
            return Path.GetFullPath(finalPath);
        }
        public static bool DirectoryIsEmpty(string path)
        {
            return (Directory.GetFiles(path).Length == 0) && (Directory.GetDirectories(path).Length == 0);
        }
        public static bool DirectoryDoesNotContainFiles(string path)
        {
            if (Directory.GetFiles(path).Length > 0)
            {
                return false;
            }
            foreach (string subFolder in Directory.GetDirectories(path))
            {
                if (!DirectoryDoesNotContainFiles(subFolder))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool DirectoryDoesNotContainFolder(string path)
        {
            return Directory.GetFiles(path).Length > 0;
        }
        //see https://stackoverflow.com/a/9995303/3905529
        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            byte[] result = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                result[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + (GetHexValue(hex[(i << 1) + 1])));
            }
            return result;
        }

        public static int GetHexValue(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }

        public static void ClearFile(string file)
        {
            File.WriteAllText(file, string.Empty, Encoding.ASCII);
        }

        /// <summary>
        /// Authenticates a user against an active directory.
        /// </summary>
        /// <param name="username">Represents the name of the desired user. It can be "MyUsername" or optionally "Domain\MyUsername."</param>
        /// <param name="authentificationServerName">Represents the (LDAP-)server. The format must be with leading slashs, e. g. "//MyServer.com".</param>
        /// <returns>Returns true if and only if <paramref name="password"/> is the correct password for the user with the name <paramref name="username"/></returns>
        private static bool IsAuthenticated(string authentificationServerName, string username, string password, string authenticationProtocol = "LDAP")
        {
            bool isAuthenticated = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(authenticationProtocol + ":" + authentificationServerName, username, password);
                object nativeObject = entry.NativeObject;
                isAuthenticated = true;
            }
            catch (DirectoryServicesCOMException)
            {
                //Not authenticated. Reason can be found in exception if desired
            }
            catch (Exception)
            {
                //Not authenticated for unknown/other reasons if desired.
            }
            return isAuthenticated;
        }
    }
}
