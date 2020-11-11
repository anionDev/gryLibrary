using GRYLibrary.Core;
using GRYLibrary.Core.XMLSerializer;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
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
using System.Xml.Serialization;

namespace GRYLibrary.Tests.Testcases
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
                Assert.IsFalse(File.Exists(testFile));
                Core.Utilities.EnsureFileExists(testFile);
                Assert.IsTrue(File.Exists(testFile));
                Core.Utilities.EnsureFileExists(testFile);
                Assert.IsTrue(File.Exists(testFile));
            }
            finally
            {
                File.Delete(testFile);
            }
        }
        [TestMethod]
        public void UtilitiesTestEnsureFileDoesNotExist()
        {
            string testFile = "file";
            Core.Utilities.EnsureFileExists(testFile);
            Assert.IsTrue(File.Exists(testFile));
            Core.Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(File.Exists(testFile));
            Core.Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(File.Exists(testFile));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryExists()
        {
            string testDir = "dir";
            try
            {
                Assert.IsFalse(Directory.Exists(testDir));
                Core.Utilities.EnsureDirectoryExists(testDir);
                Assert.IsTrue(Directory.Exists(testDir));
                Core.Utilities.EnsureDirectoryExists(testDir);
                Assert.IsTrue(Directory.Exists(testDir));
            }
            finally
            {
                Directory.Delete(testDir);
            }
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryDoesNotExist()
        {
            string testDir = "dir";
            Core.Utilities.EnsureDirectoryExists(testDir);
            Assert.IsTrue(Directory.Exists(testDir));
            Core.Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(Directory.Exists(testDir));
            Core.Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(Directory.Exists(testDir));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryDoesNotExist2()
        {
            string dir = "dir";
            string testFile = dir + "/file";
            Core.Utilities.EnsureFileExists(testFile, true);
            Assert.IsTrue(File.Exists(testFile));
            Core.Utilities.EnsureDirectoryDoesNotExist(dir);
            Assert.IsFalse(Directory.Exists(testFile));
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
                Core.Utilities.EnsureFileExists(file1, true);
                Core.Utilities.EnsureFileExists(file2, true);
                Core.Utilities.EnsureFileExists(file3, true);
                Core.Utilities.EnsureFileExists(file4, true);

                IEnumerable<string> result = Core.Utilities.GetFilesOfFolderRecursively(baseDir);
                Assert.AreEqual(4, result.Count());
            }
            finally
            {
                Core.Utilities.EnsureDirectoryDoesNotExist(baseDir);
            }
        }
        [TestMethod]
        public void IncrementGuidTest1()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Core.Utilities.IncrementGuid(inputId);
            Assert.AreEqual("5fe3eb8e-39dc-469c-a9cd-ea740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest2()
        {
            string input = "0003eb8e-39dc-469c-a9cd-00740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Core.Utilities.IncrementGuid(inputId);
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-00740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest3()
        {
            string input = "0003eb8e-39dc-469c-a9cd-90740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Core.Utilities.IncrementGuid(inputId, BigInteger.Parse("100000000000", NumberStyles.HexNumber));
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-a0740e90d338", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest4()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = Core.Utilities.IncrementGuid(inputId);
            Assert.AreNotEqual(input, result.ToString());
        }
        [Ignore]
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
            Assert.IsTrue(Core.Utilities.ObjectIsList(new List<int>()));
            Assert.IsTrue(Core.Utilities.ObjectIsList(new ArraySegment<int>()));
            Assert.IsTrue(Core.Utilities.ObjectIsList(new ArrayList()));
            Assert.IsFalse(Core.Utilities.ObjectIsList(new LinkedList<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsList(new object()));
            Assert.IsFalse(Core.Utilities.ObjectIsList("somestring"));
            Assert.IsTrue(Core.Utilities.ObjectIsList("somestring".ToCharArray()));
        }
        [TestMethod]
        public void IsPrimitiveTest()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(true));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(false));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(3));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(0));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive("somestring"));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(1.5));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(Guid.NewGuid()));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(default(Guid)));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(default(int)));
            Assert.IsTrue(Core.Utilities.ObjectIsPrimitive(default(bool)));
            Assert.IsFalse(Core.Utilities.ObjectIsPrimitive(new ArraySegment<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsPrimitive(new ArrayList()));
            Assert.IsFalse(Core.Utilities.ObjectIsPrimitive(new LinkedList<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsPrimitive(new object()));
        }
        [TestMethod]
        public void IsDictionaryEntryTest()
        {
            Assert.IsFalse(Core.Utilities.ObjectIsDictionaryEntry(new List<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionaryEntry(5));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionaryEntry(new System.Collections.Generic.KeyValuePair<object, object>()));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionaryEntry(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
            Assert.IsTrue(Core.Utilities.ObjectIsDictionaryEntry(new DictionaryEntry()));
            Assert.IsTrue(Core.Utilities.ObjectIsDictionaryEntry(new DictionaryEntry(new object(), new object())));
        }
        [TestMethod]
        public void IsDictionaryTest()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsDictionary(new Dictionary<int, string>()));
            Assert.IsTrue(Core.Utilities.ObjectIsDictionary(ImmutableDictionary.CreateBuilder<long, object>().ToImmutable()));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionary(new LinkedList<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionary(new object()));
            Assert.IsFalse(Core.Utilities.ObjectIsDictionary("somestring"));
        }
        [TestMethod]
        public void IsSetTest()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsSet(new HashSet<int>()));
            Assert.IsTrue(Core.Utilities.ObjectIsSet(new SortedSet<string>()));
            Assert.IsFalse(Core.Utilities.ObjectIsSet(new LinkedList<int>()));
            Assert.IsFalse(Core.Utilities.ObjectIsSet(new object()));
            Assert.IsFalse(Core.Utilities.ObjectIsSet("somestring"));
        }
        [TestMethod]
        public void IsKeyValuePairTest11()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsKeyValuePairTest12()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<int, string>()));
        }
        [TestMethod]
        public void IsKeyValuePairTest21()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsKeyValuePairTest22()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(new System.Collections.Generic.KeyValuePair<int, string>()));
        }
        [TestMethod]
        public void ObjectToKeyValuePairTest1()
        {
            object kvp11 = new object();
            object kvp12 = new object();
            object kvp1object = new System.Collections.Generic.KeyValuePair<object, object>(kvp11, kvp12);
            System.Collections.Generic.KeyValuePair<object, object> kvp1Typed = Core.Utilities.ObjectToKeyValuePair<object, object>(kvp1object);
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Key);
            Assert.AreEqual(kvp12, kvp1Typed.Value);
        }
        [TestMethod]
        public void ObjectToKeyValuePairTest2()
        {
            int kvp11 = 6;
            string kvp12 = "test";
            object kvp1object = new System.Collections.Generic.KeyValuePair<int, string>(kvp11, kvp12);
            System.Collections.Generic.KeyValuePair<object, object> kvp1Typed = Core.Utilities.ObjectToKeyValuePair<object, object>(kvp1object);
            Assert.IsTrue(Core.Utilities.ObjectIsKeyValuePair(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Key);
            Assert.AreEqual(kvp12, kvp1Typed.Value);
        }

        [TestMethod]
        public void IsTupleTest11()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(new Tuple<object, object>(new object(), new object())));
        }
        [TestMethod]
        public void IsTupleTest12()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(new WriteableTuple<object, object>()));
        }
        [TestMethod]
        public void IsTupleTest21()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(new Tuple<int, string>(5, "test")));
        }
        [TestMethod]
        public void IsTupleTest22()
        {
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(new WriteableTuple<int, string>()));
        }
        [TestMethod]
        public void ObjectToTupleTest1()
        {
            object kvp11 = new object();
            object kvp12 = new object();
            object kvp1object = new Tuple<object, object>(kvp11, kvp12);
            Tuple<object, object> kvp1Typed = Core.Utilities.ObjectToTuple<object, object>(kvp1object);
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Item1);
            Assert.AreEqual(kvp12, kvp1Typed.Item2);
        }
        [TestMethod]
        public void ObjectToTupleTest2()
        {
            int kvp11 = 6;
            string kvp12 = "test";
            object kvp1object = new Tuple<int, string>(kvp11, kvp12);
            Tuple<object, object> kvp1Typed = Core.Utilities.ObjectToTuple<object, object>(kvp1object);
            Assert.IsTrue(Core.Utilities.ObjectIsTuple(kvp1Typed));
            Assert.AreEqual(kvp11, kvp1Typed.Item1);
            Assert.AreEqual(kvp12, kvp1Typed.Item2);
        }


        [TestMethod]
        public void ObjectToSettTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToSet<object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToSet<object>(5));

            Assert.IsTrue(Core.Utilities.SetEquals(new HashSet<char> { 's', 'o', 'm', 'e', 't' }, Core.Utilities.ObjectToSet<char>(new HashSet<char> { 's', 'o', 'm', 'e', 't', 'e', 's', 't' })));

            HashSet<int> testSet = new HashSet<int> { 3, 4, 5 };
            object testSetAsObject = testSet;
            Assert.IsTrue(Core.Utilities.SetEquals(testSet, Core.Utilities.ObjectToSet<int>(testSetAsObject)));
        }
        [TestMethod]
        public void ObjectToListTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToList<object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToList<object>("sometest"));

            List<int> testList = new List<int> { 3, 4, 5 };
            object testListAsObject = testList;
            Assert.IsTrue(Core.Utilities.ListEquals(testList, Core.Utilities.ObjectToList<int>(testListAsObject)));

            Assert.IsTrue(Core.Utilities.ListEquals(testList, (IList)new List<int> { 3, 4, 5 }.ToImmutableList()));
        }

        [TestMethod]
        public void ObjectToDictionaryFailTest()
        {
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToDictionary<object, object>(new object()));
            Assert.ThrowsException<InvalidCastException>(() => Core.Utilities.ObjectToDictionary<object, object>("somestring"));
        }
        [TestMethod]
        public void ObjectToDictionarySuccessTest()
        {
            Dictionary<int, string> testDictionary = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" }, { 5, "5s" } };
            object testDictionaryAsObject = testDictionary;
            Core.Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject);
        }

        [TestMethod]
        public void DictionaryEqualsFailTest()
        {
            //arrange
            Dictionary<int, string> testDictionary1 = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" }, { 5, "5s" } };
            Dictionary<int, string> testDictionary2 = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" } };

            // act && assert
            Assert.IsFalse(Core.Utilities.DictionaryEquals<int, string>(testDictionary1, testDictionary2));
        }

        [TestMethod]
        public void DictionaryEqualsSuccessTest1()
        {
            Dictionary<int, string> testDictionary = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" }, { 5, "5s" } };
            object testDictionaryAsObject = testDictionary;
            Assert.IsTrue(Core.Utilities.DictionaryEquals(testDictionary, Core.Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject)));
        }
        [TestMethod]
        public void DictionaryEqualsSuccessTest2()
        {
            Dictionary<int, string> testDictionary = new Dictionary<int, string> { { 3, "3s" }, { 4, "4s" }, { 5, "5s" } };
            object testDictionaryAsObject = testDictionary;
            Assert.IsTrue(Core.Utilities.DictionaryEquals(testDictionary, Core.Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject)));

            IDictionary<int, string> testDictionary2 = new ConcurrentDictionary<int, string>();
            testDictionary2.Add(3, "3s");
            testDictionary2.Add(4, "4s");
            testDictionary2.Add(5, "5s");
            object testDictionaryAsObject2 = testDictionary2;
            Assert.IsTrue(Core.Utilities.DictionaryEquals(testDictionary2, Core.Utilities.ObjectToDictionary<int, string>(testDictionaryAsObject2)));

            Assert.IsTrue(Core.Utilities.DictionaryEquals(testDictionary, testDictionary2));
        }
        [TestMethod]
        public void ObjectIsEnumerableTest()
        {
            IEnumerable setAsEnumerable = new HashSet<object> { 3, 4, 5 };
            Assert.IsTrue(Core.Utilities.ObjectIsEnumerable(setAsEnumerable));
            Assert.IsTrue(Core.Utilities.ObjectIsEnumerable(new HashSet<object> { 3, 4, 5 }));
            Assert.IsTrue(Core.Utilities.ObjectIsEnumerable(new HashSet<int> { 3, 4, 5 }));
            Assert.IsTrue(Core.Utilities.ObjectIsEnumerable(new List<SimpleDataStructure3>()));
            Assert.IsTrue(Core.Utilities.ObjectIsEnumerable(string.Empty));
            Assert.IsFalse(Core.Utilities.ObjectIsEnumerable(4));

        }
        [TestMethod]
        public void EnumerableCount()
        {
            List<object> list = new List<object> { 3, 4, 5 };
            IEnumerable listAsEnumerable = list;
            Assert.AreEqual(list.Count, Core.Utilities.Count(listAsEnumerable));
        }
        [TestMethod]
        public void IsAssignableFromTest()
        {
            Assert.IsTrue(Core.Utilities.IsAssignableFrom(new SimpleDataStructure1(), typeof(SimpleDataStructure1)));
            Assert.IsTrue(Core.Utilities.IsAssignableFrom(new SimpleDataStructure1(), typeof(IXmlSerializable)));
        }
        [Ignore]
        [TestMethod]
        public void ReferenceEqualsWithCommonValuesTest()
        {
            Guid guid1 = Guid.NewGuid();
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(guid1, guid1));
            Guid guid2 = Guid.NewGuid();
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(guid1, guid2));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d"), Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d")));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(Guid.Parse("33257693-bcee-4afd-a648-dd45ee06695d"), Guid.Parse("22257693-bcee-4afd-a648-dd45ee066922")));
            object @object = new object();
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(@object, @object));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(@object, new object()));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(@object, "string"));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(5, 5));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals("string", "string"));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals("string", "string2"));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(5, 6));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(5, null));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(null, "string"));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(0, new object()));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(0, null));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(new System.Collections.Generic.KeyValuePair<int, string>(5, "s"), new System.Collections.Generic.KeyValuePair<int, string>(5, "s")));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(new System.Collections.Generic.KeyValuePair<int, string>(5, "s1"), new System.Collections.Generic.KeyValuePair<int, string>(5, "s2")));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(new System.Collections.Generic.KeyValuePair<int, string>(5, "s"), new System.Collections.Generic.KeyValuePair<int, string>(6, "s")));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(new System.Collections.Generic.KeyValuePair<int, string>(5, null), new System.Collections.Generic.KeyValuePair<int, object>(5, null)));
        }
        [TestMethod]
        public void ReferenceEqualsCycleTest1()
        {
            CycleA cycle = CycleA.GetRandom();
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(cycle, cycle));
        }
        [TestMethod]
        public void ReferenceEqualsCycleTest2()
        {
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(CycleA.GetRandom(), CycleA.GetRandom()));
        }
        [TestMethod]
        public void ReferenceEqualsCycleTest3()
        {
            object obj1 = new object();
            object obj2 = new object();

            WriteableTuple<object, object> wt1 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt2 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt3 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt4 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt5 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt6 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt7 = new WriteableTuple<object, object>();
            WriteableTuple<object, object> wt8 = new WriteableTuple<object, object>();

            System.Collections.Generic.KeyValuePair<object, object> kvp1 = new System.Collections.Generic.KeyValuePair<object, object>(wt1, wt2);
            System.Collections.Generic.KeyValuePair<object, object> kvp2 = new System.Collections.Generic.KeyValuePair<object, object>(wt3, wt4);
            System.Collections.Generic.KeyValuePair<object, object> kvp3 = new System.Collections.Generic.KeyValuePair<object, object>(wt5, wt6);
            System.Collections.Generic.KeyValuePair<object, object> kvp4 = new System.Collections.Generic.KeyValuePair<object, object>(wt7, obj1);
            System.Collections.Generic.KeyValuePair<object, object> kvp5 = new System.Collections.Generic.KeyValuePair<object, object>(obj1, obj2);
            System.Collections.Generic.KeyValuePair<object, object> kvp6 = new System.Collections.Generic.KeyValuePair<object, object>(obj2, wt8);

            wt1.Item1 = kvp2;
            wt2.Item1 = kvp3;
            wt3.Item1 = kvp4;
            wt4.Item1 = kvp5;
            wt5.Item1 = kvp5;
            wt6.Item1 = kvp6;
            wt7.Item1 = kvp1;
            wt8.Item1 = kvp1;

            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp1, kvp1));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp2, kvp2));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp3, kvp3));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp4, kvp4));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp5, kvp5));
            Assert.IsTrue(Core.Utilities.ImprovedReferenceEquals(kvp6, kvp6));

            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(kvp2, kvp3));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(kvp1, kvp3));
            Assert.IsFalse(Core.Utilities.ImprovedReferenceEquals(kvp1, kvp4));
        }

        [TestMethod]
        public void EnsurePathHasNoLeadingOrTrailingQuotesTest()
        {
            Assert.AreEqual("a", Core.Utilities.EnsurePathHasNoLeadingOrTrailingQuotes("'\"'\"a\"'\"'"));
        }
        [TestMethod]
        public void EnsurePathDoesNotHaveLeadingOrTrailingSlashOrBackslashTest()
        {
            Assert.AreEqual("a", Core.Utilities.EnsurePathEndsWithoutSlashOrBackslash("a/"));
            Assert.AreEqual("a", Core.Utilities.EnsurePathEndsWithoutSlashOrBackslash("a\\"));
        }
    }
}
