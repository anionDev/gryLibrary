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
using System.Xml.Schema;
using System.Xml.Serialization;

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
            SimpleDataStructure3 testObject = SimpleDataStructure3.GetRandom();
            SimpleGenericXMLSerializer<SimpleDataStructure3> serializer = new SimpleGenericXMLSerializer<SimpleDataStructure3>();
            string serialized = serializer.Serialize(testObject);
            SimpleDataStructure3 deserialized = serializer.Deserialize(serialized);
            Assert.AreEqual(testObject, deserialized);
        }
        [TestMethod]
        public void SerializeableDictionaryTest()
        {
            SerializableDictionary<int, string> dictionary = new SerializableDictionary<int, string>
            {
                { 1, "test1" },
                { 2, "test2" }
            };
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
            Assert.IsTrue(Utilities.ObjectIsList("somestring".ToCharArray()));
        }
        [TestMethod]
        public void IsPrimitiveTest()
        {
            Assert.IsTrue(Utilities.ObjectIsPrimitive(true));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(false));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(3));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(0));
            Assert.IsTrue(Utilities.ObjectIsPrimitive("somestring"));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(1.5));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(Guid.NewGuid()));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(default(Guid)));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(default(int)));
            Assert.IsTrue(Utilities.ObjectIsPrimitive(default(bool)));
            Assert.IsFalse(Utilities.ObjectIsPrimitive(new ArraySegment<int>()));
            Assert.IsFalse(Utilities.ObjectIsPrimitive(new ArrayList()));
            Assert.IsFalse(Utilities.ObjectIsPrimitive(new LinkedList<int>()));
            Assert.IsFalse(Utilities.ObjectIsPrimitive(new object()));
        }
        [TestMethod]
        public void IsDictionaryEntryTest()
        {
            Assert.IsFalse(Utilities.ObjectIsDictionaryEntry(new List<int>()));
            Assert.IsFalse(Utilities.ObjectIsDictionaryEntry(5));
            Assert.IsFalse(Utilities.ObjectIsDictionaryEntry(new System.Collections.Generic.KeyValuePair<object, object>()));
            Assert.IsFalse(Utilities.ObjectIsDictionaryEntry(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
            Assert.IsTrue(Utilities.ObjectIsDictionaryEntry(new DictionaryEntry()));
            Assert.IsTrue(Utilities.ObjectIsDictionaryEntry(new DictionaryEntry(new object(), new object())));
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
        public void IsSetTest()
        {
            Assert.IsTrue(Utilities.ObjectIsSet(new HashSet<int>()));
            Assert.IsTrue(Utilities.ObjectIsSet(new SortedSet<string>()));
            Assert.IsFalse(Utilities.ObjectIsSet(new LinkedList<int>()));
            Assert.IsFalse(Utilities.ObjectIsSet(new object()));
            Assert.IsFalse(Utilities.ObjectIsSet("somestring"));
        }
        [TestMethod]
        public void IsKeyValuePairTest11()
        {
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsKeyValuePairTest12()
        {
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<int, string>()));
        }
        [TestMethod]
        public void IsKeyValuePairTest21()
        {
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsKeyValuePairTest22()
        {
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<int, string>()));
        }
        [TestMethod]
        public void ObjectToKeyValuePairTest1()
        {
            object kvp11 = new object();
            object kvp12 = new object();
            object kvp1object = new System.Collections.Generic.KeyValuePair<object, object>(kvp11, kvp12);
            System.Collections.Generic.KeyValuePair<object, object> kvp1Typed = Utilities.ObjectToKeyValuePair<object, object>(kvp1object);
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Key);
            Assert.AreEqual(kvp12, kvp1Typed.Value);
        }
        [TestMethod]
        public void ObjectToKeyValuePairTest2()
        {
            int kvp11 = 6;
            string kvp12 = "test";
            object kvp1object = new System.Collections.Generic.KeyValuePair<int, string>(kvp11, kvp12);
            System.Collections.Generic.KeyValuePair<object, object> kvp1Typed = Utilities.ObjectToKeyValuePair<object, object>(kvp1object);
            Assert.IsTrue(Utilities.ObjectIsKeyValuePair(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Key);
            Assert.AreEqual(kvp12, kvp1Typed.Value);
        }

        [TestMethod]
        public void IsTupleTest11()
        {
            Assert.IsTrue(Utilities.ObjectIsTuple(new Tuple<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsTupleTest12()
        {
            Assert.IsTrue(Utilities.ObjectIsTuple(new WriteableTuple<object, object>()));
        }
        [TestMethod]
        public void IsTupleTest21()
        {
            Assert.IsTrue(Utilities.ObjectIsTuple(new Tuple<int, string>(5, "test")));
        }
        [TestMethod]
        public void IsTupleTest22()
        {
            Assert.IsTrue(Utilities.ObjectIsTuple(new WriteableTuple<int, string>()));
        }
        [TestMethod]
        public void ObjectToTupleTest1()
        {
            object kvp11 = new object();
            object kvp12 = new object();
            object kvp1object = new Tuple<object, object>(kvp11, kvp12);
            Tuple<object, object> kvp1Typed = Utilities.ObjectToTuple<object, object>(kvp1object);
            Assert.IsTrue(Utilities.ObjectIsTuple(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Item1);
            Assert.AreEqual(kvp12, kvp1Typed.Item2);
        }
        [TestMethod]
        public void ObjectToTupleTest2()
        {
            int kvp11 = 6;
            string kvp12 = "test";
            object kvp1object = new Tuple<int, string>(kvp11, kvp12);
            Tuple<object, object> kvp1Typed = Utilities.ObjectToTuple<object, object>(kvp1object);
            Assert.IsTrue(Utilities.ObjectIsTuple(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Item1);
            Assert.AreEqual(kvp12, kvp1Typed.Item2);
        }


        [TestMethod]
        public void ObjectToSettTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToSet<object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToSet<object>(5));

            Assert.IsTrue(Utilities.SetEquals(new HashSet<char> { 's', 'o', 'm', 'e', 't' }, Utilities.ObjectToSet<char>(new HashSet<char> { 's', 'o', 'm', 'e', 't', 'e', 's', 't' })));

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
            Assert.IsTrue(Utilities.ListEquals(testList, Utilities.ObjectToList<int>(testListAsObject)));

            Assert.IsTrue(Utilities.ListEquals(testList, (IList)new List<int> { 3, 4, 5 }.ToImmutableList()));
        }

        [TestMethod]
        public void ObjectToDictionarytFailTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToDictionary<object, object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Utilities.ObjectToDictionary<object, object>("somestring"));
        }

        [Ignore]
        [TestMethod]
        public void ObjectToDictionarytSuccessTest()
        {
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
            Assert.IsTrue(Utilities.ObjectIsEnumerable(new List<SimpleDataStructure3>()));
            Assert.IsTrue(Utilities.ObjectIsEnumerable(""));
            Assert.IsFalse(Utilities.ObjectIsEnumerable(4));

        }
        [TestMethod]
        public void EnumerableCount()
        {
            List<object> list = new List<object> { 3, 4, 5 };
            IEnumerable listAsEnumerable = list;
            Assert.AreEqual(list.Count, Utilities.Count(listAsEnumerable));
        }
        [TestMethod]
        public void IsAssignableFromTest()
        {
            Assert.IsTrue(Utilities.IsAssignableFrom(new SimpleDataStructure1(), typeof(SimpleDataStructure1)));
            Assert.IsTrue(Utilities.IsAssignableFrom(new SimpleDataStructure1(), typeof(IXmlSerializable)));
        }
        [TestMethod]
        public void ReferenceEqualsTest()
        {
            Guid guid1 = Guid.NewGuid();
            Assert.IsTrue(Utilities.ImprovedReferenceEquals(guid1, guid1));

            Guid guid2 = Guid.NewGuid();
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(guid1, guid2));

            object @object = new object();
            Assert.IsTrue(Utilities.ImprovedReferenceEquals(@object, @object));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(@object, new object()));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(@object, "string"));

            Assert.IsTrue(Utilities.ImprovedReferenceEquals(5, 5));
            Assert.IsTrue(Utilities.ImprovedReferenceEquals("string", "string"));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals("string", "string2"));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(5, 6));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(5, null));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(null, "string"));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(0, new object()));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(0, null));
            Assert.IsTrue(Utilities.ImprovedReferenceEquals(Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d"), Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d")));
            Assert.IsFalse(Utilities.ImprovedReferenceEquals(Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d"), Guid.Parse("22257693-bcee-4afd-a648-dd45ee066922")));
        }
    }
}
