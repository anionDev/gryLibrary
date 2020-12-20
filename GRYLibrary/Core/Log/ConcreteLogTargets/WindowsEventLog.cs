using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    [SupportedOSPlatform("windows")]
    public class WindowsEventLog : GRYLogTarget
    {
        public WindowsEventLog() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            using EventLog eventLog = new EventLog(Utilities.GetNameOfCurrentExecutable()) { Source = logObject.Configuration.Name };
            string messageId;
            if (string.IsNullOrWhiteSpace(logItem.MessageId))
            {
                messageId = string.Empty;
            }
            else
            {
                messageId = $"{logItem.MessageId}: ";
            }
            eventLog.WriteEntry(messageId + logItem.PlainMessage, ConvertLogLevel(logItem.LogLevel), logItem.EventId, logItem.Category);
        }

        private static EventLogEntryType ConvertLogLevel(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Trace)
            {
                return EventLogEntryType.Information;
            }
            if (logLevel == LogLevel.Debug)
            {
                return EventLogEntryType.Information;
            }
            if (logLevel == LogLevel.Information)
            {
                return EventLogEntryType.Information;
            }
            if (logLevel == LogLevel.Warning)
            {
                return EventLogEntryType.Warning;
            }
            if (logLevel == LogLevel.Error)
            {
                return EventLogEntryType.Error;
            }
            if (logLevel == LogLevel.Critical)
            {
                return EventLogEntryType.Error;
            }
            throw new KeyNotFoundException($"Loglevel '{logLevel}' is not writeable to windows-eventlog");
        }
        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
        public override void Dispose()
        {
            Utilities.NoOperation();
        }
    }
}
