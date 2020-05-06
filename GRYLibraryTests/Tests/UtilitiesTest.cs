using GRYLibrary.Core;
using GRYLibrary.Core.XMLSerializer;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GRYLibrary.Tests
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void UtilitiesTestEnsureFileExists()
        {
            string testFile = "file";
            try
            {
                Assert.IsFalse(System.IO.File.Exists(testFile));
                Utilities.EnsureFileExists(testFile);
                Assert.IsTrue(System.IO.File.Exists(testFile));
                Utilities.EnsureFileExists(testFile);
                Assert.IsTrue(System.IO.File.Exists(testFile));
            }
            finally
            {
                System.IO.File.Delete(testFile);
            }
        }
        [TestMethod]
        public void UtilitiesTestEnsureFileDoesNotExist()
        {
            string testFile = "file";
            Utilities.EnsureFileExists(testFile);
            Assert.IsTrue(System.IO.File.Exists(testFile));
            Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(System.IO.File.Exists(testFile));
            Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(System.IO.File.Exists(testFile));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryExists()
        {
            string testDir = "dir";
            try
            {
                Assert.IsFalse(System.IO.Directory.Exists(testDir));
                Utilities.EnsureDirectoryExists(testDir);
                Assert.IsTrue(System.IO.Directory.Exists(testDir));
                Utilities.EnsureDirectoryExists(testDir);
                Assert.IsTrue(System.IO.Directory.Exists(testDir));
            }
            finally
            {
                System.IO.Directory.Delete(testDir);
            }
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryDoesNotExist()
        {
            string testDir = "dir";
            Utilities.EnsureDirectoryExists(testDir);
            Assert.IsTrue(System.IO.Directory.Exists(testDir));
            Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(System.IO.Directory.Exists(testDir));
            Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(System.IO.Directory.Exists(testDir));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryDoesNotExist2()
        {
            string dir = "dir";
            string testFile = dir + "/file";
            Utilities.EnsureFileExists(testFile, true);
            Assert.IsTrue(System.IO.File.Exists(testFile));
            Utilities.EnsureDirectoryDoesNotExist(dir);
            Assert.IsFalse(System.IO.Directory.Exists(testFile));
        }
        [TestMethod]
        public void FileSelectorTest1()
        {
            string baseDir = "basetestdir/";
            string dir1 = baseDir + "dir1/";
            string dir2 = dir1 + "dir2/";
            string file1 = baseDir + dir1 + "file1";
            string file2 = baseDir + dir2 + "file2";
            string file3 = baseDir + dir2 + "file3";
            string file4 = baseDir + "dir3/file4";
            try
            {
                Utilities.EnsureFileExists(file1, true);
                Utilities.EnsureFileExists(file2, true);
                Utilities.EnsureFileExists(file3, true);
                Utilities.EnsureFileExists(file4, true);

                System.Collections.Generic.IEnumerable<string> result = Utilities.GetFilesOfFolderRecursively(baseDir);
                Assert.AreEqual(4, result.Count());
            }
            finally
            {
                Utilities.EnsureDirectoryDoesNotExist(baseDir);
            }
        }
        [TestMethod]
        public void IncrementGuidTest1()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Utilities.IncrementGuid(inputId);
            Assert.AreEqual("5fe3eb8e-39dc-469c-a9cd-ea740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest2()
        {
            string input = "0003eb8e-39dc-469c-a9cd-00740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Utilities.IncrementGuid(inputId);
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-00740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest3()
        {
            string input = "0003eb8e-39dc-469c-a9cd-90740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Utilities.IncrementGuid(inputId, BigInteger.Parse("100000000000", NumberStyles.HexNumber));
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-a0740e90d338", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest4()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Utilities.IncrementGuid(inputId);
            Assert.AreNotEqual(input, result.ToString());
        }
        [TestMethod]
        public void GenericSerializerTest1()
        {
            SimpleDataStructure3 testObject = SimpleDataStructure3.GetTestObject();
            SimpleGenericXMLSerializer<SimpleDataStructure3> seriailzer = new SimpleGenericXMLSerializer<SimpleDataStructure3>();
            Assert.AreEqual(File.ReadAllText(@"TestData\TestXMLSerialization\GenericSerializerTest1.txt", new UTF8Encoding(false)), seriailzer.Serialize(testObject));
        }
        [TestMethod]
        public void SerializeableDictionaryTest()
        {
            SerializableDictionary<int, string> dictionary = new SerializableDictionary<int, string>();
            dictionary.Add(1, "test1");
            dictionary.Add(2, "test2");
            SimpleGenericXMLSerializer<SerializableDictionary<int, string>> serializer = new SimpleGenericXMLSerializer<SerializableDictionary<int, string>>();
            string serializedDictionary = serializer.Serialize(dictionary);
            SerializableDictionary<int, string> reloadedDictionary = serializer.Deserialize(serializedDictionary);
            Assert.AreEqual(2, reloadedDictionary.Count);
            Assert.AreEqual("test1", reloadedDictionary[1]);
            Assert.AreEqual("test2", reloadedDictionary[2]);
        }
        [TestMethod]
        public void IsListTest()
        {
            Assert.IsTrue(Utilities.ObjectIsList(new List<int>()));
            Assert.IsTrue(Utilities.ObjectIsList(new ArraySegment<int>()));
            Assert.IsTrue(Utilities.ObjectIsList(new ArrayList()));
            Assert.IsFalse(Utilities.ObjectIsList(new LinkedList<int>()));
            Assert.IsFalse(Utilities.ObjectIsList(new object()));
            Assert.IsFalse(Utilities.ObjectIsList("somestring"));
        }
        [TestMethod]
        public void IsDictionaryTest()
        {
            Assert.IsTrue(Utilities.ObjectIsDictionary(new Dictionary<int, string>()));
            Assert.IsTrue(Utilities.ObjectIsDictionary(ImmutableDictionary.CreateBuilder<long, object>().ToImmutable()));
            Assert.IsFalse(Utilities.ObjectIsDictionary(new LinkedList<int>()));
            Assert.IsFalse(Utilities.ObjectIsDictionary(new object()));
            Assert.IsFalse(Utilities.ObjectIsDictionary("somestring"));
        }
        [TestMethod]
        public void ISettTest()
        {
            Assert.IsTrue(Utilities.ObjectIsSet(new HashSet<int>()));
            Assert.IsTrue(Utilities.ObjectIsSet(new SortedSet<string>()));
            Assert.IsFalse(Utilities.ObjectIsSet(new LinkedList<int>()));
            Assert.IsFalse(Utilities.ObjectIsSet(new object()));
            Assert.IsFalse(Utilities.ObjectIsSet("somestring"));
        }
        [TestMethod]
        public void ObjectToSettTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToSet<object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToSet<object>(5));

            Assert.IsTrue(Utilities.SetEquals(new HashSet<char> { 's' , 'o' , 'm' , 'e' , 't' }, Utilities.ObjectToSet<char>("sometest")));

            HashSet<int> testSet = new HashSet<int> { 3, 4, 5 };
            object testSetAsObject = testSet;
            Assert.IsTrue(Utilities.SetEquals(testSet, Utilities.ObjectToSet<int>(testSetAsObject)));
        }
        [TestMethod]
        public void ObjectToListTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToList<object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToList<object>("sometest"));

            List<int> testList = new List<int> { 3, 4, 5 };
            object testListAsObject = testList;
            Assert.IsTrue(Utilities.SequanceEqual(testList, Utilities.ObjectToList<int>(testListAsObject)));

            Assert.IsTrue(Utilities.SequanceEqual(testList, new List<int> { 3, 4, 5 }.ToImmutableList()));
        }
        [TestMethod]
        public void ObjectToDictionarytTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToDictionary<object, object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToDictionary<object, object>("somestring"));

            Dictionary<int, string> testDictionary = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" }, { 5, "5s" } };
            object testDictionaryAsObject = testDictionary;
            Assert.IsTrue(Utilities.DictionaryEquals(testDictionary, Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject)));

            IDictionary<int, string> testDictionary2 = new ConcurrentDictionary<int, string>();
            testDictionary2.Add(3, "3s");
            testDictionary2.Add(4, "4s");
            testDictionary2.Add(5, "5s");
            object testDictionaryAsObject2 = testDictionary2;
            Assert.IsTrue(Utilities.DictionaryEquals(testDictionary2, Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject2)));

            Assert.IsTrue(Utilities.DictionaryEquals(testDictionary, testDictionary2));
        }
        [TestMethod]
        public void ObjectIsEnumerableTest()
        {
            IEnumerable setAsEnumerable = new HashSet<object> { 3, 4, 5 };
            Assert.IsTrue(Utilities.ObjectIsEnumerable(setAsEnumerable));
            Assert.IsTrue(Utilities.ObjectIsEnumerable(new HashSet<object> { 3, 4, 5 }));
            Assert.IsTrue(Utilities.ObjectIsEnumerable(new HashSet<int> { 3, 4, 5 }));
        }
    }
}
