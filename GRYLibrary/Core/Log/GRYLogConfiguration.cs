using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            this.LoggedMessageTypesConfiguration = new List<XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>>();
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor = ConsoleColor.Gray }));
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan }));
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen }));
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow }));
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red }));
            this.LoggedMessageTypesConfiguration.Add(new XMLSerializer.KeyValuePair<LogLevel, LoggedMessageTypeConfiguration>(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed }));

            this.LogTargets = new HashSet<GRYLogTarget>();
            this.LogTargets.Add(new Console() { Enabled = true });
            this.LogTargets.Add(new LogFile() { File = logFile, Enabled = !string.IsNullOrWhiteSpace(logFile) });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.LogTargets.Add(new WindowsEventLog() { Enabled = false });
            }
        }
        public Target GetLogTarget<Target>() where Target : GRYLogTarget
        {
            foreach (GRYLogTarget target in this.LogTargets)
            {
                if (target is Target)
                {
                    return (Target)target;
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

        public override int GetHashCode()
        {
            return this.DateFormat.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            GRYLogConfiguration typedConfiguration = obj as GRYLogConfiguration;
            if (typedConfiguration == null)
            {
                return false;
            }
            if (this.ReloadConfigurationWhenConfigurationFileWillBeChanged != typedConfiguration.ReloadConfigurationWhenConfigurationFileWillBeChanged)
            {
                return false;
            }
            if (this.LogTargets.SetEquals(typedConfiguration.LogTargets))
            {
                return false;
            }
            if (this.WriteLogEntriesAsynchronous != typedConfiguration.WriteLogEntriesAsynchronous)
            {
                return false;
            }
            if (this.Enabled != typedConfiguration.Enabled)
            {
                return false;
            }
            if (this.ConfigurationFile != typedConfiguration.ConfigurationFile)
            {
                return false;
            }
            if (this.PrintEmptyLines != typedConfiguration.PrintEmptyLines)
            {
                return false;
            }
            if (this.PrintErrorsAsInformation != typedConfiguration.PrintErrorsAsInformation)
            {
                return false;
            }
            if (this.Name != typedConfiguration.Name)
            {
                return false;
            }
            if (this.WriteExceptionMessageOfExceptionInLogEntry != typedConfiguration.WriteExceptionMessageOfExceptionInLogEntry)
            {
                return false;
            }
            if (this.WriteExceptionStackTraceOfExceptionInLogEntry != typedConfiguration.WriteExceptionStackTraceOfExceptionInLogEntry)
            {
                return false;
            }
            if (this.DateFormat != typedConfiguration.DateFormat)
            {
                return false;
            }
            if (this.LoggedMessageTypesConfiguration.SequenceEqual(typedConfiguration.LoggedMessageTypesConfiguration))
            {
                return false;
            }
            if (this.Format != typedConfiguration.Format)
            {
                return false;
            }
            if (this.ConvertTimeForLogentriesToUTCFormat != typedConfiguration.ConvertTimeForLogentriesToUTCFormat)
            {
                return false;
            }
            if (this.LogEveryLineOfLogEntryInNewLine != typedConfiguration.LogEveryLineOfLogEntryInNewLine)
            {
                return false;
            }
            if (this.LogFile != typedConfiguration.LogFile)
            {
                return false;
            }

            return true;
        }

        public void SetEnabledOfAllLogTargets(bool newEnabledValue)
        {
            foreach (GRYLogTarget item in this.LogTargets)
            {
                item.Enabled = newEnabledValue;
            }
        }
    }

}
