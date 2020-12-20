using GRYLibrary.Core;
using GRYLibrary.Core.XMLSerializer;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        public void CSVTest1()
        {
            string file = "file1.csv";
            try
            {
                File.WriteAllText(file, "h1;h2;h3" + Environment.NewLine + "v1;v2;v3" + Environment.NewLine + "v4;v5;v6");
                string[][] content = Utilities.ReadCSVFile(file);
                Assert.AreEqual(3, content.Length);

                for (int i = 0; i < content.Length; i++)
                {
                    string[] currentLine = content[i];
                    Assert.AreEqual(3, currentLine.Length);
                    if (i == 0)
                    {
                        Assert.AreEqual("h1", currentLine[0]);
                        Assert.AreEqual("h2", currentLine[1]);
                        Assert.AreEqual("h3", currentLine[2]);
                    }
                    if (i == 1)
                    {
                        Assert.AreEqual("v1", currentLine[0]);
                        Assert.AreEqual("v2", currentLine[1]);
                        Assert.AreEqual("v3", currentLine[2]);
                    }
                    if (i == 2)
                    {
                        Assert.AreEqual("v4", currentLine[0]);
                        Assert.AreEqual("v5", currentLine[1]);
                        Assert.AreEqual("v6", currentLine[2]);
                    }
                }
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(file);
            }
        }
        [TestMethod]
        public void CSVTest2()
        {
            string file = "file1.csv";
            try
            {
                File.WriteAllText(file, "h1,h2,h3" + Environment.NewLine + "v1,v2,v3" + Environment.NewLine + "v4,v5,v6");
                string[][] content = Utilities.ReadCSVFile(file,",",true);
                Assert.AreEqual(2, content.Length);

                for (int i = 0; i < content.Length; i++)
                {
                    string[] currentLine = content[i];
                    Assert.AreEqual(3, currentLine.Length);
                     if (i == 0)
                    {
                        Assert.AreEqual("v1", currentLine[0]);
                        Assert.AreEqual("v2", currentLine[1]);
                        Assert.AreEqual("v3", currentLine[2]);
                    }
                    if (i == 1)
                    {
                        Assert.AreEqual("v4", currentLine[0]);
                        Assert.AreEqual("v5", currentLine[1]);
                        Assert.AreEqual("v6", currentLine[2]);
                    }
                }
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(file);
            }
        }
    }
}
