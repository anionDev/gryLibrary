using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace GRYLibraryTest.Tests
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
                GRYLibrary.Utilities.EnsureFileExists(testFile);
                Assert.IsTrue(System.IO.File.Exists(testFile));
                GRYLibrary.Utilities.EnsureFileExists(testFile);
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
            GRYLibrary.Utilities.EnsureFileExists(testFile);
            Assert.IsTrue(System.IO.File.Exists(testFile));
            GRYLibrary.Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(System.IO.File.Exists(testFile));
            GRYLibrary.Utilities.EnsureFileDoesNotExist(testFile);
            Assert.IsFalse(System.IO.File.Exists(testFile));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryExists()
        {
            string testDir = "dir";
            try
            {
                Assert.IsFalse(System.IO.Directory.Exists(testDir));
                GRYLibrary.Utilities.EnsureDirectoryExists(testDir);
                Assert.IsTrue(System.IO.Directory.Exists(testDir));
                GRYLibrary.Utilities.EnsureDirectoryExists(testDir);
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
            GRYLibrary.Utilities.EnsureDirectoryExists(testDir);
            Assert.IsTrue(System.IO.Directory.Exists(testDir));
            GRYLibrary.Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(System.IO.Directory.Exists(testDir));
            GRYLibrary.Utilities.EnsureDirectoryDoesNotExist(testDir);
            Assert.IsFalse(System.IO.Directory.Exists(testDir));
        }
        [TestMethod]
        public void UtilitiesTestEnsureDirectoryDoesNotExist2()
        {
            string dir = "dir";
            string testFile = dir + "/file";
            GRYLibrary.Utilities.EnsureFileExists(testFile, true);
            Assert.IsTrue(System.IO.File.Exists(testFile));
            GRYLibrary.Utilities.EnsureDirectoryDoesNotExist(dir);
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
                GRYLibrary.Utilities.EnsureFileExists(file1, true);
                GRYLibrary.Utilities.EnsureFileExists(file2, true);
                GRYLibrary.Utilities.EnsureFileExists(file3, true);
                GRYLibrary.Utilities.EnsureFileExists(file4, true);

                System.Collections.Generic.IEnumerable<string> result = GRYLibrary.Utilities.GetFilesOfFolderRecursively(baseDir);
                Assert.AreEqual(4, result.Count());
            }
            finally
            {
                GRYLibrary.Utilities.EnsureDirectoryDoesNotExist(baseDir);
            }
        }
        [TestMethod]
        public void IncrementGuidTest1()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = GRYLibrary.Utilities.IncrementGuid(inputId);
            Assert.AreEqual("5fe3eb8e-39dc-469c-a9cd-ea740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest2()
        {
            string input = "0003eb8e-39dc-469c-a9cd-00740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = GRYLibrary.Utilities.IncrementGuid(inputId);
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-00740e90d339", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest3()
        {
            string input = "0003eb8e-39dc-469c-a9cd-90740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = GRYLibrary.Utilities.IncrementGuid(inputId, BigInteger.Parse("100000000000", NumberStyles.HexNumber));
            Assert.AreEqual("0003eb8e-39dc-469c-a9cd-a0740e90d338", result.ToString());
        }
        [TestMethod]
        public void IncrementGuidTest4()
        {
            string input = "5fe3eb8e-39dc-469c-a9cd-ea740e90d338";
            Guid inputId = Guid.Parse(input);
            Guid result = GRYLibrary.Utilities.IncrementGuid(inputId);
            Assert.AreNotEqual(input, result.ToString());
        }
    }
}
