using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    internal class WindowsEventLog : GRYLogTarget
    {
        private WindowsEventLog() { }
        public static WindowsEventLog Instance { get; } = new WindowsEventLog();
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = logObject.Configuration.Name;
                eventLog.WriteEntry(logItem.PlainMessage, ConvertLogLevel(logItem.LogLevel), logItem.EventId, logItem.Category);
            }
        }

        private EventLogEntryType ConvertLogLevel(LogLevel logLevel)
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
            throw new KeyNotFoundException($"Loglevel '{logLevel.ToString()}' is not writeable to windows-eventlog");
        }
    }
}
