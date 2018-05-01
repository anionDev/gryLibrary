using GRLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GRLibraryTest
{
    [TestClass]
    public class GLogTest
    {
        [TestMethod]
        public void TestLogFileWithRelativePath()
        {
            GLog logObject = new GLog();
            string file = "y.log";
            Assert.IsFalse(File.Exists(file));
            string fileWithRelativePath =  "y.log";
            logObject.LogFile = fileWithRelativePath;
            Assert.AreEqual(fileWithRelativePath, logObject.LogFile);
            Assert.IsTrue(File.Exists(fileWithRelativePath));
            File.Delete(file);
            Assert.IsFalse(File.Exists(file));
        }
        [TestMethod]
        public void TestLogFileWithRelativePathWithSubFolder()
        {
            GLog logObject = new GLog();
            string folder = "x";
            Assert.IsFalse(Directory.Exists(folder));
            string fileWithRelativePath = folder + "/y.log";
            logObject.LogFile = fileWithRelativePath;
            Assert.AreEqual(fileWithRelativePath, logObject.LogFile);
            Assert.IsTrue(File.Exists(fileWithRelativePath));
            Directory.Delete(folder, true);
            Assert.IsFalse(Directory.Exists(folder));
        }

    }
}
