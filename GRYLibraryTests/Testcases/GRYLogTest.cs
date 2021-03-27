using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.LogObject;
using GRYLibrary.Core.LogObject.ConcreteLogTargets;
using GRYLibrary.Core.XMLSerializer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GRYLibrary.Tests.Testcases
{
    [TestClass]
    public class GRYLogTest
    {
        [TestMethod]
        public void TestLogProgress()
        {
            GRYLog logObject = GRYLog.Create();
            logObject.Configuration.StoreProcessedLogItemsInternally = true;
            logObject.LogProgress(0, 4);
            logObject.LogProgress(3, 122);
            logObject.LogProgress(73, 73);
            Assert.AreEqual(3, logObject.ProcessedLogItems.Count);
            Assert.AreEqual("Processed 0/4 items (0%)", logObject.ProcessedLogItems[0].PlainMessage);
            Assert.AreEqual("Processed 003/122 items (2,46%)", logObject.ProcessedLogItems[1].PlainMessage);
            Assert.AreEqual("Processed 73/73 items (100%)", logObject.ProcessedLogItems[2].PlainMessage);
        }
            [TestMethod]
            public void TestLogFileWithRelativePath()
            {
                string logFile = "logfile.log";
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                using GRYLog logObject = GRYLog.Create(logFile);
                logObject.Configuration.SetFormat(GRYLogLogFormat.OnlyMessage);
                string file = logFile;
                Assert.IsFalse(File.Exists(file));
                string fileWithRelativePath = logFile;
                logObject.Configuration.SetLogFile(fileWithRelativePath);
                Assert.AreEqual(fileWithRelativePath, logObject.Configuration.GetLogFile());
                Assert.IsFalse(File.Exists(fileWithRelativePath));
                string testContent = "test";
                logObject.Log(testContent);
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                Assert.AreEqual(testContent, File.ReadAllText(logFile));
            }
            finally
            {
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile);
            }
        }
        [TestMethod]
        public void TestLogFileWithRelativePathWithMessageIf()
        {
            string logFile = "logfile2.log";
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                using GRYLog logObject = GRYLog.Create(logFile);
                logObject.Configuration.SetFormat(GRYLogLogFormat.GRYLogFormat);
                string file = logFile;
                Assert.IsFalse(File.Exists(file));
                string fileWithRelativePath = logFile;
                logObject.Configuration.SetLogFile(fileWithRelativePath);
                Assert.AreEqual(fileWithRelativePath, logObject.Configuration.GetLogFile());
                Assert.IsFalse(File.Exists(fileWithRelativePath));
                string testContent = "test";
                logObject.Log(testContent, LogLevel.Warning, "MyMessageId");
                Assert.IsTrue(File.Exists(fileWithRelativePath));
                Assert.IsTrue(Regex.IsMatch(File.ReadAllText(logFile), "^\\[\\d\\d\\d\\d-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d\\] \\[MyMessageId\\] \\[Warning\\] test$"));
            }
            finally
            {
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile);
            }
        }
        [TestMethod]
        public void TestLogFileWithRelativePathWithSubFolder()
        {
            string folder = "folder";
            string logFile = folder + "/logFile.log";
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile);
            try
            {
                using GRYLog logObject = GRYLog.Create(logFile);
                logObject.Configuration.SetFormat(GRYLogLogFormat.OnlyMessage);
                Assert.IsFalse(File.Exists(logFile));
                string testContent = "x";
                logObject.Log(testContent);
                Assert.IsTrue(File.Exists(logFile));
                Assert.AreEqual(testContent, File.ReadAllText(logFile));
            }
            finally
            {
                Core.Miscellaneous.Utilities.EnsureDirectoryDoesNotExist(folder);
            }
        }
        [Ignore]
        [TestMethod]
        public void TestLogFileWithConfigurationchangeOnRuntime()
        {
            string configurationFile = "log.configuration";
            string logFile1 = "log1.log";
            string logFile2 = "log2.log";
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile1);
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile2);
            Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(configurationFile);
            GRYLogConfiguration configuration = new()
            {
                ConfigurationFile = configurationFile
            };
            configuration.ResetToDefaultValues(logFile1);
            configuration.SetFormat(GRYLogLogFormat.OnlyMessage);
            GRYLogConfiguration.SaveConfiguration(configurationFile, configuration);
            UTF8Encoding encoding = new(false);
            try
            {
                using GRYLog logObject = GRYLog.CreateByConfigurationFile(configurationFile);
                logObject.Log("test1", LogLevel.Information);//will be logged
                logObject.Log("test2", LogLevel.Debug);//will not be logged because 'debug' is not contained in LogLevels by default
                Assert.AreEqual("test1", File.ReadAllText(logFile1, encoding));

                GRYLogConfiguration reloadedConfiguration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                reloadedConfiguration.GetLogTarget<LogFile>().LogLevels.Add(LogLevel.Debug);
                GRYLogConfiguration.SaveConfiguration(configurationFile, reloadedConfiguration);

                System.Threading.Thread.Sleep(1000);//wait until config is reloaded

                logObject.Log("test3", LogLevel.Debug);// will now be logged
                Assert.AreEqual("test1" + Environment.NewLine + "test3", File.ReadAllText(logFile1, encoding));

                reloadedConfiguration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                reloadedConfiguration.SetLogFile(logFile2);
                GRYLogConfiguration.SaveConfiguration(configurationFile, reloadedConfiguration);

                System.Threading.Thread.Sleep(1000);//wait until config is reloaded

                logObject.Log("test4", LogLevel.Debug);// will be logged
                Assert.AreEqual("test1" + Environment.NewLine + "test3", File.ReadAllText(logFile1, encoding));
                Assert.AreEqual("test4", File.ReadAllText(logFile2, encoding));
            }
            finally
            {
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile1);
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(logFile2);
                Core.Miscellaneous.Utilities.EnsureFileDoesNotExist(configurationFile);
            }
        }
        [Ignore]
        [TestMethod]
        public void SerializeAndDeserialize1()
        {
            // arange
            GRYLogConfiguration logConfiguration = new()
            {
                Name = "MyLog",
                ConvertTimeForLogentriesToUTCFormat = true
            };

            // act
            SimpleGenericXMLSerializer<GRYLogConfiguration> serializer = new();
            string serializedLogConfiguration = serializer.Serialize(logConfiguration);

            SimpleGenericXMLSerializer<GRYLogConfiguration> deserializer = new();
            GRYLogConfiguration logConfigurationReloaded = deserializer.Deserialize(serializedLogConfiguration);

            // assert
            Assert.AreEqual(logConfiguration, logConfigurationReloaded);
            Assert.AreEqual(logConfiguration.GetHashCode(), logConfigurationReloaded.GetHashCode());
            Assert.IsTrue(Generic.GenericEquals(logConfiguration, logConfigurationReloaded));
            Assert.AreEqual(Generic.GenericGetHashCode(logConfiguration), Generic.GenericGetHashCode(logConfigurationReloaded));
        }
        [TestMethod]
        public void SerializeAndDeserialize2()
        {
            // arange
            GRYLogConfiguration logConfiguration = new() { Name = "MyLog" };

            // act
            string serializedLogConfiguration = Generic.GenericSerialize(logConfiguration);
            GRYLogConfiguration logConfigurationReloaded = Generic.GenericDeserialize<GRYLogConfiguration>(serializedLogConfiguration);

            // assert
            Assert.AreEqual(logConfiguration, logConfigurationReloaded);
            Assert.AreEqual(logConfiguration.GetHashCode(), logConfigurationReloaded.GetHashCode());
            Assert.IsTrue(Generic.GenericEquals(logConfiguration, logConfigurationReloaded));
            Assert.AreEqual(Generic.GenericGetHashCode(logConfiguration), Generic.GenericGetHashCode(logConfigurationReloaded));
        }
    }
}
