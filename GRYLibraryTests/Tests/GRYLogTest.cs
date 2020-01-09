using GRYLibrary;
using GRYLibrary.Core;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace GRYLibrary.Tests
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
                    Assert.AreEqual(testContent, File.ReadAllText(logFile));
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
                    Assert.AreEqual(testContent, File.ReadAllText(logFile));
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
            string logFile1 = "log1.log";
            string logFile2 = "log2.log";
            GRYLogConfiguration configuration = new GRYLogConfiguration();
            Utilities.EnsureFileDoesNotExist(logFile1);
            Utilities.EnsureFileDoesNotExist(logFile2);
            Utilities.EnsureFileDoesNotExist(configurationFile);
            configuration.LogFile = logFile1;
            configuration.Format = GRYLogLogFormat.OnlyMessage;
            GRYLogConfiguration.SaveConfiguration(configurationFile, configuration);
            UTF8Encoding encoding = new UTF8Encoding(false);
            try
            {
                using (GRYLog logObject = GRYLog.CreateByConfigurationFile(configurationFile))
                {
                    //TODO
                    logObject.Log("test1", LogLevel.Information);//will be logged
                    logObject.Log("test2", LogLevel.Debug);//will not be logged because verbose is not contained in LoggedMessageTypesInLogFile by default
                    Assert.AreEqual("test1", File.ReadAllText(logFile1, encoding));

                    GRYLogConfiguration reloadedConfiguration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                    reloadedConfiguration.LogLevelsForLogFile.Add(LogLevel.Debug);
                    GRYLogConfiguration.SaveConfiguration(configurationFile, reloadedConfiguration);

                    System.Threading.Thread.Sleep(1000);//wait until config is reloaded

                    logObject.Log("test3", LogLevel.Debug);// will be logged
                    Assert.AreEqual("test1" + Environment.NewLine + "test3", File.ReadAllText(logFile1, encoding));

                    reloadedConfiguration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                    reloadedConfiguration.LogFile = logFile2;
                    GRYLogConfiguration.SaveConfiguration(configurationFile, reloadedConfiguration);

                    System.Threading.Thread.Sleep(1000);//wait until config is reloaded

                    logObject.Log("test4", LogLevel.Debug);// will be logged
                    Assert.AreEqual("test1" + Environment.NewLine + "test3", File.ReadAllText(logFile1, encoding));
                    Assert.AreEqual("test4", File.ReadAllText(logFile2, encoding));
                }
            }
            finally
            {
                Utilities.EnsureFileDoesNotExist(logFile1);
                Utilities.EnsureFileDoesNotExist(logFile2);
                Utilities.EnsureFileDoesNotExist(configurationFile);
            }
        }
    }
}
