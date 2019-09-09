using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GRYLibraryTest.Tests
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
                GRYLog logObject = GRYLog.Create(logFile);
                logObject.Configuration.Format = GRYLogLogFormat.OnlyMessage;
                string file = logFile;
                Assert.IsFalse(File.Exists(file));
                string fileWithRelativePath = logFile;
                logObject.Configuration.LogFile = fileWithRelativePath;
                Assert.AreEqual(fileWithRelativePath, logObject.Configuration.LogFile);
                Assert.IsFalse(File.Exists(fileWithRelativePath));
                string testContent = "test";
                logObject.Log(testContent);
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                Assert.AreEqual(testContent + System.Environment.NewLine, File.ReadAllText(logFile));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(logFile);
            }
        }
        [TestMethod]
        public void TestLogFileWithRelativePathWithSubFolder()
        {
            string folder = "folder";
            string logFile = folder + "/logFile.log";
            Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                GRYLog logObject = GRYLog.Create(logFile);
                logObject.Configuration.Format = GRYLogLogFormat.OnlyMessage;
                string file = logFile;
                Assert.IsFalse(File.Exists(file));
                string fileWithRelativePath = logFile;
                logObject.Configuration.LogFile = fileWithRelativePath;
                Assert.AreEqual(fileWithRelativePath, logObject.Configuration.LogFile);
                Assert.IsFalse(File.Exists(fileWithRelativePath));
                string testContent = "test";
                logObject.Log(testContent);
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                Assert.AreEqual(testContent + System.Environment.NewLine, File.ReadAllText(logFile));
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(logFile);
                Utilities.EnsureDirectoryDoesNotExist(folder);
            }
        }

    }
}
