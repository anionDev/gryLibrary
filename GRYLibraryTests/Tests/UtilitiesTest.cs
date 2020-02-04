using GRYLibrary.Core;
using GRYLibrary.Core.XMLSerializer;
using GRYLibrary.Core.XMLSerializer.SerializationInfos;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure1;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;

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
        [Ignore]
        [TestMethod]
        public void GenericSerializerTest1()
        {
            SimpleDataStructure3 testObject = SimpleDataStructure3.GetTestObject();
            SimpleGenericXMLSerializer<SimpleDataStructure3> seriailzer = new SimpleGenericXMLSerializer<SimpleDataStructure3>();
            Assert.AreEqual(File.ReadAllText(@"TestData\TestXMLSerialization\GenericSerializerTest1.txt", new UTF8Encoding(false)), seriailzer.Serialize(testObject));
        }
        [Ignore]
        [TestMethod]
        public void SimpleDictionarySerializerTest()
        {
            IDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            dictionary.Add("key1", 2);
            dictionary.Add("key2", 4);
            CustomizableXMLSerializer customizableXMLSerializer = new CustomizableXMLSerializer();
            DictionarySerializer serializer = new DictionarySerializer(customizableXMLSerializer);
            Assert.IsTrue(serializer.IsApplicable(dictionary, typeof(IDictionary<,>)));
            IDictionary<dynamic, dynamic> dynamicDictionary = serializer.Cast(dictionary);
            Assert.AreEqual(2, dynamicDictionary.Count);

            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, Encoding = new UTF8Encoding(false), IndentChars = "     ", NewLineOnAttributes = false, OmitXmlDeclaration = true }))
                {
                    CustomizableXMLSerializer c = new CustomizableXMLSerializer();
                    c.GenericXMLSerializer(dictionary, xmlWriter);
                    result = stringWriter.ToString();
                }
            }
            Assert.AreEqual(@"<List>
     <Item>
          <Key><![CDATA[key2]]></Key>
          <Value>
               <int>4</int>
          </Value>
     </Item>
     <Item>
          <Key><![CDATA[key1]]></Key>
          <Value>
               <int>2</int>
          </Value>
     </Item>
</List>", result);
        }
        [Ignore]
        [TestMethod]
        public void SimpleDictionaryDeserializerTest()
        {
            string serializedDictionary = @"<List>
     <Item>
          <Key><![CDATA[key2]]></Key>
          <Value>
               <int>4</int>
          </Value>
     </Item>
     <Item>
          <Key><![CDATA[key1]]></Key>
          <Value>
               <int>2</int>
          </Value>
     </Item>
</List>";
            CustomizableXMLSerializer c = new CustomizableXMLSerializer();
            IDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            c.GenericXMLDeserializer(dictionary, XmlReader.Create(new StringReader(serializedDictionary)));

            Assert.AreEqual(2, dictionary.Count);
            Assert.IsTrue(dictionary.ContainsKey("key1"));
            Assert.IsTrue(dictionary.ContainsKey("key2"));
            Assert.AreEqual(2, dictionary["key1"]);
            Assert.AreEqual(4, dictionary["key2"]);
        }
        [Ignore]
        [TestMethod]
        public void SimpleListSerializerTest()
        {
            IList<string> list = new StringValues();
            list.Add("a");
            list.Add("b");
            CustomizableXMLSerializer customizableXMLSerializer = new CustomizableXMLSerializer();
            ListSerializer serializer = new ListSerializer(customizableXMLSerializer);
            Assert.IsTrue(serializer.IsApplicable(list, typeof(IList<>)));
            IList<dynamic> dynamicDictionary = serializer.Cast(list);
            Assert.AreEqual(2, dynamicDictionary.Count);

            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, Encoding = new UTF8Encoding(false), IndentChars = "     ", NewLineOnAttributes = false, OmitXmlDeclaration = true }))
                {
                    CustomizableXMLSerializer c = new CustomizableXMLSerializer();
                    c.GenericXMLSerializer(list, xmlWriter);
                    result = stringWriter.ToString();
                }
            }
            Assert.AreEqual(@"todo", result);
        }
        [Ignore]
        [TestMethod]
        public void InheritsFromTest()
        {
            Assert.IsFalse(Utilities.InheritsFrom(typeof(IList<int>), typeof(string)));
            Assert.IsFalse(Utilities.InheritsFrom(typeof(IList), typeof(IList<>)));
            Assert.IsTrue(Utilities.InheritsFrom(typeof(List<int>), typeof(IList<int>)));
            Assert.IsFalse(Utilities.InheritsFrom(typeof(IList<ArraySerializer>), typeof(IList<CustomXMLSerializer>)));
            Assert.IsTrue(Utilities.InheritsFrom(typeof(IList<int>), typeof(IList<>)));//most tricky part


        }
    }
}
