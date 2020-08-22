﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class WindowsEventLog : GRYLogTarget
    {
        public WindowsEventLog() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            using EventLog eventLog = new EventLog(Utilities.GetNameOfCurrentExecutable()) { Source = logObject.Configuration.Name };
            eventLog.WriteEntry(logItem.PlainMessage, this.ConvertLogLevel(logItem.LogLevel), logItem.EventId, logItem.Category);
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
            throw new KeyNotFoundException($"Loglevel '{logLevel}' is not writeable to windows-eventlog");
        }
        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
    }
}
