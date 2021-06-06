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
using System.Reflection;
using System.Dynamic;
using GRYLibrary.Core.XMLSerializer;
using System.Net.Sockets;
using GRYLibrary.Core.OperatingSystem;
using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using System.Runtime.InteropServices;
using GRYLibrary.Core.LogObject;
using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static GRYLibrary.Core.Miscellaneous.TableGenerator;
using GRYLibrary.Core.Exceptions;

namespace GRYLibrary.Core.Miscellaneous
{
    public static class Utilities
    {
        #region Constants
        public const string EmptyString = "";
        public const string SpecialCharacterTestString = "<Special-character-Test: (^äöüß/\\$€\"\'+-*®¬¼😊👍✆⊆ℙ≈∑∞∫/𝄞𝄤𝅘𝅥𝅮) (您好) (Здравствуйте) (नमस्कार)>";
        #endregion

        public static (T[], T[]) Split<T>(T[] source, int index)
        {
            int len2 = source.Length - index;
            T[] first = new T[index];
            T[] last = new T[len2];
            Array.Copy(source, 0, first, 0, index);
            Array.Copy(source, index, last, 0, len2);
            return (first, last);
        }
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
        public static void Repeat<T>(this Action<uint> action, uint amountOfExecutions)
        {
            for (uint i = 0; i < amountOfExecutions; i++)
            {
                action(i);
            }
        }
        /// <summary>
        /// Checks if the given <paramref name="subList"/> is contained in <paramref name="list"/>.
        /// </summary>
        /// <remarks>
        /// For performance-reasons this function will be reduced to a string-representation comparison.
        /// For this reason it is required to specify a <paramref name="serializableFunction"/> thich returns a string-representation for a list-item.
        /// It is also required to pass a <paramref name="separator"/> which will never occurr in any string-representation of any list-item.
        /// </remarks>
        /// <returns>
        /// Returns true if and only if the given <paramref name="subList"/> is contained in <paramref name="list"/> in the correct order.
        /// </returns>
        public static bool ContainsSublist<T>(this IList<T> list, IList<T> subList, Func<T, string> serializableFunction, string separator = "-")
        {
            if (list == null || subList == null)
            {
                throw new ArgumentException($"Parameter {nameof(list)} and {nameof(subList)} may not be null");
            }
            if (subList.Count > list.Count)
            {
                return false;
            }
            if (subList.Count == 0)
            {
                return true;
            }
            string listAsString = string.Join(separator, list.Select(item => serializableFunction(item)));
            string subListAsString = string.Join(separator, subList.Select(item => serializableFunction(item)));
            return listAsString.Contains(subListAsString);
        }
        public static IList<T> NTimes<T>(this IEnumerable<T> input, uint n)
        {
            List<T> result = new();
            int i = 0;
            while (i < n)
            {
                i += 1;
                result.AddRange(input);
            }
            return result;
        }
        public static uint SwapEndianness(uint value)
        {
            return ((value & 0x000000ff) << 24)
                 + ((value & 0x0000ff00) << 08)
                 + ((value & 0x00ff0000) >> 08)
                 + ((value & 0xff000000) >> 24);
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
                throw new ArgumentException($"The argument for parameter {nameof(byteArray)} must have a length which is a multiple of 4.");
            }
            uint[] result = new uint[byteArray.Length / 4];
            for (int i = 0; i < byteArray.Length / 4; i++)
            {
                result[i] = ByteArrayToUnsignedInteger32Bit(new byte[] { byteArray[4 * i], byteArray[(4 * i) + 1], byteArray[(4 * i) + 2], byteArray[(4 * i) + 3] });
            }
            return result;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new();
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
            List<PropertyInfo> result = new();
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
            List<string> result = new();
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
            HashSet<Type> result = new() { type };
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
            List<List<T>> result = new();
            foreach (T[] innerArray in items)
            {
                List<T> innerList = new();
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
            DirectoryInfo directoryInfo = new(folder);
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
                string program = null;
                string paths = Environment.ExpandEnvironmentVariables("%PATH%");// "$PATH" not used because of https://github.com/dotnet/runtime/issues/25792
                foreach (string path in paths.Split(':'))
                {
                    string combined = Path.Combine(path, this._Programname);
                    if (File.Exists(combined) && SpecialFileInformation.FileIsExecutable(program))
                    {
                        program = combined;
                        break;
                    }
                }
                return (program != null, program);
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
            List<string> result = new();
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
            ForEachFileAndDirectoryTransitively(sourceFolder, (directory, obj) => { /*TODO ensure directory exists in target-folder*/}, (sourceFile, @object) => fileAction(sourceFile, @object), false, null, null);
            if (deleteAlreadyExistingFilesWithoutCopy)
            {
                RemoveContentOfFolder(sourceFolder);
            }
            else
            {
                DeleteAllEmptyFolderTransitively(sourceFolder, false);
            }
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
            DirectoryInfo directoryInfo = new(folder);
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
            ConcurrentBag<Tuple<Func<T>, T, Exception>> result = new();
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
            public readonly object _LockObject = new();
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
            throw new NotImplementedException(); // TODO call CastHelper using reflection
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
                throw new ArgumentException($"Parameter {nameof(hexString)} may not have an odd amount of characters");
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
        public static string[] SplitOnNewLineCharacter(string input)
        {
            return input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(line => line.Replace("\r", string.Empty).Replace("\n", string.Empty)).ToArray();
        }
        public static void AssertCondition(bool condition, string message = EmptyString)
        {
            if (!condition)
            {
                throw new AssertionException("Assertion failed. Condition is false." + (string.IsNullOrWhiteSpace(message) ? string.Empty : " " + message));
            }
        }
        public static void FormatCSVFile(string file, string separator = ";", bool firstLineContainsHeadlines = false)
        {
            FormatCSVFile(file, new UTF8Encoding(false), separator, firstLineContainsHeadlines);
        }
        public static void FormatCSVFile(string file, Encoding encoding, string separator = ";", bool firstLineContainsHeadlines = false)
        {
            UpdateCSVFileEntry(file, encoding, (line) => line, separator, firstLineContainsHeadlines);
        }
        public static void UpdateCSVFileEntry(string file, Func<string[], string[]> updateFunction, string separator = ";", bool firstLineContainsHeadlines = false)
        {
            UpdateCSVFileEntry(file, new UTF8Encoding(false), updateFunction, separator, firstLineContainsHeadlines);
        }
        public static void UpdateCSVFileEntry(string file, Encoding encoding, Func<string[], string[]> updateFunction, string separator = ";", bool firstLineContainsHeadlines = false)
        {
            IList<string[]> content = ReadCSVFile(file, encoding, out string[] headlines, separator, firstLineContainsHeadlines);
            content = content.Select(line => updateFunction(line)).ToList();
            WriteCSVFile(file, content, headlines, separator);
        }
        public static void WriteCSVFile(string file, IList<string[]> content, string[] headLines, string separator = ";")
        {
            WriteCSVFile(file, new UTF8Encoding(false), content, headLines, separator);
        }
        public static void WriteCSVFile(string file, Encoding encoding, IList<string[]> content, string[] headLines, string separator = ";")
        {
            List<string[]> contentAdjusted = content.ToList();
            if (headLines != null)
            {
                contentAdjusted.Insert(0, headLines);
            }
            EscapeForCSV(headLines);
            foreach (var line in content)
            {
                EscapeForCSV(headLines);
            }
            // TODO insert padding
            File.WriteAllLines(file, contentAdjusted.Select(item => string.Join(separator, item)), encoding);
        }

        internal static void EscapeForCSV(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = EscapeForCSV(line[i]);
            }
        }
        internal static string EscapeForCSV(string item)
        {
            if (item.Contains("\""))
            {
                item = item.Replace("\"", "\"\"");
                item = $"\"{item}\"";
            }
            return item;
        }

