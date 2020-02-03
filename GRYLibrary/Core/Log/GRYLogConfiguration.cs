using GRYLibrary.Core.Log.ConcreteLogTargets;
using GRYLibrary.Core.XMLSerializer;
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
    public class GRYLogConfiguration : IXmlSerializable
    {

        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenConfigurationFileWillBeChanged { get; set; } = true;
        public ISet<GRYLogTarget> LogTargets { get; set; } = new HashSet<GRYLogTarget>();
        public bool WriteLogEntriesAsynchronous { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public string ConfigurationFile { get; set; } = string.Empty;
        public bool PrintEmptyLines { get; set; } = false;
        public bool PrintErrorsAsInformation { get; set; } = false;
        public string Name { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; } = true;
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; } = true;
        public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public IDictionary<LogLevel, LoggedMessageTypeConfiguration> LoggedMessageTypesConfiguration { get; set; } = new Dictionary<LogLevel, LoggedMessageTypeConfiguration>();
        public GRYLogLogFormat Format { get; set; } = GRYLogLogFormat.GRYLogFormat;
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; } = false;
        public bool LogEveryLineOfLogEntryInNewLine { get; set; } = false;
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
        public GRYLogConfiguration(string name, bool writeToConsole, bool writeToLogFile, bool writeToWindowsEventLog)
        {
            this.Name = name;
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor = ConsoleColor.Gray });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed });

            this.LogTargets.Add(new LogFile() { Enabled = writeToLogFile });
            this.LogTargets.Add(new Console() { Enabled = writeToConsole });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.LogTargets.Add(new WindowsEventLog());
            }
        }
        public GRYLogConfiguration() : this(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, true, false, false)
        {
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
            throw new KeyNotFoundException($"No {nameof(Target)}-target available");
        }
        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            GRYLogConfiguration result = Utilities.LoadFromDisk<GRYLogConfiguration>(configurationFile).Object;
            result.ConfigurationFile = configurationFile;
            return result;
        }
        public static void SaveConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            Utilities.PersistToDisk(configuration, configurationFile);
        }

        public XmlSchema GetSchema()
        {
            return new CustomizableXMLSerializer().GenericGetXMLSchema(this.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            new CustomizableXMLSerializer().GenericXMLDeserializer(this, reader);
        }
        public void WriteXml(XmlWriter writer)
        {
            new CustomizableXMLSerializer().GenericXMLSerializer(this, writer);
        }
        public override int GetHashCode()
        {
            return Utilities.GenericGetHashCode(this);
        }
        public override bool Equals(object obj)
        {
            return Utilities.GenericEquals(this, obj);
        }

        public void SetEnabledOfAllLogTargets(bool newEnabledValue)
        {
            foreach (var item in LogTargets)
            {
                item.Enabled = newEnabledValue;
            }
        }
    }

}
