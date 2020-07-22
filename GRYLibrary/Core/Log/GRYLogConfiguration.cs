using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Console = GRYLibrary.Core.Log.ConcreteLogTargets.Console;

namespace GRYLibrary.Core.Log
{
    public class GRYLogConfiguration
    {

        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenConfigurationFileWillBeChanged { get; set; } = true;
        public HashSet<GRYLogTarget> LogTargets { get; set; } = new HashSet<GRYLogTarget>();
        public bool WriteLogEntriesAsynchronous { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public string ConfigurationFile { get; set; } = string.Empty;
        public bool PrintEmptyLines { get; set; } = false;

        public LoggedMessageTypeConfiguration GetLoggedMessageTypesConfigurationByLogLevel(LogLevel logLevel)
        {
            foreach (XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration> obj in this.LoggedMessageTypesConfiguration)
            {
                if (obj.Key == logLevel)
                {
                    return obj.Value;
                }
            }
            throw new KeyNotFoundException();
        }
        /// <summary>
        /// Represents a value which indicates if the output which goes to stderr should be treated as stdout.       
        /// </summary>
        public bool PrintErrorsAsInformation { get; set; } = false;
        public string Name { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; } = true;
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; } = true;
        public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public List<XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>> LoggedMessageTypesConfiguration { get; set; } = new List<XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>();
        public GRYLogLogFormat Format { get; set; } = GRYLogLogFormat.GRYLogFormat;
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; } = false;
        public bool LogEveryLineOfLogEntryInNewLine { get; set; } = false;
        [XmlIgnore]
        public string LogFile
        {
            get
            {
                return this.GetLogTarget<LogFile>().File;
            }
            set
            {
                this.GetLogTarget<LogFile>().File = value;

            }
        }
        public GRYLogConfiguration()
        {
        }
        public void ResetToDefaultValues()
        {
            this.ResetToDefaultValues(null);
        }
        public void ResetToDefaultValues(string logFile)
        {
            this.Name = string.Empty;
            this.LoggedMessageTypesConfiguration = new List<XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>
            {
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor = ConsoleColor.Gray }),
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan }),
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen }),
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow }),
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red }),
                new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed })
            };

            this.LogTargets = new HashSet<GRYLogTarget>
            {
                new Console() { Enabled = true },
                new LogFile() { File = logFile, Enabled = !string.IsNullOrWhiteSpace(logFile) }
            };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.LogTargets.Add(new WindowsEventLog() { Enabled = false });
            }
        }
        public Target GetLogTarget<Target>() where Target : GRYLogTarget
        {
            foreach (GRYLogTarget target in this.LogTargets)
            {
                if (target is Target target1)
                {
                    return target1;
                }
            }
            throw new KeyNotFoundException($"No {typeof(Target).Name}-target available");
        }
        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            return Utilities.LoadFromDisk<GRYLogConfiguration>(configurationFile).Object;
        }
        public static void SaveConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            Utilities.PersistToDisk(configuration, configurationFile);
        }

        public void SetEnabledOfAllLogTargets(bool newEnabledValue)
        {
            foreach (GRYLogTarget item in this.LogTargets)
            {
                item.Enabled = newEnabledValue;
            }
        }
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }

        public XmlSchema GetSchema()
        {
            return Generic.GenericGetSchema(this);
        }

        public void ReadXml(XmlReader reader)
        {
            Generic.GenericReadXml(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Generic.GenericWriteXml(this, writer);
        }
        #endregion
    }

}