        public static IList<string[]> ReadCSVFile(string file, out string[] headLines, string separator = ";", bool firstLineContainsHeadlines = false, bool trimValues = true)
        {
            return ReadCSVFile(file, new UTF8Encoding(false), out headLines, separator, firstLineContainsHeadlines, trimValues);
        }
        public static IList<string[]> ReadCSVFile(string file, Encoding encoding, out string[] headLines, string separator = ";", bool firstLineContainsHeadlines = false, bool trimValues = true)
        {
            List<string[]> outterList = new();
            string[] lines = File.ReadAllLines(file, encoding);
            List<string> headlineValues = new();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (i == 0 && firstLineContainsHeadlines)
                {
                    headlineValues.AddRange(line.Split(new string[] { separator }, StringSplitOptions.None).Select(item => NormalizeCSVItemForReading(item, trimValues)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        List<string> innerList = new();
                        if (line.Contains(separator))
                        {
                            innerList.AddRange(line.Split(new string[] { separator }, StringSplitOptions.None).Select(item => NormalizeCSVItemForReading(item, trimValues)));
                        }
                        else
                        {
                            innerList.Add(line);
                        }
                        outterList.Add(innerList.ToArray());
                    }
                }
            }
            if (firstLineContainsHeadlines)
            {
                headLines = headlineValues.ToArray();
            }
            else
            {
                headLines = null;
            }
            return outterList;
        }

        private static string NormalizeCSVItemForReading(string value, bool trimValues)
        {
            if (trimValues)
            {
                value = value.Trim();
            }
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value[1..];
                value = value.Remove(value.Length - 1);
                value = value.Replace("\"\"", "\"");
                value = value.Trim();
            }
            return value;
        }

