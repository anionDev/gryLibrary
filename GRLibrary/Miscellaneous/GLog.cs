using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GRLibrary
{
    public class GLog
    {
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(string message, string fullMessage, LogLevel level);
        public string InformationPrefix { get; set; }
        public string WarningPrefix { get; set; }
        public string ErrorPrefix { get; set; }
        public string DebugPrefix { get; set; }
        public string VerbosePrefix { get; set; }
        private string _LogFile;
        private bool _WriteToLogFile;
        private readonly object _LockObject = new object();
        private readonly bool _Initialized = false;
        public bool IgnoreErrorsWhileWriteLogItem { get; set; }
        public int LogItemIdLength { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public bool PrintOutputInConsole { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        public bool AddIdToEveryLogEntry { get; set; }
        public bool WriteLogEntryWhenGLogWIllBeEnabledOrDisabled { get; set; }
        public string DateFormat { get; set; }
        public IList<LogLevel> LoggedMessageTypesInConsole { get; set; }
        public IList<LogLevel> LoggedMessageTypesInLogFile { get; set; }
        public ConsoleColor ColorForInfo { get; set; }
        public ConsoleColor ColorForWarning { get; set; }
        public ConsoleColor ColorForError { get; set; }
        public ConsoleColor ColorForDebug { get; set; }
        public ConsoleColor ColorForVerbose { get; set; }
        public bool LogOverhead { get; set; }
        public bool ConvertTimeToUTCFormat { get; set; }
        private int _AmountOfErrors = 0;
        private int _AmountOfWarnings = 0;
        public string LogFile
        {
            get
            {
                return this._LogFile;
            }
            set
            {
                string newValue = value;
                if (!File.Exists(newValue))
                {
                    string directoryOfLogFile = Path.GetDirectoryName(newValue);
                    if (!(string.IsNullOrWhiteSpace(directoryOfLogFile) || Directory.Exists(directoryOfLogFile)))
                    {
                        Directory.CreateDirectory(directoryOfLogFile);
                    }
                    File.Create(newValue).Dispose();
                }
                this._LogFile = newValue;
            }
        }
        public bool WriteToLogFile
        {
            get
            {
                return this._WriteToLogFile;
            }
            set
            {
                if (value != this.WriteToLogFile)
                {
                    this._WriteToLogFile = value;
                    if (this.WriteLogEntryWhenGLogWIllBeEnabledOrDisabled)
                    {

                        if (value)
                        {
                            LogInformation("GLog.WriteToLogFile is now enabled.");
                        }
                        else
                        {
                            LogInformation("GLog.WriteToLogFile is now disabled.");
                        }
                    }
                }
            }
        }
        public int GetAmountOfErrors()
        {
            return this._AmountOfErrors;
        }
        public int GetAmountOfWarnings()
        {
            return this._AmountOfWarnings;
        }
        public void Summarize()
        {
            LogInformation("Amount of occurred Errors: " + GetAmountOfErrors().ToString());
            LogInformation("Amount of occurred Warnings: " + GetAmountOfWarnings().ToString());
        }
        public GLog(string logFile)
        {
            this.DateFormat = "yyyy/MM/dd HH:mm:ss";
            this.LoggedMessageTypesInConsole = new List<LogLevel>();
            this.LoggedMessageTypesInLogFile = new List<LogLevel>();
            this.WriteLogEntryWhenGLogWIllBeEnabledOrDisabled = false;
            this.InformationPrefix = "Info";
            this.ErrorPrefix = "Error";
            this.DebugPrefix = "Debug";
            this.WarningPrefix = "Warning";
            this.VerbosePrefix = "Additional";
            this.LogOverhead = true;
            this.WriteExceptionMessageOfExceptionInLogEntry = true;
            this.WriteExceptionStackTraceOfExceptionInLogEntry = true;
            this.AddIdToEveryLogEntry = false;
            this.PrintOutputInConsole = true;
            this.ColorForDebug = ConsoleColor.DarkBlue;
            this.ColorForError = ConsoleColor.DarkRed;
            this.ColorForInfo = ConsoleColor.Green;
            this.ColorForWarning = ConsoleColor.DarkYellow;
            this.ColorForVerbose = ConsoleColor.Blue;
            this.LogItemIdLength = 8;
            this.LoggedMessageTypesInConsole.Add(LogLevel.Exception);
            this.LoggedMessageTypesInConsole.Add(LogLevel.Warning);
            this.LoggedMessageTypesInConsole.Add(LogLevel.Information);
            this.LoggedMessageTypesInLogFile.Add(LogLevel.Exception);
            this.LoggedMessageTypesInLogFile.Add(LogLevel.Warning);
            this.LoggedMessageTypesInLogFile.Add(LogLevel.Information);
            this.IgnoreErrorsWhileWriteLogItem = false;

            if (string.IsNullOrWhiteSpace(logFile))
            {
                this.WriteToLogFile = false;
                this._LogFile = logFile;
            }
            else
            {
                this.LogFile = logFile;
                this.WriteToLogFile = true;
            }
            this._Initialized = true;
        }
        public GLog() : this(string.Empty)
        {
        }
        public void LogInformation(string message, string logLineId = "")
        {
            if (LineShouldBePrinted(message))
            {
                LogIt(message, LogLevel.Information, logLineId);
            }
        }

        private bool LineShouldBePrinted(string message)
        {
            if (message == null)
            {
                return false;
            }

            if (this.PrintEmptyLines)
            {
                return true;
            }
            else
            {
                return !string.IsNullOrWhiteSpace(message);
            }
        }
        public void LogDebugInformation(string message, string logLineId = "")
        {
            if (!LineShouldBePrinted(message))
            {
                return;
            }

            LogIt(message, LogLevel.Debug, logLineId);
        }

        public void LogWarning(string message, string logLineId = "")
        {
            if (!LineShouldBePrinted(message))
            {
                return;
            }

            this._AmountOfWarnings = this._AmountOfWarnings + 1;
            LogIt(message, LogLevel.Warning, logLineId);
        }
        public void LogVerboseMessage(string message, string logLineId = "")
        {
            if (!LineShouldBePrinted(message))
            {
                return;
            }

            LogIt(message, LogLevel.Verbose, logLineId);
        }
        public void LogError(string message, Exception exception, string logLineId = "")
        {
            if (!LineShouldBePrinted(message))
            {
                return;
            }
            LogError(GetExceptionMessage(message, exception), logLineId);
        }
        public void LogError(Exception exception, string logLineId = "")
        {
            LogError(GetExceptionMessage("An exception occurred", exception), logLineId);
        }
        public void LogError(string message, string logLineId = "")
        {
            if (!LineShouldBePrinted(message))
            {
                return;
            }

            if (this.PrintErrorsAsInformation)
            {
                LogIt(message, LogLevel.Information, logLineId);
            }
            else
            {
                this._AmountOfErrors = this._AmountOfErrors + 1;
                LogIt(message, LogLevel.Exception, logLineId);
            }
        }
        public enum LogLevel : int
        {
            Exception = 0,
            Warning = 1,
            Information = 2,
            Verbose = 3,
            Debug = 4
        }
        private void LogIt(string message, LogLevel logLevel, string logLineId)
        {
            if (!this._Initialized)
            {
                return;
            }
            DateTime momentOfLogEntry = DateTime.Now;
            message = message.Trim();
            string originalMessage = message;
            logLineId = logLineId.Trim();
            if (!string.IsNullOrEmpty(logLineId))
            {
                message = "[" + logLineId + "] " + message;
            }
            if (this.AddIdToEveryLogEntry)
            {
                message = "[" + GetLogItemId() + "] " + message;
            }
            string part1 = "[" + momentOfLogEntry.ToString(this.DateFormat) + "] [";
            string part2 = GetPrefixInStringFormat(logLevel);
            string part3 = "] " + message;
            lock (this._LockObject)
            {
                if (this.PrintOutputInConsole && this.LoggedMessageTypesInConsole.Contains(logLevel))
                {
                    if (this.LogOverhead)
                    {
                        Console.Write(part1);
                        WriteWithColor(part2, logLevel);
                        Console.Write(part3 + Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine(originalMessage);
                    }
                }
                if (this.WriteToLogFile && this.LoggedMessageTypesInLogFile.Contains(logLevel))
                {
                    try
                    {
                        if (!File.Exists(this.LogFile))
                        {
                            using (File.Create(this.LogFile))
                            {
                            }
                        }
                        if (this.LogOverhead)
                        {
                            File.AppendAllLines(this.LogFile, new string[] { part1 + part2 + part3 }, Encoding.UTF8);
                        }
                        else
                        {
                            File.AppendAllLines(this.LogFile, new string[] { originalMessage }, Encoding.UTF8);
                        }
                    }
                    catch
                    {
                        if (!this.IgnoreErrorsWhileWriteLogItem)
                        {
                            throw;
                        }
                    }
                }
            }
            if (this.LogOverhead)
            {
                NewLogItem?.Invoke(originalMessage, part1 + part2 + part3, logLevel);
            }
            else
            {
                NewLogItem?.Invoke(originalMessage, originalMessage, logLevel);
            }
        }
        private string GetLogItemId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, this.LogItemIdLength);
        }
        private string GetExceptionMessage(string message, Exception exception)
        {
            string result = string.Empty;
            if (!(message.EndsWith(".") | message.EndsWith("?") | message.EndsWith(":") | message.EndsWith("!")))
            {
                result = message + ".";
            }
            if (this.WriteExceptionMessageOfExceptionInLogEntry)
            {
                result = result + " Exception-message: " + exception.Message;
            }
            if (this.WriteExceptionStackTraceOfExceptionInLogEntry)
            {
                result = result + " (Exception-details: " + exception.ToString().Replace(Environment.NewLine, string.Empty) + ")";
            }
            return result;
        }
        private string GetPrefixInStringFormat(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Exception)
            {
                return this.ErrorPrefix;
            }
            if (logLevel == LogLevel.Information)
            {
                return this.InformationPrefix;
            }
            if (logLevel == LogLevel.Warning)
            {
                return this.WarningPrefix;
            }
            if (logLevel == LogLevel.Debug)
            {
                return this.DebugPrefix;
            }
            if (logLevel == LogLevel.Verbose)
            {
                return this.VerbosePrefix;
            }
            throw new Exception("Invalid LogLevel");
        }

        private void WriteWithColor(string part2, LogLevel type)
        {
            ConsoleColor consoleForegroundColorBeforeCHanging = Console.ForegroundColor;
            Console.ForegroundColor = GetColorByType(type);
            Console.Write(part2);
            Console.ForegroundColor = consoleForegroundColorBeforeCHanging;
        }

        private ConsoleColor GetColorByType(LogLevel type)
        {
            if (type == LogLevel.Exception)
            {
                return this.ColorForError;
            }
            if (type == LogLevel.Information)
            {
                return this.ColorForInfo;
            }
            if (type == LogLevel.Warning)
            {
                return this.ColorForWarning;
            }
            if (type == LogLevel.Debug)
            {
                return this.ColorForDebug;
            }
            if (type == LogLevel.Verbose)
            {
                return this.ColorForVerbose;
            }
            throw new Exception("Invalid LogLevel");
        }
    }
}