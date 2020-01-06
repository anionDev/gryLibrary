using GRYLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GRYLibraryTest.Tests
{
    [TestClass]
    public class GRYLogTest
    {
        [TestMethod]
        public void TestLogFileWithRelativePath()
        {
            string logFile = "logfile.log";
            Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                using (GRYLog logObject = GRYLog.Create(logFile))
                {
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
                using (GRYLog logObject = GRYLog.Create(logFile))
                {
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
            }
            finally
            {
                Utilities.EnsureDirectoryDoesNotExist(folder);
            }
        }
        [TestMethod]
        public void TestLogFileWithConfigurationchangeOnRuntime()
        {
            string configurationFile = "log.configuration";
            GRYLogConfiguration configuration = new GRYLogConfiguration();
            configuration.LogFile = "log.log";
            GRYLogConfiguration.SaveConfiguration(configurationFile, configuration);
            try
            {
                using (GRYLog logObject = GRYLog.CreateByConfigurationFile(configurationFile))
                {
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
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(logFile);
            }
        }
    }
}
