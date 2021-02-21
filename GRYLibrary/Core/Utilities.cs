using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper;
using System;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.Xsl;
using static GRYLibrary.Core.TableGenerator;
using System.Reflection;
using System.Dynamic;
using GRYLibrary.Core.XMLSerializer;
using System.Net.Sockets;
using GRYLibrary.Core.OperatingSystem;
using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer;
using System.Runtime.InteropServices;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GRYLibrary.Core
{
    public static class Utilities
    {
        #region Constants
        public const string EmptyString = "";
        public const string SpecialCharacterTest = "<Special-character-Test: (^äöüß/\\€\"\'+-*®¬¼😊👍✆⊆ℙ≈∑∞∫/𝄞𝄤𝅘𝅥𝅮) (您好) (Здравствуйте) (नमस्कार)>";
        #endregion

        #region Miscellaneous
        public static PercentValue ToPercentValue(this float value)
        {
            return new PercentValue(value);
        }
        public static PercentValue ToPercentValue(this int value)
        {
            return new PercentValue((decimal)value);
        }
        public static PercentValue ToPercentValue(this double value)
        {
            return new PercentValue(value);
        }
        public static PercentValue ToPercentValue(this decimal value)
        {
            return new PercentValue(value);
        }
        public static uint SwapEndianness(uint value)
        {
            return ((value & 0x000000ff) << 24) +
                   ((value & 0x0000ff00) << 8) +
                   ((value & 0x00ff0000) >> 8) +
                   ((value & 0xff000000) >> 24);
        }

        public static byte[] GetRandomByteArray(long length = 65536)
        {
            byte[] result = new byte[length];
            new Random().NextBytes(result);
            return result;
        }

        /// <summary>
        /// This is the inverse function of <see cref="ConcatBytesArraysWithLengthInformation"/>
        /// </summary>
        public static byte[][] GetBytesArraysFromConcatBytesArraysWithLengthInformation(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is the inverse function of <see cref="GetBytesArraysFromConcatBytesArraysWithLengthInformation"/>
        /// </summary>
        public static byte[] ConcatBytesArraysWithLengthInformation(params byte[][] byteArrays)
        {
            byte[] result = Array.Empty<byte>();
            foreach (byte[] byteArray in byteArrays)
            {
                result = Concat(result, UnsignedInteger32BitToByteArray((uint)byteArray.Length), byteArray);
            }
            return result;
        }

        public static uint[] ByteArrayToUnsignedInteger32BitArray(byte[] byteArray)
        {
            if ((byteArray.Length % 4) != 0)
            {
                throw new ArgumentException();
            }
            var result = new uint[byteArray.Length / 4];
            for (int i = 0; i < byteArray.Length / 4; i++)
            {
                result[i] = ByteArrayToUnsignedInteger32Bit(new byte[] { byteArray[4 * i], byteArray[4 * i + 1], byteArray[4 * i + 2], byteArray[4 * i + 3] });
            }
            return result;
        }

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

        public static IEnumerable<PropertyInfo> GetPropertiesWhichHaveGetterAndSetter(Type type)
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            foreach (PropertyInfo property in type.GetType().GetProperties())
            {
                if (property.CanWrite && property.CanRead)
                {
                    result.Add(property);
                }
            }
            return result;
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

        public static bool IsValidXML(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                return false;
            }
            try
            {
                XDocument.Parse(xmlString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int Count(this IEnumerable enumerable)
        {
            int result = 0;
            IEnumerator enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result += 1;
            }
            return result;
        }
        /// <summary>
        /// This function does nothing.
        /// The purpose of this function is to say explicitly that nothing should be done at the point where this function is called.
        /// </summary>
        public static void NoOperation()
        {
            //nothing to do
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
                if (Directory.Exists(result))
                {
                    MoveContentOfFoldersAcrossVolumes(folder, result);
                    DeleteFolder(folder);
                }
                else
                {
                    Directory.Move(folder, result);
                }
            }
            return result;
        }

        public static string ReplaceUnderscores(string @string, IDictionary<string, string> replacements)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, string> replacement in replacements)
            {
                @string = @string.Replace($"__{replacement.Key}__", replacement.Value);
            }
            return @string;
        }
        public static bool ObjectIsPrimitive(object @object)
        {
            return TypeIsPrimitive(@object.GetType());
        }
        public static bool TypeIsPrimitive(Type type)
        {
            if (type.IsGenericType)
            {
                return false;
            }
            else
            {
                return type.IsPrimitive || typeof(string).Equals(type) || type.IsValueType;
            }
        }
        public static bool IsAssignableFrom(object @object, Type genericTypeToCompare)
        {
            return TypeIsAssignableFrom(@object.GetType(), genericTypeToCompare);
        }
        public static bool TypeIsAssignableFrom(Type typeForCheck, Type parentType)
        {
            ISet<Type> typesToCheck = GetTypeWithParentTypesAndInterfaces(typeForCheck);
            return typesToCheck.Contains(parentType, TypeComparerIgnoringGenerics);
        }
        public static ISet<Type> GetTypeWithParentTypesAndInterfaces(Type type)
        {
            HashSet<Type> result = new HashSet<Type> { type };
            result.UnionWith(type.GetInterfaces());
            if (type.BaseType != null)
            {
                result.UnionWith(GetTypeWithParentTypesAndInterfaces(type.BaseType));
            }
            return result;
        }
        public static IEqualityComparer<Type> TypeComparerIgnoringGenerics { get; } = new TypeComparerIgnoringGenericsType();
        private class TypeComparerIgnoringGenericsType : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                if (!x.Name.Equals(y.Name))
                {
                    return false;
                }
                if (!x.Namespace.Equals(y.Namespace))
                {
                    return false;
                }
                if (!x.Assembly.Equals(y.Assembly))
                {
                    return false;
                }
                return true;
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
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
            string[] table = Generate(JaggedArrayToTwoDimensionalArray(EnumerableOfEnumerableToJaggedArray(columns)), new ASCIITable());
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
            path = ResolveToFullPath(path);
            string directory = Path.GetDirectoryName(path);
            if (createDirectoryIfRequired && !string.IsNullOrWhiteSpace(directory))
            {
                EnsureDirectoryExists(directory);
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

        public static string DurationToUserFriendlyString(TimeSpan timespan)
        {
            return $"{Math.Floor(timespan.TotalHours).ToString().PadLeft(2, '0')}:{timespan.Minutes.ToString().PadLeft(2, '0')}:{timespan.Seconds.ToString().PadLeft(2, '0')}";
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
            (bool, string) result = OperatingSystem.OperatingSystem.GetCurrentOperatingSystem().Accept(new TryResolvePathByPathVariableVisitor(program));
            programWithFullPath = result.Item2;
            return result.Item1;
        }

        private class TryResolvePathByPathVariableVisitor : IOperatingSystemVisitor<(bool/*Success*/, string/*programWithFullPath*/)>
        {
            private readonly string _Programname;

            public TryResolvePathByPathVariableVisitor(string programname)
            {
                this._Programname = programname;
            }

            public (bool, string) Handle(OSX operatingSystem)
            {
                throw new NotImplementedException();
            }

            public (bool, string) Handle(Windows operatingSystem)
            {
                string program = null;
                string[] knownExtension = new string[] { ".exe", ".cmd" };
                string paths = Environment.ExpandEnvironmentVariables("%PATH%");
                bool @break = false;
                foreach (string path in paths.Split(';'))
                {
                    foreach (string combined in GetCombinations(path, knownExtension, this._Programname))
                    {
                        if (File.Exists(combined))
                        {
                            program = combined;
                            @break = true;
                            break;
                        }
                    }
                    if (@break)
                    {
                        break;
                    }
                }
                return (program != null, program);
            }

            public (bool, string) Handle(Linux operatingSystem)
            {
                throw new NotImplementedException();
            }
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
                        string fullTargetFolder = Path.Combine(targetFolder, Path.GetDirectoryName(sourceFile)[sourceFolderTrimmed.Length..].TrimStart('/', '\\'));
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
                    throw new ArgumentException($"Argument '{ nameof(functions) }' does not contain any function.");
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
                WaitUntilConditionIsTrue(() => this.ResultSet || this._AmountOfRunningFunctions == 0);
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
        public static void WaitUntilConditionIsTrue(Func<bool> condition)
        {
            while (!condition())
            {
                Thread.Sleep(50);
            }
        }
        public static ISet<string> ToCaseInsensitiveSet(this ISet<string> input)
        {
            ISet<WriteableTuple<string, string>> tupleList = new HashSet<WriteableTuple<string, string>>(input.Select((item) => new WriteableTuple<string, string>(item, item.ToLower())));
            return new HashSet<string>(tupleList.Select((item) => item.Item1));
        }
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> dictionary = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                dictionary.Add(property.Name, property.GetValue(value));
            }
            return dictionary as ExpandoObject;
        }

        private static readonly IFormatter _Formatter = new BinaryFormatter();
        public static T DeepClone<T>(this T @object)
        {
            return Generic.GenericDeserialize<T>(Generic.GenericSerialize(@object));
        }
        /// <summary>
        /// Casts an object to the given type if possible.
        /// This can be useful for example to to cast 'Action&lt;Object&gt' to 'Action' or 'Func&lt;string&gt' to 'Func&lt;Object&gt' to fulfil interface-compatibility.
        /// </summary>
        public static object Cast(object @object, Type targetType)
        {
            throw new NotImplementedException();// TODO call CastHelper using reflection
        }
        private static T CastHelper<T>(object @object)
        {
            return (T)@object;
        }
        public static long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
        public static SimpleObjectPersistence<T> PersistToDisk<T>(this T @object, string file) where T : new()
        {
            SimpleObjectPersistence<T> result = SimpleObjectPersistence<T>.CreateByObjectAndFile(@object, file);
            result.SaveObjectToFile();
            return result;
        }
        public static SimpleObjectPersistence<T> LoadFromDisk<T>(this string file) where T : new()
        {
            SimpleObjectPersistence<T> result = SimpleObjectPersistence<T>.CreateByFile(file);
            result.LoadObjectFromFile();
            return result;
        }
        /// <returns>Returns the command line arguments of the current executed program.</returns>
        /// <remarks>It is guaranteed that the result does not have leading or trailing whitespaces.</remarks>
        public static string GetCommandLineArguments()
        {
            string rawCmd = Environment.CommandLine;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() == 1)
            {
                return string.Empty;
            }
            string exe = args[0];
            string quotedExe = "\"" + exe + "\"";
            if (rawCmd.StartsWith(exe))
            {
                rawCmd = rawCmd[(exe.Length + 1)..];
            }
            else if (rawCmd.StartsWith(quotedExe))
            {
                rawCmd = rawCmd[(quotedExe.Length + 1)..];
            }
            return rawCmd.Trim();
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
            return (pathRoot.Length > 2 || pathRoot == "/") && !(pathRoot == path && pathRoot.StartsWith(@"\\") && pathRoot.IndexOf('\\', 2) == -1);
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

        public static void ClearFile(string file)
        {
            File.WriteAllText(file, string.Empty, Encoding.ASCII);
        }

        private const char SingleQuote = '\'';
        private const char DoubleQuote = '"';
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
        public static string EnsurePathEndsWithoutSlashOrBackslash(this string path)
        {
            return path.EnsurePathEndsWithoutSlash().EnsurePathEndsWithoutBackslash();
        }

        public static string EnsurePathHasNoLeadingOrTrailingQuotes(this string path)
        {
            string result = path;
            bool changed = true;
            while (changed)
            {
                string old = result;
                result = result.TrimStart(SingleQuote).TrimEnd(SingleQuote).TrimStart(DoubleQuote).TrimEnd(DoubleQuote);
                changed = old != result;
            }
            return result;
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

        public static byte[] StringToByteArray(string @string)
        {
            return new UTF8Encoding(false).GetBytes(@string);
        }
        public static string ByteArrayToString(byte[] bytes)
        {
            return new UTF8Encoding(false).GetString(bytes);
        }
        public static string ByteArrayToHexString(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 == 1)
            {
                throw new ArgumentException();
            }
            hexString = hexString.ToUpper();
            byte[] result = new byte[hexString.Length >> 1];
            for (int i = 0; i < hexString.Length >> 1; ++i)
            {
                result[i] = (byte)((GetHexValue(hexString[i << 1]) << 4) + GetHexValue(hexString[(i << 1) + 1]));
            }
            return result;
        }

        private static int GetHexValue(char hex)
        {
            int val = hex;
            return val - (val < 58 ? 48 : 55);
        }

        public static string BigIntegerToHexString(BigInteger input)
        {
            return input.ToString("X");
        }
        public static BigInteger HexStringToBigInteger(string input)
        {
            return BigInteger.Parse(input.ToUpper(), NumberStyles.HexNumber);
        }
        public static T[] Concat<T>(params T[][] arrays)
        {
            T[] result = Array.Empty<T>();
            foreach (T[] array in arrays)
            {
                result = Concat2Arrays(result, array);
            }
            return result;
        }
        private static T[] Concat2Arrays<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }
        public static bool StringToBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            value = value.Trim().ToLower();
            return value == "1"
                || value == "y"
                || value == "yes"
                || value == "true";
        }

        public static void Assert(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception("Assertion failed. Condition is false." + (string.IsNullOrWhiteSpace(message) ? string.Empty : " " + message));
            }
        }
        public static List<string[]> ReadCSVFile(string file, string separator = ";", bool ignoreFirstLine = false)
        {
            return ReadCSVFile(file, new UTF8Encoding(false), separator, ignoreFirstLine);
        }
        public static List<string[]> ReadCSVFile(string file, Encoding encoding, string separator = ";", bool ignoreFirstLine = false)
        {
            List<string[]> outterList = new List<string[]>();
            string[] lines = File.ReadAllLines(file, encoding);
            for (int i = 0; i < lines.Length; i++)
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
                        outterList.Add(innerList.ToArray());
                    }
                }
            }
            return outterList;
        }
        /// <summary>
        /// Executes <paramref name="action"/>. When <paramref name="action"/> longer takes than <paramref name="timeout"/> then <paramref name="action"/> will be aborted.
        /// </summary>
        public static bool RunWithTimeout(this ThreadStart action, TimeSpan timeout)
        {
            Thread workerThread = new Thread(action);
            workerThread.Start();
            bool terminatedInGivenTimeSpan = workerThread.Join(timeout);
            if (!terminatedInGivenTimeSpan)
            {
                workerThread.Interrupt();
            }
            return terminatedInGivenTimeSpan;
        }

        public static string ResolveToFullPath(this string path)
        {
            return ResolveToFullPath(path, Directory.GetCurrentDirectory());
        }
        /// <summary>
        /// This function transforms <paramref name="path"/> into an absolute path.
        /// It does not matter if you pass a relative or absolute path: This function checks that.
        /// </summary>
        /// <returns>
        /// Returns an absolute path.
        /// </returns>
        public static string ResolveToFullPath(this string path, string baseDirectory)
        {
            path = path.Trim();
            if (IsAbsolutePath(path))
            {
                return path;
            }
            else
            {
                return Path.GetFullPath(new Uri(Path.Combine(baseDirectory, path)).LocalPath);
            }
        }
        /// <summary>
        /// This function takes a given <paramref name="xml"/>-string and validates it against a given <paramref name="xsd"/>-string.
        /// </summary>
        /// <returns>
        /// This function returns true if and only if <paramref name="errorMessages"/> is empty.
        /// If this function returns true it means, that <paramref name="xml"/> is structured according to <paramref name="xsd"/>.
        /// </returns>
        public static bool ValidateXMLAgainstXSD(string xml, string xsd, out IList<object> errorMessages)
        {
            errorMessages = new List<object>();
            try
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XmlReader.Create(new StringReader(xsd)));
                XDocument xDocument = XDocument.Parse(xml);

                List<object> events = new List<object>();
                xDocument.Validate(schemaSet, (o, eventArgument) =>
                {
                    events.Add(eventArgument);
                });
                foreach (object @event in events)
                {
                    errorMessages.Add(@event);
                }
            }
            catch (Exception exception)
            {
                errorMessages.Add(exception);
            }
            return errorMessages.Count == 0;
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
            using StringWriter stringWriter = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, applyXSLTToXMLXMLWriterDefaultSettings))
            {
                myXslTrans.Transform(XmlReader.Create(new StringReader(xml)), xmlWriter);

            }
            return xmlDeclaration + stringWriter.ToString();
        }
        public static readonly Encoding FormatXMLFile_DefaultEncoding = new UTF8Encoding(false);
        public static readonly XmlWriterSettings FormatXMLFile_DefaultXmlWriterSettings = new XmlWriterSettings() { Indent = true, IndentChars = "    " };
        public static void FormatXMLFile(string file)
        {
            FormatXMLFile(file, FormatXMLFile_DefaultEncoding, FormatXMLFile_DefaultXmlWriterSettings);
        }
        public static void FormatXMLFile(string file, Encoding encoding)
        {
            FormatXMLFile(file, encoding, FormatXMLFile_DefaultXmlWriterSettings);
        }
        public static void FormatXMLFile(string file, XmlWriterSettings settings)
        {
            FormatXMLFile(file, FormatXMLFile_DefaultEncoding, settings);
        }
        public static void FormatXMLFile(string file, Encoding encoding, XmlWriterSettings settings)
        {
            File.WriteAllText(file, FormatXMLString(File.ReadAllText(file), settings), encoding);
        }
        public static string FormatXMLString(string xmlString)
        {
            return FormatXMLString(xmlString, FormatXMLFile_DefaultXmlWriterSettings);
        }
        public static string FormatXMLString(string xmlString, XmlWriterSettings settings)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            xmlWriter.Flush();
            memoryStream.Flush();
            memoryStream.Position = 0;
            using StreamReader streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }
        public static string XmlToString(XmlDocument xmlDocument)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(stringBuilder, new XmlWriterSettings
            {
                Encoding = FormatXMLFile_DefaultEncoding,
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = false,
                NewLineChars = Environment.NewLine
            }))
                xmlDocument.Save(writer);
            return stringBuilder.ToString();
        }
        public static void AddMountPointForVolume(Guid volumeId, string mountPoint)
        {
            if (mountPoint.Length > 3)
            {
                EnsureDirectoryExists(mountPoint);
            }
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("mountvol", $"{mountPoint} \\\\?\\Volume{{{volumeId}}}\\")
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
        }
        public static ISet<Guid> GetAvailableVolumeIds()
        {
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("mountvol", string.Empty)
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
            HashSet<Guid> result = new HashSet<Guid>();
            for (int i = 0; i < externalProgramExecutor.AllStdOutLines.Length - 1; i++)
            {
                string rawLine = externalProgramExecutor.AllStdOutLines[i];
                try
                {
                    string line = rawLine.Trim(); //line looks like "\\?\Volume{80aa12de-7392-4051-8cd2-f28bf56dc9d3}\"
                    string prefix = @"\\?\Volume{";
                    if (line.StartsWith(prefix))
                    {
                        line = line[prefix.Length..];//remove "\\?\Volume{"
                        line = line[0..^2];//remove "}\"
                        string nextLine = externalProgramExecutor.AllStdOutLines[i + 1].Trim();
                        if (Directory.Exists(nextLine) || nextLine.StartsWith("***"))
                        {
                            result.Add(Guid.Parse(line));
                        }
                    }
                }
                catch
                {
                    NoOperation();
                }
            }
            return result;
        }
        public static ISet<string> GetAllMountPointsOfAllAvailableVolumes()
        {
            HashSet<string> result = new HashSet<string>();
            foreach (Guid volumeId in GetAvailableVolumeIds())
            {
                result.UnionWith(GetMountPoints(volumeId));
            }
            return result;
        }
        public static ISet<string> GetMountPoints(Guid volumeId)
        {
            HashSet<string> result = new HashSet<string>();
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("mountvol", string.Empty)
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
            for (int i = 0; i < externalProgramExecutor.AllStdOutLines.Length; i++)
            {
                string line = externalProgramExecutor.AllStdOutLines[i].Trim();
                if (line.StartsWith($"\\\\?\\Volume{{{volumeId}}}\\"))
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
        public static void RemoveAllMountPointsOfVolume(Guid volumeId)
        {
            foreach (string mountPoint in GetMountPoints(volumeId))
            {
                RemoveMountPointOfVolume(mountPoint);
            }
        }
        public static void RemoveMountPointOfVolume(string mountPoint)
        {
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("mountvol", $"{mountPoint} /d")
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:{Environment.NewLine}" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
            if (mountPoint.Length > 3)
            {
                EnsureDirectoryDoesNotExist(mountPoint);
            }
        }
        public static Guid GetVolumeIdByMountPoint(string mountPoint)
        {
            if (!mountPoint.EndsWith("\\"))
            {
                mountPoint += "\\";
            }
            foreach (Guid volumeId in GetAvailableVolumeIds())
            {
                foreach (string currentMountPoint in GetMountPoints(volumeId))
                {
                    if (currentMountPoint.Equals(mountPoint))
                    {
                        return volumeId;
                    }
                }
            }
            throw new KeyNotFoundException($"No volume could be found which provides the volume accessible at {mountPoint}");
        }


        public static T[] PadLeft<T>(T[] array, int length)
        {
            return PadLeft(array, default, length);
        }
        public static T[] PadLeft<T>(T[] array, T fillItem, int length)
        {
            return PadHelper(array, length, fillItem, true);
        }
        public static T[] PadRight<T>(T[] array, int length)
        {
            return PadRight(array, default, length);
        }
        public static T[] PadRight<T>(T[] array, T fillItem, int length)
        {
            return PadHelper(array, length, fillItem, false);
        }
        private static T[] PadHelper<T>(T[] array, int length, T fillItem, bool PadLeft)
        {
            T[] result = array;
            while (array.Length <= length)
            {
                if (PadLeft)
                {
                    Concat(new T[] { fillItem }, result);
                }
                else
                {
                    Concat(result, new T[] { fillItem });
                }
            }
            return result;
        }
        /// <param name="value">
        /// must contain exacltly 4 bytes.
        /// </param>
        public static uint ByteArrayToUnsignedInteger32Bit(byte[] value, Endianness endianness = Endianness.BigEndian)
        {
            if (value.Length != 4)
            {
                throw new ArgumentException();
            }
            if (endianness == Endianness.BigEndian)
            {
                return (((uint)value[0]) << 24)
                     + (((uint)value[1]) << 16)
                     + (((uint)value[2]) << 08)
                     + (((uint)value[3]) << 00);
            }
            if (endianness == Endianness.MixedEndian)
            {
                return (((uint)value[1]) << 24)
                     + (((uint)value[0]) << 16)
                     + (((uint)value[3]) << 08)
                     + (((uint)value[2]) << 00);
            }
            if (endianness == Endianness.LittleEndian)
            {
                return (((uint)value[4]) << 24)
                     + (((uint)value[3]) << 16)
                     + (((uint)value[2]) << 08)
                     + (((uint)value[1]) << 00);
            }
            throw new ArgumentException($"Unknown or unsupported value given for parameter {nameof(endianness)}");
        }
        /// <returns>
        /// Returns an array with exactly 4 bytes.
        /// </returns>
        public static byte[] UnsignedInteger32BitToByteArray(uint value, Endianness endianness = Endianness.BigEndian)
        {
            byte[] result = new byte[4];
            if (endianness == Endianness.BigEndian)
            {
                result[0] = (byte)((value & 0xff000000) >> 24);
                result[1] = (byte)((value & 0x00ff0000) >> 16);
                result[2] = (byte)((value & 0x0000ff00) >> 08);
                result[3] = (byte)((value & 0x000000ff) >> 00);
                return result;
            }
            if (endianness == Endianness.MixedEndian)
            {
                result[0] = (byte)((value & 0x00ff0000) >> 24);
                result[1] = (byte)((value & 0xff000000) >> 16);
                result[2] = (byte)((value & 0x000000ff) >> 08);
                result[3] = (byte)((value & 0x0000ff00) >> 00);
                return result;
            }
            if (endianness == Endianness.LittleEndian)
            {
                result[0] = (byte)((value & 0x000000ff) >> 24);
                result[1] = (byte)((value & 0x0000ff00) >> 16);
                result[2] = (byte)((value & 0x00ff0000) >> 08);
                result[3] = (byte)((value & 0xff000000) >> 00);
                return result;
            }
            throw new ArgumentException($"Unknown or unsupported value given for parameter {nameof(endianness)}");
        }
        /// <param name="value">
        /// must contain exacltly 8 bytes.
        /// </param>
        public static ulong ByteArrayToUnsignedInteger64Bit(byte[] value, Endianness endianness = Endianness.BigEndian)
        {
            if (value.Length != 8)
            {
                throw new ArgumentException();
            }
            if (endianness == Endianness.BigEndian)
            {
                return (((ulong)value[0]) << 56)
                     + (((ulong)value[1]) << 48)
                     + (((ulong)value[2]) << 40)
                     + (((ulong)value[3]) << 32)
                     + (((ulong)value[4]) << 24)
                     + (((ulong)value[5]) << 16)
                     + (((ulong)value[6]) << 08)
                     + (((ulong)value[7]) << 00);
            }
            if (endianness == Endianness.MixedEndian)
            {
                return (((ulong)value[1]) << 56)
                     + (((ulong)value[0]) << 48)
                     + (((ulong)value[3]) << 40)
                     + (((ulong)value[2]) << 32)
                     + (((ulong)value[5]) << 24)
                     + (((ulong)value[4]) << 16)
                     + (((ulong)value[7]) << 08)
                     + (((ulong)value[6]) << 00);
            }
            if (endianness == Endianness.LittleEndian)
            {
                return (((ulong)value[7]) << 56)
                     + (((ulong)value[6]) << 48)
                     + (((ulong)value[5]) << 40)
                     + (((ulong)value[4]) << 32)
                     + (((ulong)value[3]) << 24)
                     + (((ulong)value[2]) << 16)
                     + (((ulong)value[1]) << 08)
                     + (((ulong)value[0]) << 00);
            }
            throw new ArgumentException($"Unknown or unsupported value given for parameter {nameof(endianness)}");
        }
        /// <returns>
        /// Returns an array with exactly 8 bytes.
        /// </returns>
        public static byte[] UnsignedInteger64BitToByteArray(ulong value, Endianness endianness = Endianness.BigEndian)
        {
            byte[] result = new byte[8];
            if (endianness == Endianness.BigEndian)
            {
                result[0] = (byte)((value & 0xff00000000000000) >> 56);
                result[1] = (byte)((value & 0x00ff000000000000) >> 48);
                result[2] = (byte)((value & 0x0000ff0000000000) >> 40);
                result[3] = (byte)((value & 0x000000ff00000000) >> 32);
                result[4] = (byte)((value & 0x00000000ff000000) >> 24);
                result[5] = (byte)((value & 0x0000000000ff0000) >> 16);
                result[6] = (byte)((value & 0x000000000000ff00) >> 08);
                result[7] = (byte)((value & 0x00000000000000ff) >> 00);
                return result;
            }
            if (endianness == Endianness.MixedEndian)
            {
                result[0] = (byte)((value & 0x00ff000000000000) >> 56);
                result[1] = (byte)((value & 0xff00000000000000) >> 48);
                result[2] = (byte)((value & 0x000000ff00000000) >> 40);
                result[3] = (byte)((value & 0x0000ff0000000000) >> 32);
                result[4] = (byte)((value & 0x0000000000ff0000) >> 24);
                result[5] = (byte)((value & 0x00000000ff000000) >> 16);
                result[6] = (byte)((value & 0x00000000000000ff) >> 08);
                result[7] = (byte)((value & 0x000000000000ff00) >> 00);
                return result;
            }
            if (endianness == Endianness.LittleEndian)
            {
                result[0] = (byte)((value & 0x00000000000000ff) >> 56);
                result[1] = (byte)((value & 0x000000000000ff00) >> 48);
                result[2] = (byte)((value & 0x0000000000ff0000) >> 40);
                result[3] = (byte)((value & 0x00000000ff000000) >> 32);
                result[4] = (byte)((value & 0x000000ff00000000) >> 24);
                result[5] = (byte)((value & 0x0000ff0000000000) >> 16);
                result[6] = (byte)((value & 0x00ff000000000000) >> 08);
                result[7] = (byte)((value & 0xff00000000000000) >> 00);
                return result;
            }
            throw new ArgumentException($"Unknown or unsupported value given for parameter {nameof(endianness)}");
        }
        public enum Endianness
        {
            BigEndian = 0,
            MixedEndian = 1,
            LittleEndian = 2,
        }
        public static bool NullSafeSetEquals<T>(this ISet<T> @this, ISet<T> obj)
        {
            return NullSafeHelper(@this, obj, (obj1, obj2) => obj1.SetEquals(obj2));
        }
        public static bool NullSafeListEquals<T>(this IList<T> @this, IList<T> obj)
        {
            return NullSafeHelper(@this, obj, (obj1, obj2) => obj1.SequenceEqual(obj2));
        }
        public static bool NullSafeEnumerableEquals<T>(this IEnumerable<T> @this, IEnumerable<T> obj)
        {
            return NullSafeHelper(@this, obj, (obj1, obj2) =>
            {
                if (obj1.Count() != obj2.Count())
                {
                    return false;
                }
                List<T> obj1Copy = new List<T>(obj1);
                List<T> obj2Copy = new List<T>(obj2);
                for (int i = 0; i < obj1.Count(); i++)
                {
                    if (!RemoveItemOnlyOnce(obj2Copy, obj1Copy[i]))
                    {
                        return false;
                    }
                }
                return true;
            });
        }
        public static bool NullSafeEquals(this object @this, object obj)
        {
            return NullSafeHelper(@this, obj, (obj1, obj2) => obj1.Equals(obj2));
        }
        private static bool NullSafeHelper<T>(T object1, T object2, Func<T, T, bool> f)
        {
            bool thisIsNull = object1 == null;
            bool objIsNull = object2 == null;
            if (thisIsNull ^ objIsNull)
            {
                return false;
            }
            else
            {
                if (thisIsNull && objIsNull)
                {
                    return true;
                }
                else
                {
                    return f(object1, object2);
                }
            }
        }
        public static bool RemoveItemOnlyOnce<T>(this IList<T> list, T item)
        {
            int index = list.IndexOf(item);
            if (index > -1)
            {
                list.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static DateTime GetTimeFromInternetUtC()
        {
            return GetTimeFromInternet(TimeZoneInfo.Utc);
        }
        public static DateTime GetTimeFromInternetCurrentTimeZone()
        {
            return GetTimeFromInternet(TimeZoneInfo.Local);
        }
        public static DateTime GetTimeFromInternet(TimeZoneInfo timezone)
        {
            return GetTimeFromInternet(timezone, "yy-MM-dd HH:mm:ss", "time.nist.gov", 13, 7, 17);
        }
        public static DateTime GetTimeFromInternet(TimeZoneInfo timezone, string format, string domain, int port, int begin, int length)
        {
            using StreamReader streamReader = new StreamReader(new TcpClient(domain, port).GetStream());
            DateTime originalDateTime = DateTime.ParseExact(streamReader.ReadToEnd().Substring(begin, length), format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return TimeZoneInfo.ConvertTime(originalDateTime, timezone);
        }

        public static SerializableDictionary<TKey, TValue> ToSerializableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            SerializableDictionary<TKey, TValue> result = new SerializableDictionary<TKey, TValue>();
            foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }

        public static bool IsDefault(object @object)
        {
            if (@object == null)
            {
                return true;
            }
            else
            {
                return EqualityComparer<object>.Default.Equals(@object, GetDefault(@object.GetType()));
            }
        }
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
        public static void ResolvePathOfProgram(ref string program, ref string argument)
        {
            if (File.Exists(program))
            {
                string resultProgram;
                string resultArgument;
                if (FileIsExecutable(program))
                {
                    resultProgram = program;
                    resultArgument = argument;
                }
                else
                {
                    if (OperatingSystem.OperatingSystem.GetCurrentOperatingSystem() is Windows)
                    {
                        resultProgram = GetDefaultProgramToOpenFile(Path.GetExtension(program));
                        resultArgument = program;
                    }
                    else
                    {
                        resultProgram = program;
                        resultArgument = argument;
                    }
                }
                program = resultProgram;
                argument = resultArgument;
                return;
            }
            if (!(program.Contains("/") || program.Contains("\\") || program.Contains(":")))
            {
                if (TryResolvePathByPathVariable(program, out string programWithFullPath))
                {
                    program = programWithFullPath;
                    return;
                }
            }
            throw new FileNotFoundException($"Program '{program}' can not be found");
        }

        public static string GetAssertionFailMessage(object expectedObject, object actualObject, int maxLengthPerObject = 1000)
        {
            return $"Equal failed. Expected: <{Environment.NewLine}{Generic.GenericToString(expectedObject, maxLengthPerObject)}{Environment.NewLine}> Actual: <{Environment.NewLine}{Generic.GenericToString(actualObject, maxLengthPerObject)}{Environment.NewLine}>";
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            foreach (object item in source)
            {
                action(item);
            }
        }


        public static bool ImprovedReferenceEquals(object item1, object item2)
        {
            bool itemHasValueType = HasValueType(item1);
            if (itemHasValueType != HasValueType(item2))
            {
                return false;
            }
            bool item1IsDefault = IsDefault(item1);
            bool item2IsDefault = IsDefault(item2);
            if (item1IsDefault && item2IsDefault)
            {
                return true;
            }
            if (item1IsDefault && !item2IsDefault)
            {
                return false;
            }
            if (!item1IsDefault && item2IsDefault)
            {
                return false;
            }
            if (!item1IsDefault && !item2IsDefault)
            {
                if (itemHasValueType)
                {
                    Type type = item1.GetType();
                    if (type.Equals(item2.GetType()))//TODO ignore generics here when type is keyvaluepair
                    {
                        if (TypeIsKeyValuePair(type))
                        {
                            System.Collections.Generic.KeyValuePair<object, object> kvp1 = ObjectToKeyValuePairUnsafe<object, object>(item1);
                            System.Collections.Generic.KeyValuePair<object, object> kvp2 = ObjectToKeyValuePairUnsafe<object, object>(item2);
                            return ImprovedReferenceEquals(kvp1.Key, kvp2.Key) && ImprovedReferenceEquals(kvp1.Value, kvp2.Value);
                        }
                        else
                        {
                            return item1.Equals(item2);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return ReferenceEquals(item1, item2);
                }
            }
            throw new ArgumentException("Can not calculate reference-equals for the given arguments.");
        }

        public static bool HasValueType(object @object)
        {
            if (@object == null)
            {
                return false;
            }
            else
            {
                return @object.GetType().IsValueType;
            }
        }

        public static string GetNameOfCurrentExecutable()
        {
            return Process.GetCurrentProcess().ProcessName;
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
            return input.First().ToString().ToUpper() + input[1..].ToLower();
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
        public static string ToPascalCase(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            IEnumerable<string> words = input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(word => word.Substring(0, 1).ToUpper() +
                                         word[1..].ToLower());

            return string.Concat(words);
        }
        public static string ToCamelCase(this string input)
        {
            string pascalCase = input.ToPascalCase();
            return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
        }

        private static readonly Regex _OneOrMoreHexSigns = new Regex(@"^[0-9a-f]+$");
        public static bool IsHexString(string result)
        {
            return _OneOrMoreHexSigns.Match(result.ToLower()).Success;
        }
        public static bool IsHexDigit(this char @char)
        {
            return (@char >= '0' && @char <= '9') || (@char >= 'a' && @char <= 'f') || (@char >= 'A' && @char <= 'F');
        }

        public static bool DarkModeEnabled
        {
            get
            {
                return OperatingSystem.OperatingSystem.GetCurrentOperatingSystem().Accept(_DarkModeEnabledVisitor);
            }
            set
            {
                OperatingSystem.OperatingSystem.GetCurrentOperatingSystem().Accept(new SetDarkModeEnabledVisitor(value));
            }
        }
        public static (IObservable<T>, Action) FuncToObservable<T>(Func<T> valueFunction, TimeSpan updateInterval)
        {
            Subject<T> subject = new Subject<T>();
            bool enabled = true;
            SupervisedThread thread = SupervisedThread.Create(() =>
            {
                while (enabled)
                {
                    try
                    {
                        Thread.Sleep(updateInterval);
                        if (subject.HasObservers)
                        {
                            subject.OnNext(valueFunction());
                        }
                    }
                    catch
                    {
                        NoOperation();
                    }
                }
                subject.OnCompleted();
                subject.Dispose();
            });
            thread.Start();
            return (subject.AsObservable().DistinctUntilChanged(), () => enabled = false);
        }
        private static readonly IOperatingSystemVisitor<bool> _DarkModeEnabledVisitor = new GetDarkModeEnabledVisitor();
        private class SetDarkModeEnabledVisitor : IOperatingSystemVisitor
        {
            private readonly bool _Enabled;

            public SetDarkModeEnabledVisitor(bool enabled)
            {
                this._Enabled = enabled;
            }

            public void Handle(OSX operatingSystem)
            {
                throw new NotImplementedException();
            }

            public void Handle(Windows operatingSystem)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

                    key.SetValue("AppsUseLightTheme", _Enabled ? 0 : 1);
                }
            }

            public void Handle(Linux operatingSystem)
            {
                throw new NotImplementedException();
            }
        }
        private class GetDarkModeEnabledVisitor : IOperatingSystemVisitor<bool>
        {
            public bool Handle(OSX operatingSystem)
            {
                throw new NotSupportedException();
            }

            public bool Handle(Windows operatingSystem)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                        return ((int)key.GetValue("AppsUseLightTheme")) == 0;
                    }
                    catch
                    {
                        NoOperation();
                    }
                }
                return false;
            }

            public bool Handle(Linux operatingSystem)
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region Git
        public static GitCommandResult ExecuteGitCommand(string repositoryFolder, string argument, bool throwErrorIfExitCodeIsNotZero = false, int? timeoutInMilliseconds = null, bool printErrorsAsInformation = false, bool writeOutputToConsole = false)
        {
            using GRYLog log = GRYLog.Create();
            log.Configuration.Enabled = true;
            log.Configuration.SetEnabledOfAllLogTargets(writeOutputToConsole);
            using ExternalProgramExecutor externalProgramExecutor = new ExternalProgramExecutor("git", argument, repositoryFolder)
            {
                LogObject = log,
                TimeoutInMilliseconds = timeoutInMilliseconds,
                PrintErrorsAsInformation = printErrorsAsInformation,
                ThrowErrorIfExitCodeIsNotZero = throwErrorIfExitCodeIsNotZero
            };
            externalProgramExecutor.StartSynchronously();
            return new GitCommandResult(argument, repositoryFolder, externalProgramExecutor.AllStdOutLines, externalProgramExecutor.AllStdErrLines, externalProgramExecutor.ExitCode);
        }
        /// <returns>
        /// Returns a enumeration of the submodule-paths of <paramref name="repositoryFolder"/>.
        /// </returns>
        public static IEnumerable<string> GetGitSubmodulePaths(string repositoryFolder, bool recursive = true)
        {
            GitCommandResult commandresult = ExecuteGitCommand(repositoryFolder, "submodule status" + (recursive ? " --recursive" : string.Empty), true);
            List<string> result = new List<string>();
            foreach (string rawLine in commandresult.StdOutLines)
            {
                string line = rawLine.Trim();
                if (line.Contains(" "))
                {
                    string[] splitted = line.Split(' ');
                    int amountOfWhitespaces = splitted.Length - 1;
                    if (0 < amountOfWhitespaces)
                    {
                        string rawPath = splitted[1];
                        if (rawPath.Contains("..") || rawPath == "./")
                        {
                            continue;
                        }
                        else
                        {
                            result.Add(Path.Combine(repositoryFolder, rawPath.Replace("/", Path.DirectorySeparatorChar.ToString())));
                        }
                    }
                }
            }
            return result;
        }
        public static bool GitRepositoryContainsObligatoryFiles(string repositoryFolder, out ISet<string> missingFiles)
        {
            List<Tuple<string, ISet<string>>> fileLists = new List<Tuple<string/*file*/, ISet<string>/*aliase*/>>
            {
                Tuple.Create<string, ISet<string>>(".gitignore", new HashSet<string>()),
                Tuple.Create<string, ISet<string>>("License.txt", new HashSet<string>() { "License", "License.md" }),
                Tuple.Create<string, ISet<string>>("ReadMe.md", new HashSet<string>() { "ReadMe", "ReadMe.txt" })
            };
            return GitRepositoryContainsFiles(repositoryFolder, out missingFiles, fileLists);
        }
        public static void AddGitRemote(string repositoryFolder, string remoteFolder, string remoteName)
        {
            ExecuteGitCommand(repositoryFolder, $"remote add {remoteName} \"{remoteFolder}\"", true);
        }
        public static bool GitRemoteIsAvailable(string repositoryFolder, string remoteName)
        {
            try
            {
                return ExecuteGitCommand(repositoryFolder, $"ls-remote {remoteName}", false, 1000 * 60, writeOutputToConsole: false).ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
        /// <returns>Returns the address of the remote with the given <paramref name="remoteName"/>.</returns>
        public static string GetGitRemoteAddress(string repository, string remoteName)
        {
            return ExtractTextFromOutput(ExecuteGitCommand(repository, $"config --get remote.{remoteName}.url", true).StdOutLines);
        }
        public static void SetGitRemoteAddress(string repositoryFolder, string remoteName, string newRemoteAddress)
        {
            ExecuteGitCommand(repositoryFolder, $"remote set-url {remoteName} {newRemoteAddress}", true);
        }
        /// <summary>Removes unused internal files in the .git-folder of the given <paramref name="repositoryFolder"/>.</summary>
        /// <remarks>Warning: After executing this function deleted commits can not be restored because then they are really deleted.</remarks>
        public static void GitTidyUp(string repositoryFolder, bool writeOutputToConsole = false)
        {
            ExecuteGitCommand(repositoryFolder, $"reflog expire --expire-unreachable=now --all", true, writeOutputToConsole: writeOutputToConsole);
            ExecuteGitCommand(repositoryFolder, $"gc --prune=now", true, writeOutputToConsole: writeOutputToConsole);
        }
        public static bool GitRepositoryContainsFiles(string repositoryFolder, out ISet<string> missingFiles, IEnumerable<Tuple<string/*file*/, ISet<string>/*aliase*/>> fileLists)
        {
            missingFiles = new HashSet<string>();
            foreach (Tuple<string, ISet<string>> file in fileLists)
            {
                if (!(File.Exists(Path.Combine(repositoryFolder, file.Item1)) || AtLeastOneFileExistsInFolder(repositoryFolder, file.Item2, out string _)))
                {
                    missingFiles.Add(file.Item1);
                }
            }
            return missingFiles.Count == 0;
        }
        /// <returns>
        /// Returns the names of all remotes of the given <paramref name="repositoryFolder"/>.
        /// </returns>
        /// <remarks>
        /// This function does not return the addresses of these remotes.
        /// </remarks>
        public static IEnumerable<string> GetAllGitRemotes(string repositoryFolder)
        {
            return ExecuteGitCommand(repositoryFolder, "remote", true).StdOutLines.Where(line => !string.IsNullOrWhiteSpace(line));
        }
        public static bool AtLeastOneFileExistsInFolder(string repositoryFolder, IEnumerable<string> files, out string foundFile)
        {
            foreach (string file in files)
            {
                if (File.Exists(Path.Combine(repositoryFolder, file)))
                {
                    foundFile = file;
                    return true;
                }
            }
            foundFile = null;
            return false;
        }
        /// <returns>
        /// Returns a tuple.
        /// tuple.Item1 represents the remote-name.
        /// tuple.Item1 represents the remote-branchname.
        /// </returns>
        public static IEnumerable<Tuple<string/*remote-name*/, string/*branch-name*/>> GetAllGitRemoteBranches(string repository)
        {
            return ExecuteGitCommand(repository, "branch -r", true).StdOutLines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line =>
            {
                if (line.Contains("/"))
                {
                    string[] splitted = line.Split(new[] { '/' }, 2);
                    return new Tuple<string, string>(splitted[0].Trim(), splitted[1].Trim());
                }
                else
                {
                    throw new Exception($"'{repository}> git branch -r' contained the unexpected output-line '{line}'.");
                }
            });
        }
        /// <returns>Returns the names of the remotes of the given <paramref name="repositoryFolder"/>.</returns>
        public static IEnumerable<string> GetGitRemotes(string repositoryFolder)
        {
            return ExecuteGitCommand(repositoryFolder, "remote", true).StdOutLines.Where(line => !string.IsNullOrWhiteSpace(line));
        }
        public static void RemoveGitRemote(string repositoryFolder, string remote)
        {
            ExecuteGitCommand(repositoryFolder, $"remote remove {remote}", true);
        }
        public static IEnumerable<string> GetLocalGitBranchNames(string repositoryFolder)
        {
            return ExecuteGitCommand(repositoryFolder, "branch", true).StdOutLines.Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.Replace("*", string.Empty).Trim());
        }
        /// <returns>Returns the toplevel of the <paramref name="repositoryFolder"/>.</returns>
        public static string GetTopLevelOfGitRepositoryPath(string repositoryFolder)
        {
            if (IsInGitRepository(repositoryFolder))
            {
                return ExtractTextFromOutput(ExecuteGitCommand(repositoryFolder, "rev-parse --show-toplevel", true).StdOutLines);
            }
            else
            {
                throw new ArgumentException($"The given folder '{repositoryFolder}' is not a git-repository.");
            }
        }
        private static string ExtractTextFromOutput(string[] lines)
        {
            return string.Join(string.Empty, lines).Trim();
        }
        /// <returns>Returns true if and only if <paramref name="repositoryFolder"/> is in a repository which is used as submodule.</returns>
        public static bool IsInGitSubmodule(string repositoryFolder)
        {
            if (IsInGitRepository(repositoryFolder))
            {
                return !GetParentGitRepositoryPathHelper(repositoryFolder).Equals(string.Empty);
            }
            else
            {
                return false;
            }
        }

        /// <returns>
        /// If <paramref name="repositoryFolder"/> is used as submodule then this function returns the toplevel-folder of the parent-repository.
        /// </returns>
        public static string GetParentGitRepositoryPath(string repositoryFolder)
        {
            if (IsInGitRepository(repositoryFolder))
            {
                string content = GetParentGitRepositoryPathHelper(repositoryFolder);
                if (string.IsNullOrEmpty(content))
                {
                    throw new ArgumentException($"The given folder '{repositoryFolder}' is not used as submodule so a parent-repository-path can not be calculated.");
                }
                else
                {
                    return content;
                }
            }
            else
            {
                throw new ArgumentException($"The given folder '{repositoryFolder}' is not a git-repository.");
            }
        }
        private static string GetParentGitRepositoryPathHelper(string repositoryFolder)
        {
            return ExtractTextFromOutput(ExecuteGitCommand(repositoryFolder, "rev-parse --show-superproject-working-tree", true).StdOutLines);
        }
        /// <returns>
        /// Returns true if and only if <paramref name="folder"/> is the toplevel of a git-repository.
        /// </returns>
        public static bool IsGitRepository(string folder)
        {
            string combinedPath = Path.Combine(folder, ".git");
            return Directory.Exists(combinedPath) || File.Exists(combinedPath);
        }
        /// <returns>
        /// Returns true if and only if <paramref name="folder"/> or a parent-folder of <paramref name="folder"/> is a toplevel of a git-repository.
        /// </returns>
        public static bool IsInGitRepository(string folder)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);
            if (IsGitRepository(directoryInfo.FullName))
            {
                return true;
            }
            else if (directoryInfo.Parent == null)
            {
                return false;
            }
            else
            {
                return IsInGitRepository(directoryInfo.Parent.FullName);
            }
        }
        /// <summary>
        /// Commits all staged and unstaged changes in <paramref name="repositoryFolder"/>.
        /// </summary>
        /// <param name="repositoryFolder">Repository where changes should be committed</param>
        /// <param name="commitMessage">Message for the commit</param>
        /// <param name="commitWasCreated">Will be set to true if and only if really a commit was created. Will be set to false if and only if there are no changes to get committed.</param>
        /// <returns>Returns the commit-id of the currently checked out commit. This returns the id of the new created commit if there were changes which were committed by this function.</returns>
        /// <exception cref="UnexpectedExitCodeException">If there are uncommitted changes in submodules of <paramref name="repositoryFolder"/>.</exception>
        public static string GitCommit(string repositoryFolder, string commitMessage, out bool commitWasCreated, bool writeOutputToConsole = false)
        {
            commitWasCreated = false;
            if (GitRepositoryHasUncommittedChanges(repositoryFolder))
            {
                ExecuteGitCommand(repositoryFolder, $"add -A", true, writeOutputToConsole: writeOutputToConsole);
                ExecuteGitCommand(repositoryFolder, $"commit -m \"{commitMessage}\"", true, writeOutputToConsole: writeOutputToConsole);
                commitWasCreated = true;
            }
            return GetLastGitCommitId(repositoryFolder, "HEAD", writeOutputToConsole);
        }
        /// <returns>Returns the commit-id of the given <paramref name="revision"/>.</returns>
        public static string GetLastGitCommitId(string repositoryFolder, string revision = "HEAD", bool writeOutputToConsole = false)
        {
            return ExecuteGitCommand(repositoryFolder, $"rev-parse {revision}", true, writeOutputToConsole: writeOutputToConsole).GetFirstStdOutLine();
        }
        /// <param name="printErrorsAsInformation">
        /// Represents a value which indicates if the git-output which goes to stderr should be treated as stdout.
        /// The default-value is true since even if no error occurs git write usual information to stderr.
        /// If really an error occures (=the exit-code of git is not 0) then this function throws an exception
        /// </param>
        public static void GitFetch(string repositoryFolder, string remoteName = "--all", bool printErrorsAsInformation = true, bool writeOutputToConsole = false)
        {
            ExecuteGitCommand(repositoryFolder, $"fetch {remoteName} --tags --prune", true, printErrorsAsInformation: printErrorsAsInformation, writeOutputToConsole: writeOutputToConsole);
        }
        public static bool GitRepositoryHasUnstagedChanges(string repositoryFolder)
        {
            if (GitRepositoryHasUnstagedChangesOfTrackedFiles(repositoryFolder))
            {
                return true;
            }
            if (GitRepositoryHasNewUntrackedFiles(repositoryFolder))
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<string> GetFilesOfGitRepository(string repositoryFolder, string revision)
        {
            return ExecuteGitCommand(repositoryFolder, $"ls-tree --full-tree -r --name-only {revision}", true).StdOutLines;
        }

        public static bool GitRepositoryHasNewUntrackedFiles(string repositoryFolder)
        {
            return GitChangesHelper(repositoryFolder, "ls-files --exclude-standard --others");
        }

        public static bool GitRepositoryHasUnstagedChangesOfTrackedFiles(string repositoryFolder)
        {
            return GitChangesHelper(repositoryFolder, "diff");
        }

        public static bool GitRepositoryHasStagedChanges(string repositoryFolder)
        {
            return GitChangesHelper(repositoryFolder, "diff --cached");
        }

        private static bool GitChangesHelper(string repositoryFolder, string argument)
        {
            GitCommandResult result = ExecuteGitCommand(repositoryFolder, argument, true);
            foreach (string line in result.StdOutLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GitRepositoryHasUncommittedChanges(string repositoryFolder)
        {
            if (GitRepositoryHasUnstagedChanges(repositoryFolder))
            {
                return true;
            }
            if (GitRepositoryHasStagedChanges(repositoryFolder))
            {
                return true;
            }
            return false;
        }
        /// <remarks>
        /// <paramref name="revision"/> can be all kinds of revision-labels, for example "HEAD" or branch-names (e. g. "master") oder revision-ids (e. g. "a1b2c3b4").
        /// </remarks>
        public static int GetAmountOfCommitsInGitRepository(string repositoryFolder, string revision = "HEAD")
        {
            return int.Parse(ExecuteGitCommand(repositoryFolder, $"rev-list --count {revision}", true).GetFirstStdOutLine());
        }
        public static string GetCurrentGitRepositoryBranch(string repositoryFolder)
        {
            return ExecuteGitCommand(repositoryFolder, $"rev-parse --abbrev-ref HEAD", true).GetFirstStdOutLine();
        }
        /// <remarks>
        /// <paramref name="ancestor"/> and <paramref name="descendant"/> can be all kinds of revision-labels, for example "HEAD" or branch-names (e. g. "master") oder revision-ids (e. g. "a1b2c3b4").
        /// </remarks>
        public static bool IsGitCommitAncestor(string repositoryFolder, string ancestor, string descendant = "HEAD")
        {
            return ExecuteGitCommand(repositoryFolder, $"merge-base --is-ancestor {ancestor} {descendant}", false).ExitCode == 0;
        }
        #endregion

        #region Execute or open file
        public static bool FileIsExecutable(string file)
        {
            return OperatingSystem.OperatingSystem.GetCurrentOperatingSystem().Accept(new FileIsExecutableVisitor(file));
        }
        public static ExternalProgramExecutor ExecuteFile(string file)
        {
            if (FileIsExecutable(file))
            {
                using ExternalProgramExecutor result = new ExternalProgramExecutor(file, string.Empty);
                result.StartSynchronously();
                return result;
            }
            else
            {
                throw new Exception($"File '{file}' can not be executed");
            }
        }

        public static void OpenFileWithDefaultProgram(string file)
        {
            new ExternalProgramExecutor(file, string.Empty).StartAsynchronously();
        }
        private class FileIsExecutableVisitor : IOperatingSystemVisitor<bool>
        {
            private readonly string _File;

            public FileIsExecutableVisitor(string file)
            {
                this._File = file;
            }

            public bool Handle(OSX operatingSystem)
            {
                return true;
            }

            public bool Handle(Windows operatingSystem)
            {
                string fileToLower = this._File.ToLower();
                return fileToLower.EndsWith(".exe")
                    || fileToLower.EndsWith(".cmd")
                    || fileToLower.EndsWith(".bat");
            }

            public bool Handle(Linux operatingSystem)
            {
                return true;
            }
        }




        #endregion

        #region Enumerable

        #region IsEnumerable
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IEnumerable"/>.</returns>
        public static bool ObjectIsEnumerable(this object @object)
        {
            return @object is IEnumerable;
        }
        public static bool TypeIsEnumerable(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IEnumerable)) && !typeof(string).Equals(type);
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="ISet{T}"/>.</returns>
        public static bool ObjectIsSet(this object @object)
        {
            return TypeIsSet(@object.GetType());
        }
        public static bool TypeIsSet(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(ISet<>));
        }
        public static bool ObjectIsList(this object @object)
        {
            return TypeIsList(@object.GetType());
        }
        public static bool TypeIsList(this Type type)
        {
            return TypeIsListNotGeneric(type) || TypeIsListGeneric(type);
        }
        public static bool TypeIsListNotGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IList));
        }
        public static bool TypeIsListGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IList<>));
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IDictionary{TKey, TValue}"/> or <see cref="IDictionary"/>.</returns>
        public static bool ObjectIsDictionary(this object @object)
        {
            return TypeIsDictionary(@object.GetType());
        }
        public static void AddItemToEnumerable(object enumerable, object[] addMethodArgument)
        {
            enumerable.GetType().GetMethod("Add").Invoke(enumerable, addMethodArgument);
        }
        public static bool TypeIsDictionary(this Type type)
        {
            return TypeIsDictionaryNotGeneric(type) || TypeIsDictionaryGeneric(type);
        }
        public static bool TypeIsDictionaryNotGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IDictionary));
        }
        public static bool TypeIsDictionaryGeneric(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(IDictionary<,>));
        }
        public static bool ObjectIsKeyValuePair(this object @object)
        {
            return TypeIsKeyValuePair(@object.GetType());
        }
        public static bool TypeIsKeyValuePair(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(System.Collections.Generic.KeyValuePair<,>)) || TypeIsAssignableFrom(type, typeof(XMLSerializer.KeyValuePair<object, object>));
        }
        public static bool ObjectIsDictionaryEntry(this object @object)
        {
            return TypeIsDictionaryEntry(@object.GetType());
        }
        public static bool TypeIsDictionaryEntry(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(DictionaryEntry));
        }
        public static bool ObjectIsTuple(this object @object)
        {
            return TypeIsTuple(@object.GetType());
        }
        public static bool TypeIsTuple(this Type type)
        {
            return TypeIsAssignableFrom(type, typeof(Tuple<,>));
        }

        #endregion
        #region ToEnumerable
        public static IEnumerable ObjectToEnumerable(this object @object)
        {
            if (!ObjectIsEnumerable(@object))
            {
                throw new InvalidCastException();
            }
            return @object as IEnumerable;
        }
        public static IEnumerable<T> ObjectToEnumerable<T>(this object @object)
        {
            if (!ObjectIsEnumerable(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            List<T> result = new List<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        public static ISet<T> ObjectToSet<T>(this object @object)
        {
            if (!ObjectIsSet(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            HashSet<T> result = new HashSet<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        /// <returns>Returns true if and only if the most concrete type of <paramref name="object"/> implements <see cref="IList{T}"/> or <see cref="IList"/>.</returns>
        public static IList ObjectToList(this object @object)
        {
            return ObjectToList<object>(@object).ToList();
        }
        public static IList<T> ObjectToList<T>(this object @object)
        {
            if (!ObjectIsList(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable objects = ObjectToEnumerable(@object);
            List<T> result = new List<T>();
            foreach (object obj in objects)
            {
                if (obj is T t)
                {
                    result.Add(t);
                }
                else if (IsDefault(obj))
                {
                    result.Add(default);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            return result;
        }
        public static IDictionary ObjectToDictionary(this object @object)
        {
            IDictionary result = new Hashtable();
            foreach (System.Collections.Generic.KeyValuePair<object, object> item in ObjectToDictionary<object, object>(@object))
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
        public static IDictionary<TKey, TValue> ObjectToDictionary<TKey, TValue>(this object @object)
        {
            if (!ObjectIsDictionary(@object))
            {
                throw new InvalidCastException();
            }
            IEnumerable<object> objects = ObjectToEnumerable<object>(@object);
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            foreach (object obj in objects)
            {
                System.Collections.Generic.KeyValuePair<TKey, TValue> kvp = ObjectToKeyValuePair<TKey, TValue>(obj);
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
        public static System.Collections.Generic.KeyValuePair<TKey, TValue> ObjectToKeyValuePair<TKey, TValue>(this object @object)
        {
            if (!ObjectIsKeyValuePair(@object))
            {
                throw new InvalidCastException();
            }
            return ObjectToKeyValuePairUnsafe<TKey, TValue>(@object);
        }

        internal static System.Collections.Generic.KeyValuePair<TKey, TValue> ObjectToKeyValuePairUnsafe<TKey, TValue>(object @object)
        {
            object key = ((dynamic)@object).Key;
            object value = ((dynamic)@object).Value;
            TKey tKey;
            TValue tValue;

            if (key is TKey key1)
            {
                tKey = key1;
            }
            else if (IsDefault(key))
            {
                tKey = default;
            }
            else
            {
                throw new InvalidCastException();
            }
            if (value is TValue value1)
            {
                tValue = value1;
            }
            else if (IsDefault(value))
            {
                tValue = default;
            }
            else
            {
                throw new InvalidCastException();
            }
            return new System.Collections.Generic.KeyValuePair<TKey, TValue>(tKey, tValue);
        }

        public static DictionaryEntry ObjectToDictionaryEntry(object @object)
        {
            if (!ObjectIsDictionaryEntry(@object))
            {
                throw new InvalidCastException();
            }
            object key = ((dynamic)@object).Key;
            object value = ((dynamic)@object).Value;
            return new DictionaryEntry(key, value);
        }
        public static Tuple<T1, T2> ObjectToTuple<T1, T2>(this object @object)
        {
            if (!ObjectIsTuple(@object))
            {
                throw new InvalidCastException();
            }
            object item1 = ((dynamic)@object).Item1;
            object item2 = ((dynamic)@object).Item2;
            if (item1 is T1 t1 && item2 is T2 t2)
            {
                return new Tuple<T1, T2>(t1, t2);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        #endregion
        #region EqualsEnumerable
        public static bool EnumerableEquals(this IEnumerable enumerable1, IEnumerable enumerable2)
        {
            return new EnumerableComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(enumerable1, enumerable2);
        }
        /// <returns>Returns true if and only if the items in <paramref name="list1"/> and <paramref name="list2"/> are equal (ignoring the order) using the GRYLibrary-AdvancedObjectAnalysis for object-comparison.</returns>
        public static bool SetEquals<T>(this ISet<T> set1, ISet<T> set2)
        {
            return new SetComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(set1, set2);
        }
        public static bool ListEquals(this IList list1, IList list2)
        {
            return new ListComparer(new PropertyEqualsCalculatorConfiguration()).Equals(list1, list2);
        }
        /// <returns>Returns true if and only if the items in <paramref name="list1"/> and <paramref name="list2"/> are equal using the GRYLibrary-AdvancedObjectAnalysis for object-comparison.</returns>
        public static bool ListEquals<T>(this IList<T> list1, IList<T> list2)
        {
            return new ListComparer(new PropertyEqualsCalculatorConfiguration()).EqualsTyped(list1, list2);
        }
        public static bool DictionaryEquals(this IDictionary dictionary1, IDictionary dictionary2)
        {
            return new DictionaryComparer(new PropertyEqualsCalculatorConfiguration()).Equals(dictionary1, dictionary2);
        }
        public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            return new DictionaryComparer(new PropertyEqualsCalculatorConfiguration()).DefaultEquals(dictionary1, dictionary2);
        }
        public static bool KeyValuePairEquals<TKey, TValue>(this System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair1, System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair2)
        {
            return new KeyValuePairComparer(new PropertyEqualsCalculatorConfiguration()).Equals(keyValuePair1, keyValuePair2);
        }
        public static bool TupleEquals<TKey, TValue>(this Tuple<TKey, TValue> tuple1, Tuple<TKey, TValue> tuple2)
        {
            return new TupleComparer(new PropertyEqualsCalculatorConfiguration()).Equals(tuple1, tuple2);
        }

        #endregion
        #endregion

        #region Similarity
        public static PercentValue CalculateCombinedSimilarity(string string1, string string2)
        {
            return new PercentValue(CalculateCosineSimilarity(string1, string2).Value * (1 / 3)
                + CalculateJaccardSimilarity(string1, string2).Value * (1 / 3)
                + CalculateLevenshteinSimilarity(string1, string2).Value * (1 / 3));
        }
        public static int CalculateLevenshteinDistance(string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1) && string.IsNullOrEmpty(string2))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(string1))
            {
                return string2.Length;
            }
            if (string.IsNullOrEmpty(string2))
            {
                return string1.Length;
            }
            int lengthA = string1.Length;
            int lengthB = string2.Length;
            int[,] distance = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distance[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distance[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = string2[j - 1] == string1[i - 1] ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[lengthA, lengthB];
        }
        public static PercentValue CalculateLevenshteinSimilarity(string string1, string string2)
        {
            int levenshteinDistance = CalculateLevenshteinDistance(string1, string2);
            if (levenshteinDistance == 0)
            {
                return PercentValue.HundredPercent;
            }
            int maxLength = Math.Max(string1.Length, string2.Length);
            if (levenshteinDistance == maxLength)
            {
                return PercentValue.ZeroPercent;
            }
            else
            {
                return new PercentValue(1 - ((double)levenshteinDistance) / maxLength);
            }
        }
        public static PercentValue CalculateCosineSimilarity(string string1, string string2)
        {
            int length1 = string1.Length;
            int length2 = string2.Length;
            if ((length1 == 0 && length2 > 0) || (length2 == 0 && length1 > 0))
            {
                return PercentValue.ZeroPercent;
            }
            IDictionary<string, int> a = CalculateSimilarityHelperConvert(CalculateSimilarityHelperGetCharFrequencyMap(string1));
            IDictionary<string, int> b = CalculateSimilarityHelperConvert(CalculateSimilarityHelperGetCharFrequencyMap(string2));
            HashSet<string> intersection = CalculateSimilarityHelperGetIntersectionOfCharSet(a.Keys, b.Keys);
            double dotProduct = 0, magnitudeA = 0, magnitudeB = 0;
            foreach (string item in intersection)
            {
                dotProduct += a[item] * b[item];
            }
            foreach (string k in a.Keys)
            {
                magnitudeA += Math.Pow(a[k], 2);
            }
            foreach (string k in b.Keys)
            {
                magnitudeB += Math.Pow(b[k], 2);
            }
            return new PercentValue(dotProduct / Math.Sqrt(magnitudeA * magnitudeB));
        }
        public static double CalculateJaccardIndex(string string1, string string2)
        {
            return CalculateSimilarityHelperGetIntersection(string1, string2).Count() / (double)CalculateSimilarityHelperGetUnion(string1, string2).Count();
        }
        public static PercentValue CalculateJaccardSimilarity(string string1, string string2)
        {
            return new PercentValue(CalculateJaccardIndex(string1, string2) * 2);
        }
        private static string CalculateSimilarityHelperGetIntersection(string string1, string string2)
        {
            IList<char> list = new List<char>();
            foreach (char character in string1)
            {
                if (string2.Contains(character))
                {
                    list.Add(character);
                }
            }
            string result = new string(list.ToArray());
            return result;
        }
        private static IDictionary<char, int> CalculateSimilarityHelperGetCharFrequencyMap(string str)
        {
            Dictionary<char, int> result = new Dictionary<char, int>();
            foreach (char chr in str)
            {
                if (result.ContainsKey(chr))
                {
                    result[chr] = result[chr] + 1;
                }
                else
                {
                    result.Add(chr, 1);
                }
            }
            return result;
        }
        private static string CalculateSimilarityHelperGetUnion(string string1, string string2)
        {
            return new string((string1 + string2).ToCharArray());
        }
        private static HashSet<string> CalculateSimilarityHelperGetIntersectionOfCharSet(ICollection<string> keys1, ICollection<string> keys2)
        {
            HashSet<string> result = new HashSet<string>();
            result.UnionWith(keys1);
            result.IntersectWith(keys2);
            return result;
        }
        private static IDictionary<string, int> CalculateSimilarityHelperConvert(IDictionary<char, int> dictionary)
        {
            IDictionary<string, int> result = new Dictionary<string, int>();
            foreach (System.Collections.Generic.KeyValuePair<char, int> obj in dictionary)
            {
                result.Add(obj.Key.ToString(), obj.Value);
            }
            return result;
        }
        #endregion

        #region Get file extension on windows
        private static string GetDefaultProgramToOpenFile(string extensionWithDot)
        {
            return FileExtentionInfo(AssocStr.Executable, extensionWithDot);
        }
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);
        private static string FileExtentionInfo(AssocStr assocStr, string extensionWithDot)
        {
#pragma warning disable CA1806 // Do not ignore method results
            uint pcchOut = 0;
            AssocQueryString(AssocF.Verify, assocStr, extensionWithDot, null, null, ref pcchOut);
            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            AssocQueryString(AssocF.Verify, assocStr, extensionWithDot, null, pszOut, ref pcchOut);
#pragma warning restore CA1806 // Do not ignore method results
            return pszOut.ToString();
        }

        public static (T[], T[]) Split<T>(T[] source, int index)
        {
            int len2 = source.Length - index;
            T[] first = new T[index];
            T[] last = new T[len2];
            Array.Copy(source, 0, first, 0, index);
            Array.Copy(source, index, last, 0, len2);
            return (first, last);
        }

        [Flags]
        private enum AssocF
        {
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
#pragma warning disable CA1069 // Enums values should not be duplicated
            Open_ByExeName = 0x2,
#pragma warning restore CA1069 // Enums values should not be duplicated
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200
        }

        private enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic
        }
        #endregion

    }
}
