using GRYLibrary.GRYObjectSystem.Meta.Property;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace GRYLibrary
{
    public class GRYLog : IDisposable
    {
        public bool Enabled { get; set; }
        public Encoding Encoding { get; set; }
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(string message, string fullMessage, LogLevel level);
        public string InformationPrefix { get; set; }
        public string WarningPrefix { get; set; }
        public string ErrorPrefix { get; set; }
        public string DebugPrefix { get; set; }
        public string VerbosePrefix { get; set; }
        private string _LogFile;
        private bool _WriteToLogFile;
        private readonly bool _Initialized = false;
        public bool IgnoreErrorsWhileWriteLogItem { get; set; }
        public int LogItemIdLength { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public bool StoreErrorsInErrorQueueInsteadOfLoggingThem { get; set; }
        public bool PrintOutputInConsole { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        public bool AddIdToEveryLogEntry { get; set; }
        public bool WriteLogEntryWhenGRYLogWriteToLogFileWIllBeEnabledOrDisabled { get; set; }
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
        public bool DebugBreakMode { get; set; }
        public IList<LogLevel> DebugBreakLevel { get; set; }
        private readonly ConsoleColor _ConsoleDefaultColor;
        private Property<bool> _DequeueSchedulerEnabled = null;
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
                    Utilities.EnsureFileExists(newValue);
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
                    if (this.WriteLogEntryWhenGRYLogWriteToLogFileWIllBeEnabledOrDisabled)
                    {
                        if (value)
                        {
                            this.LogInformation($"{nameof(GRYLog)}.{nameof(this.WriteToLogFile)} is now enabled.");
                        }
                        else
                        {
                            this.LogInformation($"{nameof(GRYLog)}.{nameof(this.WriteToLogFile)} is now disabled.");
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
            this.LogInformation("Amount of occurred Errors: " + this.GetAmountOfErrors().ToString());
            this.LogInformation("Amount of occurred Warnings: " + this.GetAmountOfWarnings().ToString());
        }
        public GRYLog(string logFile)
        {
            this.Enabled = true;
            this.DebugBreakMode = false;
            this.ConvertTimeToUTCFormat = false;
            this.Encoding = new UTF8Encoding(false);
            this.DebugBreakLevel = new List<LogLevel>() { LogLevel.Exception };
            this._ConsoleDefaultColor = Console.ForegroundColor;
            this.DateFormat = "yyyy-MM-dd HH:mm:ss";
            this.LoggedMessageTypesInConsole = new List<LogLevel>();
            this.LoggedMessageTypesInLogFile = new List<LogLevel>();
            this.WriteLogEntryWhenGRYLogWriteToLogFileWIllBeEnabledOrDisabled = false;
            this.InformationPrefix = "Info";
            this.ErrorPrefix = "Error";
            this.DebugPrefix = "Debug";
            this.WarningPrefix = "Warning";
            this.VerbosePrefix = "Additional";
            this.LogOverhead = true;
            this.PrintEmptyLines = false;
            this.WriteExceptionMessageOfExceptionInLogEntry = true;
            this.WriteExceptionStackTraceOfExceptionInLogEntry = true;
            this.AddIdToEveryLogEntry = false;
            this.StoreErrorsInErrorQueueInsteadOfLoggingThem = false;
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
            this._DequeueSchedulerEnabled = new Property<bool>(true, nameof(_DequeueSchedulerEnabled), false)
            {
                LockEnabled = true
            };

            Thread dequeueThread = new Thread(Dequeue);
            dequeueThread.Name = $"{nameof(GRYLog)}-Thread.";
            dequeueThread.Start();
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
        private void Dequeue()
        {
            while (_DequeueSchedulerEnabled.Value)
            {
                if (_Queue.TryDequeue(out LogItem logitem))
                {
                    try
                    {
                        LogIt(logitem);
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                }
            }
        }

        public GRYLog() : this(string.Empty)
        {
        }
        public void LogInformation(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (this.LineShouldBePrinted(message))
            {
                this.EnqueueLogItem(message, LogLevel.Information, logLineId);
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
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }

            this.EnqueueLogItem(message, LogLevel.Debug, logLineId);
        }

        public void LogWarning(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }

            this._AmountOfWarnings = this._AmountOfWarnings + 1;
            this.EnqueueLogItem(message, LogLevel.Warning, logLineId);
        }
        public void LogVerboseMessage(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }

            this.EnqueueLogItem(message, LogLevel.Verbose, logLineId);
        }
        public void LogError(string message, Exception exception, string logLineId = "")
        {
            if (!this.LineShouldBePrinted(message))
            {
                return;
            }
            this.LogError(this.GetExceptionMessage(message, exception), logLineId);
        }
        public void LogError(Exception exception, string logLineId = "")
        {
            this.LogError(this.GetExceptionMessage("An exception occurred", exception), logLineId);
        }
        private readonly Queue<Tuple<string, string>> _StoredErrors = new Queue<Tuple<string, string>>();
        public void PrintErrorQueue()
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            while (this._StoredErrors.Count != 0)
            {
                Tuple<string, string> dequeuedError = this._StoredErrors.Dequeue();
                this.LogErrorInternal(dequeuedError.Item1, dequeuedError.Item2);
            }
        }
        public void LogError(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }
            if (this.StoreErrorsInErrorQueueInsteadOfLoggingThem)
            {
                this._StoredErrors.Enqueue(new Tuple<string, string>(message, logLineId));
            }
            else
            {
                this.LogErrorInternal(message, logLineId);
            }
        }

        private void LogErrorInternal(string message, string logLineId)
        {
            if (this.PrintErrorsAsInformation)
            {
                this.EnqueueLogItem(message, LogLevel.Information, logLineId);
            }
            else
            {
                this._AmountOfErrors = this._AmountOfErrors + 1;
                this.EnqueueLogItem(message, LogLevel.Exception, logLineId);
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
        private ConcurrentQueue<LogItem> _Queue = new ConcurrentQueue<LogItem>();
        private void EnqueueLogItem(string message, LogLevel logLevel, string logLineId)
        {
            _Queue.Enqueue(new LogItem(message, logLevel, logLineId));
        }

        private void LogIt(LogItem logItem)
        {
            if (!this._Initialized)
            {
                return;
            }
            DateTime momentOfLogEntry = DateTime.Now;
            if (this.ConvertTimeToUTCFormat)
            {
                momentOfLogEntry = momentOfLogEntry.ToUniversalTime();
            }
            logItem.Message = logItem.Message.Trim();
            string originalMessage = logItem.Message;
            logItem.LogLineId = logItem.LogLineId.Trim();
            if (!string.IsNullOrEmpty(logItem.LogLineId))
            {
                logItem.Message = "[" + logItem.LogLineId + "] " + logItem.Message;
            }
            if (this.AddIdToEveryLogEntry)
            {
                logItem.Message = "[" + this.GetLogItemId() + "] " + logItem.Message;
            }
            string part1 = "[" + momentOfLogEntry.ToString(this.DateFormat) + "] [";
            string part2 = this.GetPrefixInStringFormat(logItem.LogLevel);
            string part3 = "] " + logItem.Message;
            if (this.PrintOutputInConsole && this.LoggedMessageTypesInConsole.Contains(logItem.LogLevel))
            {
                if (this.LogOverhead)
                {
                    Console.Write(part1);
                    this.WriteWithColorToConsole(part2, logItem.LogLevel);
                    Console.Write(part3 + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine(originalMessage);
                }
            }
            if (this.WriteToLogFile && this.LoggedMessageTypesInLogFile.Contains(logItem.LogLevel))
            {
                try
                {
                    if (this.LogOverhead)
                    {
                        File.AppendAllLines(this.LogFile, new string[] { part1 + part2 + part3 }, this.Encoding);
                    }
                    else
                    {
                        File.AppendAllLines(this.LogFile, new string[] { originalMessage }, this.Encoding);
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
            if (this.DebugBreakMode && this.DebugBreakLevel.Contains(logItem.LogLevel) && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            if (this.LogOverhead)
            {
                NewLogItem?.Invoke(originalMessage, part1 + part2 + part3, logItem.LogLevel);
            }
            else
            {
                NewLogItem?.Invoke(originalMessage, originalMessage, logItem.LogLevel);
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

        private void WriteWithColorToConsole(string message, LogLevel logLevel)
        {
            Console.ForegroundColor = this.GetColorByType(logLevel);
            Console.Write(message);
            Console.ForegroundColor = this._ConsoleDefaultColor;
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
        private bool CheckEnabled()
        {
            return this.Enabled;
        }

        public void Dispose()
        {
            this.Enabled = false;
            SpinWait.SpinUntil(() => _Queue.IsEmpty);
            this._DequeueSchedulerEnabled.Value = false;
        }

        private class LogItem
        {
            public LogItem(string message, LogLevel logLevel, string logLineId)
            {
                this.Message = message;
                this.LogLevel = logLevel;
                this.LogLineId = logLineId;
            }

            public string Message { get; internal set; }
            public LogLevel LogLevel { get; internal set; }
            public string LogLineId { get; internal set; }
        }

    }
}