        /// <summary>
        /// Executes <paramref name="action"/>. When <paramref name="action"/> longer takes than <paramref name="timeout"/> then <paramref name="action"/> will be aborted.
        /// </summary>
        public static bool RunWithTimeout(this ThreadStart action, TimeSpan timeout)
        {
            Thread workerThread = new(action);
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
                XmlSchemaSet schemaSet = new();
                schemaSet.Add(null, XmlReader.Create(new StringReader(xsd)));
                XDocument xDocument = XDocument.Parse(xml);

                List<object> events = new();
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

        public static readonly XmlWriterSettings ApplyXSLTToXMLXMLWriterDefaultSettings = new() { Indent = true, Encoding = new UTF8Encoding(false), OmitXmlDeclaration = true, IndentChars = "    " };
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
            XslCompiledTransform myXslTrans = new();
            myXslTrans.Load(XmlReader.Create(new StringReader(xslt)));
            using StringWriter stringWriter = new();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, applyXSLTToXMLXMLWriterDefaultSettings))
            {
                myXslTrans.Transform(XmlReader.Create(new StringReader(xml)), xmlWriter);
            }
            return xmlDeclaration + stringWriter.ToString();
        }
        public static readonly Encoding FormatXMLFile_DefaultEncoding = new UTF8Encoding(false);
        public static readonly XmlWriterSettings FormatXMLFile_DefaultXmlWriterSettings = new() { Indent = true, IndentChars = "    " };
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
            using MemoryStream memoryStream = new();
            using XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
            XmlDocument document = new();
            document.LoadXml(xmlString);
            xmlWriter.Flush();
            memoryStream.Flush();
            memoryStream.Position = 0;
            using StreamReader streamReader = new(memoryStream);
            return streamReader.ReadToEnd();
        }
        public static uint BinaryStringToUint(string binaryString)
        {
            return (uint)Convert.ToInt32(binaryString, 2);
        }
        public static string UintToBinaryString(uint binaryString)
        {
            return Convert.ToString(binaryString, 2);
        }
        public static BigInteger BinaryStringToBigInteger(string binaryString)
        {
            throw new NotImplementedException();
        }
        public static string BigIntegerToBinaryString(BigInteger binaryString)
        {
            throw new NotImplementedException();
        }

        public static string XmlToString(XmlDocument xmlDocument)
        {
            StringBuilder stringBuilder = new();
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
            using ExternalProgramExecutor externalProgramExecutor = new("mountvol", $"{mountPoint} \\\\?\\Volume{{{volumeId}}}\\")
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.LogObject = GRYLog.Create();
            externalProgramExecutor.LogObject.Configuration.Enabled = false;
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
        }
        public static ISet<Guid> GetAvailableVolumeIds()
        {
            using ExternalProgramExecutor externalProgramExecutor = new("mountvol", string.Empty)
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.LogObject = GRYLog.Create();
            externalProgramExecutor.LogObject.Configuration.Enabled = false;
            externalProgramExecutor.StartSynchronously();
            if (externalProgramExecutor.ExitCode != 0)
            {
                throw new Exception($"Exitcode of mountvol was {externalProgramExecutor.ExitCode}. StdErr:" + string.Join(Environment.NewLine, externalProgramExecutor.AllStdErrLines));
            }
            HashSet<Guid> result = new();
            for (int i = 0; i < externalProgramExecutor.AllStdOutLines.Length - 1; i++)
            {
                string rawLine = externalProgramExecutor.AllStdOutLines[i];
                try
                {
                    string line = rawLine.Trim(); //line looks like "\\?\Volume{80aa12de-7392-4051-8cd2-f28bf56dc9d3}\"
                    string prefix = @"\\?\Volume{";
                    if (line.StartsWith(prefix))
                    {
                        line = line[prefix.Length..]; //remove "\\?\Volume{"
                        line = line[0..^2]; //remove "}\"
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
            HashSet<string> result = new();
            foreach (Guid volumeId in GetAvailableVolumeIds())
            {
                result.UnionWith(GetMountPoints(volumeId));
            }
            return result;
        }
        public static ISet<string> GetMountPoints(Guid volumeId)
        {
            HashSet<string> result = new();
            using ExternalProgramExecutor externalProgramExecutor = new("mountvol", string.Empty)
            {
                ThrowErrorIfExitCodeIsNotZero = true,
                CreateWindow = false
            };
            externalProgramExecutor.LogObject = GRYLog.Create();
            externalProgramExecutor.LogObject.Configuration.Enabled = false;
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
            using ExternalProgramExecutor externalProgramExecutor = new("mountvol", $"{mountPoint} /d")
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
                throw new ArgumentException($"Length of parameter {nameof(value)} must be 4.");
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
                throw new ArgumentException($"Length of parameter {nameof(value)} must be 8.");
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
                List<T> obj1Copy = new(obj1);
                List<T> obj2Copy = new(obj2);
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
            using StreamReader streamReader = new(new TcpClient(domain, port).GetStream());
            DateTime originalDateTime = DateTime.ParseExact(streamReader.ReadToEnd().Substring(begin, length), format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return TimeZoneInfo.ConvertTime(originalDateTime, timezone);
        }

        public static SerializableDictionary<TKey, TValue> ToSerializableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            SerializableDictionary<TKey, TValue> result = new();
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
        public static Tuple<string, string, string> ResolvePathOfProgram(string program, string argument, string workingDirectory)
        {
            // adapt working directory if required
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }

            // resolve program
            if (HasPath(program))
            {
                if (IsRelativePath(program))
                {
                    program = ResolveToFullPath(program, workingDirectory);
                }
            }
            else
            {
                string cwdWithProgram = Path.Combine(workingDirectory, program);
                if (File.Exists(cwdWithProgram))
                {
                    program = cwdWithProgram;
                }
                else
                {
                    if (TryResolvePathByPathVariable(program, out string programWithFullPath))
                    {
                        program = programWithFullPath;
                    }
                    else
                    {
                        throw new ArgumentException($"Program '{program}' can not be found");
                    }
                }
            }

            // check program
            if (File.Exists(program))
            {
                if (SpecialFileInformation.FileIsExecutable(program))
                {
                    // nothing to do
                }
                else
                {
                    // adapt argument
                    if (OperatingSystem.OperatingSystem.GetCurrentOperatingSystem() is Windows)
                    {
                        argument = $"\"{program}\" {argument}";
                        program = SpecialFileInformation.GetDefaultProgramToOpenFile(Path.GetExtension(program));
                    }
                    else
                    {
                        throw new ArgumentException($"Program '{program}' is not executable");
                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"Program '{program}' does not exist");
            }

            return new Tuple<string, string, string>(program, argument, workingDirectory);
        }
        private static bool HasPath(string str)
        {
            return str.Contains("/") || str.Contains("\\");
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
            if (!itemHasValueType)
            {
                return ReferenceEquals(item1, item2);
            }
            Type type = item1.GetType();
            if (!type.Equals(item2.GetType()))//TODO ignore generics here when type is keyvaluepair
            {
                return false;
            }
            if (EnumerableTools.TypeIsKeyValuePair(type))
            {
                System.Collections.Generic.KeyValuePair<object, object> kvp1 = EnumerableTools.ObjectToKeyValuePairUnsafe<object, object>(item1);
                System.Collections.Generic.KeyValuePair<object, object> kvp2 = EnumerableTools.ObjectToKeyValuePairUnsafe<object, object>(item2);
                return ImprovedReferenceEquals(kvp1.Key, kvp2.Key) && ImprovedReferenceEquals(kvp1.Value, kvp2.Value);
            }
            else
            {
                return item1.Equals(item2);
            }
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
                if (i == 0)
                {
                    splitted[i] = splitted[i].ToString().ToUpper().First();
                }
                if (i > 0)
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

        private static readonly Regex _OneOrMoreHexSigns = new(@"^[0-9a-f]+$");
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
            Subject<T> subject = new();
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
        public static Encoding GetEncodingByIdentifier(string encodingIdentifier)
        {
            if (encodingIdentifier == "utf-8")
            {
                return new UTF8Encoding(false);
            }
            if (encodingIdentifier == "utf-8-bom")
            {
                return new UTF8Encoding(true);
            }
            if (encodingIdentifier == "utf-16")
            {
                return new UnicodeEncoding(false, false);
            }
            if (encodingIdentifier == "utf-16-bom")
            {
                return new UnicodeEncoding(false, true);
            }
            if (encodingIdentifier == "utf-16-be")
            {
                return new UnicodeEncoding(true, false);
            }
            if (encodingIdentifier == "utf-16-be-bom")
            {
                return new UnicodeEncoding(true, true);
            }
            if (encodingIdentifier == "utf-32")
            {
                return new UTF32Encoding(false, false);
            }
            if (encodingIdentifier == "utf-32-bom")
            {
                return new UTF32Encoding(false, true);
            }
            if (encodingIdentifier == "utf-32-be")
            {
                return new UTF32Encoding(true, false);
            }
            if (encodingIdentifier == "utf-32-be-bom")
            {
                return new UTF32Encoding(true, true);
            }
            return Encoding.GetEncoding(encodingIdentifier);
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

                    key.SetValue("AppsUseLightTheme", this._Enabled ? 0 : 1);
                    key.SetValue("SystemUsesLightTheme", this._Enabled ? 0 : 1);
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
                        return ((int)key.GetValue("AppsUseLightTheme")) == 0 && ((int)key.GetValue("SystemUsesLightTheme")) == 0;
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
    }
}