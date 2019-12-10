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
using static GRYLibrary.Miscellaneous.TableGenerator;
using System.Numerics;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Xsl;

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

        public static void ReplaceUnderscoresInFolderTransitively(string folder, IDictionary<string, string> replacements)
        {
            void replaceInFile(string file, object obj) { string newFileWithPath = RenameFileIfRequired(file, replacements); ReplaceUnderscoresInFile(newFileWithPath, replacements); }
            void replaceInDirectory(string directory, object obj) { RenameFolderIfRequired(directory, replacements); }
            ForEachFileAndDirectoryTransitively(folder, replaceInDirectory, replaceInFile);
        }

        private static string RenameFileIfRequired(string file, IDictionary<string, string> replacements)
        {
            string originalFilename = Path.GetFileName(file);
            string newFilename = ReplaceUnderscores(originalFilename, replacements);
            string result = Path.Combine(Path.GetDirectoryName(file), newFilename);
            if (!newFilename.Equals(originalFilename))
            {
                File.Move(file, result);
            }
            return result;
        }

        private static string RenameFolderIfRequired(string folder, IDictionary<string, string> replacements)
        {
            string originalFoldername = new DirectoryInfo(folder).Name;
            string newFoldername = ReplaceUnderscores(originalFoldername, replacements);
            string result = Path.Combine(Path.GetDirectoryName(folder), newFoldername);
            if (!newFoldername.Equals(originalFoldername))
            {
                Directory.Move(originalFoldername, result);
            }
            return result;
        }

        public static string ReplaceUnderscores(string @string, IDictionary<string, string> replacements)
        {
            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                @string = @string.Replace($"__{replacement.Key}__", replacement.Value);
            }
            return @string;
        }

        public static void ReplaceUnderscoresInFile(string file, IDictionary<string, string> replacements)
        {
            ReplaceUnderscoresInFile(file, replacements, new UTF8Encoding(false));
        }
        public static void ReplaceUnderscoresInFile(string file, IDictionary<string, string> replacements, Encoding encoding)
        {
            string originalContent = File.ReadAllText(file, encoding);
            string replacedContent = ReplaceUnderscores(originalContent, replacements);
            if (!originalContent.Equals(replacedContent))
            {
                File.WriteAllText(file, replacedContent, encoding);
            }
        }
        public static void WriteToConsoleAsASCIITable(IList<IList<string>> columns)
        {
            string[] table = TableGenerator.Generate(JaggedArrayToTwoDimensionalArray(EnumerableOfEnumerableToJaggedArray(columns)), new ASCIITable());
            foreach (string line in table)
            {
                Console.WriteLine(line);
            }
        }

        public static T[][] EnumerableOfEnumerableToJaggedArray<T>(IEnumerable<IEnumerable<T>> items)
        {
            return items.Select(Enumerable.ToArray).ToArray();
        }
        public static T[,] JaggedArrayToTwoDimensionalArray<T>(T[][] items)
        {
            int amountOfItemsInFirstDimension = items.Length;
            int amountOfItemsInSecondDimension = items.GroupBy(tArray => tArray.Length).Single().Key;
            T[,] result = new T[amountOfItemsInFirstDimension, amountOfItemsInSecondDimension];
            for (int i = 0; i < amountOfItemsInFirstDimension; ++i)
            {
                for (int j = 0; j < amountOfItemsInSecondDimension; ++j)
                {
                    result[i, j] = items[i][j];
                }
            }

            return result;
        }

        /// <returns>
        /// Returns a new <see cref="Guid"/> whose value in the last block is incremented
        /// </returns>
        public static Guid IncrementGuid(Guid id, long valueToIncrement = 1)
        {
            return IncrementGuid(id, new BigInteger(valueToIncrement));
        }
        public static Guid IncrementGuid(Guid id, BigInteger valueToIncrement)
        {
            BigInteger value = BigInteger.Parse(id.ToString("N"), NumberStyles.HexNumber);
            return Guid.Parse((value + valueToIncrement).ToString("X").PadLeft(32, '0'));
        }

        public static IEnumerable<IEnumerable<T>> JaggedArrayToEnumerableOfEnumerable<T>(T[][] items)
        {
            List<List<T>> result = new List<List<T>>();
            foreach (T[] innerArray in items)
            {
                List<T> innerList = new List<T>();
                foreach (T item in innerArray)
                {
                    innerList.Add(item);
                }
                result.Add(innerList);
            }
            return result;
        }
        public static T[][] TwoDimensionalArrayToJaggedArray<T>(T[,] items)
        {
            int rowsFirstIndex = items.GetLowerBound(0);
            int rowsLastIndex = items.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;
            int columnsFirstIndex = items.GetLowerBound(1);
            int columnsLastIndex = items.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;
            T[][] result = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                result[i] = new T[numberOfColumns];
                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    result[i][j] = items[i, j];
                }
            }
            return result;
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
            EnsureDirectoryExists(destinationFolder);
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
            ForEachFileAndDirectoryTransitively(folder, (string directory, object argument) => { if (DirectoryIsEmpty(directory)) { Directory.Delete(directory); } }, (string file, object argument) => { }, false, null, null);
            if (deleteFolderItselfIfAlsoEmpty && DirectoryIsEmpty(folder))
            {
                Directory.Delete(folder);
            }
        }

        public static void MoveFolderAcrossVolumes(string sourceFolder, string destinationFolder, bool deleteSourceFolderCompletely = true)
        {
            CopyFolderAcrossVolumes(sourceFolder, destinationFolder);
            DeleteFolder(sourceFolder, deleteSourceFolderCompletely);
        }

        public static void DeleteFolder(string folder, bool deleteSourceFolderCompletely = true)
        {
            if (deleteSourceFolderCompletely)
            {
                Directory.Delete(folder, true);
            }
            else
            {
                DeleteContentOfFolder(folder);
            }
        }
        public static string TwoDimensionalArrayToString<T>(T[,] array)
        {
            return string.Join(",", array.OfType<T>().Select((value, index) => new { value, index }).GroupBy(x => x.index / array.GetLength(1)).Select(x => $"{{{string.Join(",", x.Select(y => y.value))}}}"));
        }

        public static bool TwoDimensionalArrayEquals<T>(T[,] array1, T[,] array2)
        {
            return array1.Rank == array2.Rank && Enumerable.Range(0, array1.Rank).All(dimension => array1.GetLength(dimension) == array2.GetLength(dimension)) && array1.Cast<T>().SequenceEqual(array2.Cast<T>());
        }

        public static void DeleteContentOfFolder(string folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
            {
                subDirectoryInfo.Delete(true);
            }
        }

        internal static bool TryResolvePathByPathVariable(string program, out string programWithFullPath)
        {
            programWithFullPath = null;
            string[] knownExtension = new string[] { ".exe", ".cmd" };
            string paths = Environment.ExpandEnvironmentVariables("%PATH%");
            foreach (string path in paths.Split(';'))
            {

                foreach (string combined in GetCombinations(path, knownExtension, program))
                {
                    if (File.Exists(combined))
                    {
                        programWithFullPath = combined;
                    }
                }
            }
            return programWithFullPath != null;

        }

        private static IEnumerable<string> GetCombinations(string path, string[] knownExtensions, string program)
        {
            string programToLower = program.ToLower();
            foreach (string extension in knownExtensions)
            {
                if (programToLower.EndsWith(extension))
                {
                    return new string[] { Path.Combine(path, programToLower) };
                }
            }
            List<string> result = new List<string>();
            foreach (string extension in knownExtensions)
            {
                result.Add(Path.Combine(path, program + extension));
            }
            return result;
        }

        public static void MoveContentOfFoldersAcrossVolumes(string sourceFolder, string targetFolder)
        {
            MoveContentOfFoldersAcrossVolumes(sourceFolder, targetFolder, FileSelector.FilesInFolder(sourceFolder, true));
        }
        public static void MoveContentOfFoldersAcrossVolumes(string sourceFolder, string targetFolder, FileSelector fileSelector, bool deleteAlreadyExistingFilesWithoutCopy = false)
        {
            MoveContentOfFoldersAcrossVolumes(sourceFolder, targetFolder, fileSelector, (exception) => { }, deleteAlreadyExistingFilesWithoutCopy);
        }
        public static void MoveContentOfFoldersAcrossVolumes(string sourceFolder, string targetFolder, Func<string, bool> fileSelectorPredicate, bool deleteAlreadyExistingFilesWithoutCopy = false)
        {
            MoveContentOfFoldersAcrossVolumes(sourceFolder, targetFolder, fileSelectorPredicate, (exception) => { }, deleteAlreadyExistingFilesWithoutCopy);
        }

        public static void MoveContentOfFoldersAcrossVolumes(string sourceFolder, string targetFolder, FileSelector fileSelector, Action<Exception> errorHandler, bool deleteAlreadyExistingFilesWithoutCopy = false)
        {
            MoveContentOfFoldersAcrossVolumes(sourceFolder, targetFolder, (file) => fileSelector.Files.Contains(file), errorHandler, deleteAlreadyExistingFilesWithoutCopy);
        }
        /// <summary>
        /// Moves the content of <paramref name="sourceFolder"/> to <paramref name="targetFolder"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="deleteAlreadyExistingFilesWithoutCopy"/>==true then the files in <paramref name="sourceFolder"/> which do already exist in <paramref name="targetFolder"/> will be deleted without copying them and without any backup. (Only filepath/-name will be compared. The content of the file does not matter for this comparison.)
        /// This function preserves the directory-structure of <paramref name="sourceFolder"/>.
        /// This function ignores empty directories in <paramref name="sourceFolder"/>.
        /// </remarks>
        public static void MoveContentOfFoldersAcrossVolumes(string sourceFolder, string targetFolder, Func<string, bool> fileSelectorPredicate, Action<Exception> errorHandler, bool deleteAlreadyExistingFilesWithoutCopy = false)
        {
            void fileAction(string sourceFile, object @object)
            {
                try
                {
                    if (fileSelectorPredicate(sourceFile))
                    {
                        string sourceFolderTrimmed = sourceFolder.Trim().TrimStart('/', '\\').TrimEnd('/', '\\');
                        string fileName = Path.GetFileName(sourceFile);
                        string fullTargetFolder = Path.Combine(targetFolder, Path.GetDirectoryName(sourceFile).Substring(sourceFolderTrimmed.Length).TrimStart('/', '\\'));
                        EnsureDirectoryExists(fullTargetFolder);
                        string targetFile = Path.Combine(fullTargetFolder, fileName);
                        if (File.Exists(targetFile))
                        {
                            if (deleteAlreadyExistingFilesWithoutCopy)
                            {
                                File.Delete(sourceFile);
                            }
                        }
                        else
                        {
                            File.Copy(sourceFile, targetFile);
                            File.Delete(sourceFile);
                        }
                    }
                }
                catch (Exception exception)
                {
                    errorHandler(exception);
                }
            }
            ForEachFileAndDirectoryTransitively(sourceFolder, (str, obj) => { }, (sourceFile, @object) => fileAction(sourceFile, @object), false, null, null);
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
        /// Starts all <see cref="Func{TResult}"/>-objects in <paramref name="functions"/> concurrent and return all results which did not throw an exception.
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
            private readonly int _MaximalDegreeOfParallelism = 4;

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
            return new HashSet<string>(tupleList.Select((item) => item.Item1));
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

        private static readonly IFormatter _Formatter = new BinaryFormatter();
        //see https://stackoverflow.com/a/129395/3905529
        public static T DeepClone<T>(this T @object)
        {
            using (Stream memoryStream = new MemoryStream())
            {
                _Formatter.Serialize(memoryStream, @object);
                memoryStream.Position = 0;
                return (T)_Formatter.Deserialize(memoryStream);
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
            return new SimpleObjectPersistence<T>(file, @object, new XmlWriterSettings() { Indent = true, Encoding = encoding });
        }
        public static SimpleObjectPersistence<T> Load<T>(this string file) where T : new()
        {
            return new SimpleObjectPersistence<T>(file);
        }
        public static SimpleObjectPersistence<T> Load<T>(this string file, Encoding encoding) where T : new()
        {
            return new SimpleObjectPersistence<T>(file, encoding);
        }

        public static string GetCommandLineArguments()
        {
            string exe = Environment.GetCommandLineArgs()[0];
            string rawCmd = Environment.CommandLine;
            return rawCmd.Remove(rawCmd.IndexOf(exe), exe.Length).TrimStart('"').Substring(1);
        }

        public static string ToPascalCase(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            IEnumerable<string> words = input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(word => word.Substring(0, 1).ToUpper() +
                                         word.Substring(1).ToLower());

            return string.Concat(words);
        }
        public static string ToCamelCase(this string input)
        {
            string pascalCase = input.ToPascalCase();
            return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
        }

        private static Regex OneOrMoreHexSigns = new Regex(@"^[0-9a-f]+$");
        public static bool IsHexString(string result)
        {
            return OneOrMoreHexSigns.Match(result.ToLower()).Success;
        }
        public static bool IsHexDigit(this char @char)
        {
            return (@char >= '0' && @char <= '9') || (@char >= 'a' && @char <= 'f') || (@char >= 'A' && @char <= 'F');
        }

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
            return Uri.TryCreate(path, UriKind.Relative, out _);
        }
        public static bool IsAbsolutePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || path.Length > 255 || !Path.IsPathRooted(path))
            {
                return false;
            }
            string pathRoot = Path.GetPathRoot(path).Trim();
            return pathRoot.Length <= 2 && pathRoot != "/" ? false : !(pathRoot == path && pathRoot.StartsWith(@"\\") && pathRoot.IndexOf('\\', 2) == -1);
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
            string finalPath;
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
        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            byte[] result = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                result[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + GetHexValue(hex[(i << 1) + 1]));
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
        /// <param name="password">Represents the password to authenticate <paramref name="username"/>.</param>
        /// <param name="authenticationProtocol"></param>
        /// <param name="authentificationServerName">Represents the (LDAP-)server. The format must be with leading slashs, e. g. "//MyServer.com".</param>
        /// <returns>Returns true if and only if <paramref name="password"/> is the correct password for the user with the name <paramref name="username"/></returns>
        public static bool IsAuthenticated(string authentificationServerName, string username, string password, string authenticationProtocol = "LDAP")
        {
            bool isAuthenticated = false;
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry(authenticationProtocol + ":" + authentificationServerName, username, password))
                {
                    object nativeObject = entry.NativeObject;
                }
                isAuthenticated = true;
            }
            catch (DirectoryServicesCOMException)
            {
                //Not authenticated. Reason can be found in exception if desired
            }
            catch (Exception)
            {
                //Not authenticated for unknown/other reasons.
            }
            return isAuthenticated;
        }
        private const char Slash = '/';
        private const char Backslash = '\\';
        public static string EnsurePathStartsWithSlash(this string path)
        {
            if (path.StartsWith(Slash.ToString()))
            {
                return path;
            }
            else
            {
                return Slash + path;
            }
        }
        public static string EnsurePathStartsWithBackslash(this string path)
        {
            if (path.StartsWith(Slash.ToString()))
            {
                return path;
            }
            else
            {
                return Backslash + path;
            }
        }
        public static string EnsurePathStartsWithoutSlash(this string path)
        {
            if (path.StartsWith(Slash.ToString()))
            {
                return path.TrimStart(Slash);
            }
            else
            {
                return path;
            }
        }
        public static string EnsurePathStartsWithoutBackslash(this string path)
        {
            if (path.StartsWith(Backslash.ToString()))
            {
                return path.TrimStart(Slash);
            }
            else
            {
                return path;
            }
        }
        public static string EnsurePathEndsWithSlash(this string path)
        {
            if (path.EndsWith(Slash.ToString()))
            {
                return path;
            }
            else
            {
                return path + Slash;
            }
        }
        public static string EnsurePathEndsWithBackslash(this string path)
        {
            if (path.EndsWith(Backslash.ToString()))
            {
                return path;
            }
            else
            {
                return path + Backslash;
            }
        }
        public static string EnsurePathEndsWithoutSlash(this string path)
        {
            if (path.EndsWith(Slash.ToString()))
            {
                return path.TrimEnd(Slash);
            }
            else
            {
                return path;
            }
        }
        public static string EnsurePathEndsWithoutBackslash(this string path)
        {
            if (path.EndsWith(Backslash.ToString()))
            {
                return path.TrimEnd(Backslash);
            }
            else
            {
                return path;
            }
        }
        public static string EnsurePathStartsWithoutSlashOrBackslash(this string path)
        {
            return path.EnsurePathStartsWithoutSlash().EnsurePathStartsWithoutBackslash();
        }
        public static string EnsurePathSEndsWithoutSlashOrBackslash(this string path)
        {
            return path.EnsurePathEndsWithoutSlash().EnsurePathEndsWithoutBackslash();
        }

        public static bool StartsWith<T>(T[] entireArray, T[] start)
        {
            if (start.Count() > entireArray.Count())
            {
                return false;
            }
            for (int i = 0; i < start.Length; i++)
            {
                if (!entireArray[i].Equals(start[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static string ByteArrayToHexString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 == 1)
            {
                hexString = "0" + hexString;
            }
            int inputLength = hexString.Length;
            byte[] bytes = new byte[inputLength / 2];
            for (int i = 0; i < inputLength; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }
        public static string IntegerToHexString(BigInteger input)
        {
            string result = input.ToString("X");
            if (result.StartsWith("0"))
            {
                return result.Substring(1);
            }
            else
            {
                return result;
            }
        }
        public static BigInteger HexStringToInteger(string input)
        {
            return BigInteger.Parse(input, NumberStyles.HexNumber);
        }
        public static T[] Concat<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }
        public static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception("Assertion failed. Condition is false." + (string.IsNullOrWhiteSpace(message) ? string.Empty : " " + message));
            }
        }
        public static string[,] ReadCSVFile(string file, string separator, bool ignoreFirstLine, Encoding encoding)
        {
            string[] lines = File.ReadAllLines(file, encoding);
            List<List<string>> outterList = new List<List<string>>();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (!(i == 0 && ignoreFirstLine))
                {
                    string line = lines[i].Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        List<string> innerList = new List<string>();
                        if (line.Contains(separator))
                        {
                            innerList.AddRange(line.Split(new string[] { separator }, StringSplitOptions.None));
                        }
                        else
                        {
                            innerList.Add(line);
                        }
                        outterList.Add(innerList);
                    }
                }
            }
            return JaggedArrayToTwoDimensionalArray(EnumerableOfEnumerableToJaggedArray(outterList));
        }
        public static bool RunWithTimeout(this ThreadStart threadStart, TimeSpan timeout)
        {
            Thread workerThread = new Thread(threadStart);
            workerThread.Start();
            bool terminatedInGivenTimeSpan = workerThread.Join(timeout);
            if (!terminatedInGivenTimeSpan)
            {
                workerThread.Abort();
            }
            return terminatedInGivenTimeSpan;
        }
        public static string ResolveToFullPath(this string relativePath, string baseDirectory)
        {
            return Path.GetFullPath(new Uri(Path.Combine(baseDirectory, relativePath)).LocalPath);
        }
        public static byte[] DecryptAsymmetrical(byte[] content, string privateKey)
        {
            throw new NotImplementedException();
        }
        public static byte[] EncryptAsymmetrical(byte[] content, string publicKey)
        {
            throw new NotImplementedException();
        }
        public static byte[] DecryptSymmetrical(byte[] content, string key)
        {
            throw new NotImplementedException();
        }
        public static byte[] EncryptSymmetrical(byte[] content, string key)
        {
            throw new NotImplementedException();
        }
        public static bool ValidateXMLAgainstXSD(string xml, string xsd, out IList<object> errorMessages)
        {
            try
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XmlReader.Create(new StringReader(xsd)));
                XDocument xDocument = XDocument.Parse(xml);
                List<object> errorMessagesList = new List<object>();

                xDocument.Validate(schemaSet, (o, eventArgument) =>
                {
                    errorMessagesList.Add(eventArgument);
                });
                errorMessages = errorMessagesList;
                return errorMessages.Count == 0;
            }
            catch (Exception e)
            {
                errorMessages = new List<object>() { e };
                return false;
            }
        }

        public static readonly XmlWriterSettings ApplyXSLTToXMLXMLWriterDefaultSettings = new XmlWriterSettings() { Indent = true, Encoding = new UTF8Encoding(false), OmitXmlDeclaration = true, IndentChars = "    " };
        public static readonly string ApplyXSLTToXMLXMLWriterDefaultXMLDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";
        public static string ApplyXSLTToXML(string xml, string xslt)
        {
            return ApplyXSLTToXML(xml, xslt, ApplyXSLTToXMLXMLWriterDefaultXMLDeclaration, ApplyXSLTToXMLXMLWriterDefaultSettings);
        }
        public static string ApplyXSLTToXML(string xml, string xslt, string xmlDeclaration)
        {
            return ApplyXSLTToXML(xml, xslt, xmlDeclaration, ApplyXSLTToXMLXMLWriterDefaultSettings);
        }
        public static string ApplyXSLTToXML(string xml, string xslt, XmlWriterSettings applyXSLTToXMLXMLWriterDefaultSettings)
        {
            return ApplyXSLTToXML(xml, xslt, ApplyXSLTToXMLXMLWriterDefaultXMLDeclaration, applyXSLTToXMLXMLWriterDefaultSettings);
        }
        public static string ApplyXSLTToXML(string xml, string xslt, string xmlDeclaration, XmlWriterSettings applyXSLTToXMLXMLWriterDefaultSettings)
        {
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(XmlReader.Create(new StringReader(xslt)));
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, applyXSLTToXMLXMLWriterDefaultSettings))
                {
                    myXslTrans.Transform(XmlReader.Create(new StringReader(xml)), xmlWriter);

                }
                return xmlDeclaration + stringWriter.ToString();
            }
        }
        public static void MountVolume(Guid volumeId, string mountPath)
        {
            ExternalProgramExecutor externalProgramExecutor = ExternalProgramExecutor.Create("mountvol", $"{mountPath} \\\\?\\Volume{{{volumeId.ToString()}}}\\");
            externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;
            externalProgramExecutor.LogObject.Configuration.PrintOutputInConsole = false;
            externalProgramExecutor.StartConsoleApplicationInCurrentConsoleWindow();
        }
        public static ISet<Guid> GetAvailableVolumeIds()
        {
            ExternalProgramExecutor externalProgramExecutor = ExternalProgramExecutor.Create("mountvol", string.Empty);
            externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;
            externalProgramExecutor.LogObject.Configuration.PrintOutputInConsole = false;
            externalProgramExecutor.StartConsoleApplicationInCurrentConsoleWindow();
            HashSet<Guid> result = new HashSet<Guid>();
            foreach (string rawLine in externalProgramExecutor.AllStdOutLines)
            {
                string line = rawLine.Trim(); //line is "\\?\Volume{80aa12de-7392-4051-8cd2-f28bf56dc9d3}\"
                string prefix = "\\\\?\\Volume{";
                if (line.StartsWith(prefix))
                {
                    line = line.Substring(prefix.Length);//remove "\\?\Volume{"
                    line = line.Substring(line.Length - 2);//remove "}\"
                    result.Add(Guid.Parse(line));
                }
            }
            return result;
        }
        public static ISet<string> GetMountPaths(Guid volumeId)
        {
            HashSet<string> result = new HashSet<string>();
            ExternalProgramExecutor externalProgramExecutor = ExternalProgramExecutor.Create("mountvol", string.Empty);
            externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;
            externalProgramExecutor.LogObject.Configuration.PrintOutputInConsole = false;
            externalProgramExecutor.StartConsoleApplicationInCurrentConsoleWindow();
            for (int i = 0; i < externalProgramExecutor.AllStdOutLines.Length; i++)
            {
                string line = externalProgramExecutor.AllStdOutLines[i].Trim();
                if (line.StartsWith($"\\\\?\\Volume{{{volumeId.ToString()}}}\\"))
                {
                    int j = i;
                    do
                    {
                        j += 1;
                        string mountPath = externalProgramExecutor.AllStdOutLines[j].Trim();
                        if (Directory.Exists(mountPath))
                        {
                            result.Add(mountPath);
                        }
                    } while (!string.IsNullOrWhiteSpace(externalProgramExecutor.AllStdOutLines[j]));
                    return result;
                }
            }
            return result;
        }
        public static void DismountAllMountPointsOfVolume(Guid volumeId)
        {
            foreach (string mountPoint in GetMountPaths(volumeId))
            {
                DismountVolume(mountPoint);
            }
        }
        public static void DismountVolume(string mountPath)
        {
            ExternalProgramExecutor externalProgramExecutor = ExternalProgramExecutor.Create("mountvol", $"{mountPath} /d");
            externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;
            externalProgramExecutor.LogObject.Configuration.PrintOutputInConsole = false;
            externalProgramExecutor.StartConsoleApplicationInCurrentConsoleWindow();
        }
    }
}
