using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GRYLibraryTest
{
    [TestClass]
    public class GLogTest
    {
        [TestMethod]
        public void TestLogFileWithRelativePath()
        {
            string logFile = "logfile.log";
            Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                GRYLog logObject = new GRYLog();
                string file = logFile;
                Assert.IsFalse(File.Exists(file));
                string fileWithRelativePath = logFile;
                logObject.LogFile = fileWithRelativePath;
                logObject.WriteToLogFile = true;
                Assert.AreEqual(fileWithRelativePath, logObject.LogFile);
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                logObject.LogInformation("test");
                File.Delete(file);
                Assert.IsFalse(File.Exists(file));
            }
            finally
            {
                Utilities.EnsureDirectoryDoesNotExist(logFile);
            }
        }
        [TestMethod]
        public void TestLogFileWithRelativePathWithSubFolder()
        {
            string folder = "folder";
            string fileWithRelativePath = folder + "/logFile.log";
            try
            {
                Utilities.EnsureFileDoesNotExist(fileWithRelativePath);
                Utilities.EnsureDirectoryDoesNotExist(folder);
                GRYLog logObject = new GRYLog();
                Assert.IsFalse(Directory.Exists(folder));
                logObject.LogFile = fileWithRelativePath;
                Assert.AreEqual(fileWithRelativePath, logObject.LogFile);
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                Directory.Delete(folder, true);
                Assert.IsFalse(Directory.Exists(folder));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(fileWithRelativePath);
                Utilities.EnsureDirectoryDoesNotExist(folder);
            }
        }

    }
}
