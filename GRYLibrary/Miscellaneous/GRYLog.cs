using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GRYLibrary
{
    public enum GRYLogLogLevel : int
    {
        Critical = 0,
        Exception = 1,
        Warning = 2,
        Information = 3,
        Verbose = 4,
        Debug = 5
    }
    public class GRYLog : IDisposable
    {
        EventLog _EventLog;
        public GRYLogConfiguration Configuration { get; set; }
        private readonly static object _LockObject = new object();
        private readonly bool _Initialized = false;
        private int _AmountOfErrors = 0;
        private int _AmountOfWarnings = 0;
        private readonly ConsoleColor _ConsoleDefaultColor;
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(string message, string fullMessage, GRYLogLogLevel level);
        private FileSystemWatcher _Watcher;
        private GRYLog(GRYLogConfiguration configuration, string configurationFile)
        {
            this._ConsoleDefaultColor = Console.ForegroundColor;
            this.Configuration = configuration;
            if (this.Configuration.ReloadConfigurationWhenSourceFileWillBeChanged && File.Exists(configurationFile))
            {
                StartFileWatcherForConfigurationFile(configurationFile);
            }
            _EventLog = new EventLog(this.Configuration.ApplicationNameForWindowsEventViewer)
            {
                Source = this.Configuration.NameOfSourceForWindowsEventViewer
            };
            this._Initialized = true;
        }
        public static GRYLog Create()
        {
            return new GRYLog(new GRYLogConfiguration(), string.Empty);
        }
        public static GRYLog Create(string logFile)
        {
            var configuration = new GRYLogConfiguration
            {
                LogFile = logFile
            };
            return new GRYLog(configuration, string.Empty);
        }
        public static GRYLog CreateByConfigurationFile(string configurationFile)
        {
            return new GRYLog(GRYLogConfiguration.LoadConfiguration(configurationFile), configurationFile);
        }

        private void StartFileWatcherForConfigurationFile(string configurationFile)
        {
            _Watcher = new FileSystemWatcher
            {
                Path = configurationFile,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.*"
            };
            _Watcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs eventArgs) =>
            {
                try
                {
                    _Watcher.EnableRaisingEvents = false;
                    this.Configuration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                }
                catch (Exception exception)
                {
                    this.LogError("Could not reload Configuration", exception);
                }
                finally
                {
                    _Watcher.EnableRaisingEvents = true;
                }
            });
            _Watcher.EnableRaisingEvents = true;
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
        public void LogInformation(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (this.LineShouldBePrinted(message))
            {
                this.LogIt(message, GRYLogLogLevel.Information, logLineId);
            }
        }

        private bool LineShouldBePrinted(string message)
        {
            if (message == null)
            {
                return false;
            }

            if (this.Configuration.PrintEmptyLines)
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

            this.LogIt(message, GRYLogLogLevel.Debug, logLineId);
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
            this.LogIt(message, GRYLogLogLevel.Warning, logLineId);
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

            this.LogIt(message, GRYLogLogLevel.Verbose, logLineId);
        }

        public void LogCritical(string message, Exception exception, string logLineId = "")
        {
            if (!this.LineShouldBePrinted(message))
            {
                return;
            }
            this.LogCritical(this.GetExceptionMessage(message, exception), logLineId);
        }
        public void LogCritical(Exception exception, string logLineId = "")
        {
            this.LogCritical(this.GetExceptionMessage("An exception occurred", exception), logLineId);
        }
        public void LogCritical(string message, string logLineId = "")
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }
            if (this.Configuration.StoreErrorsInErrorQueueInsteadOfLoggingThem)
            {
                this._StoredErrors.Enqueue(new Tuple<string, string>(message, logLineId));
            }
            else
            {
                this.LogCriticalInternal(message, logLineId);
            }
        }

        private void LogCriticalInternal(string message, string logLineId)
        {
            LogErrorHelper(message, logLineId, GRYLogLogLevel.Critical);
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
        public void LogError(string message, string logLineId = "")
        {
            LogErrorHelper(message, logLineId, GRYLogLogLevel.Exception);
        }

        private void LogErrorHelper(string message, string logLineId, GRYLogLogLevel loglevel)
        {
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }
            if (this.Configuration.StoreErrorsInErrorQueueInsteadOfLoggingThem)
            {
                this._StoredErrors.Enqueue(new Tuple<string, string>(message, logLineId));
            }
            else
            {
                this.LogErrorInternal(message, logLineId, GRYLogLogLevel.Critical);
            }
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
                this.LogErrorInternal(dequeuedError.Item1, dequeuedError.Item2, GRYLogLogLevel.Exception);
            }
        }

        private void LogErrorInternal(string message, string logLineId, GRYLogLogLevel errorLogLevel)
        {
            if (this.Configuration.PrintErrorsAsInformation)
            {
                this.LogIt(message, GRYLogLogLevel.Information, logLineId);
            }
            else
            {
                this._AmountOfErrors = this._AmountOfErrors + 1;
                this.LogIt(message, errorLogLevel, logLineId);
            }
        }

        private void LogIt(string message, GRYLogLogLevel logLevel, string logLineId)
        {
            if (!this._Initialized)
            {
                return;
            }
            DateTime momentOfLogEntry = DateTime.Now;
            if (this.Configuration.ConvertTimeForLogentriesToUTCFormat)
            {
                momentOfLogEntry = momentOfLogEntry.ToUniversalTime();
            }
            message = message.Trim();
            string originalMessage = message;
            logLineId = logLineId.Trim();
            if (!string.IsNullOrEmpty(logLineId))
            {
                message = "[" + logLineId + "] " + message;
            }
            if (this.Configuration.AddIdToEveryLogEntry)
            {
                message = "[" + this.GetLogItemId() + "] " + message;
            }
            string part1 = "[" + momentOfLogEntry.ToString(this.Configuration.DateFormat) + "] [";
            string part2 = this.GetPrefixInStringFormat(logLevel);
            string part3 = "] " + message;
            lock (_LockObject)
            {
                string textForLogFileAndEventViewer;
                if (this.Configuration.LogOverhead)
                {
                    textForLogFileAndEventViewer = part1 + part2 + part3;
                }
                else
                {
                    textForLogFileAndEventViewer = originalMessage;
                }
                if (this.Configuration.WriteToLogFileIfLogFileIsAvailable && this.Configuration.LoggedMessageTypesInLogFile.Contains(logLevel) && (File.Exists(this.Configuration.LogFile) || this.Configuration.CreateLogFileIfRequiredAndIfPossible))
                {
                    if (!File.Exists(this.Configuration.LogFile))
                    {
                        if (this.Configuration.CreateLogFileIfRequiredAndIfPossible)
                        {
                            if (string.IsNullOrWhiteSpace(this.Configuration.LogFile))
                            {
                                throw new FileNotFoundException($"LogFile '{this.Configuration.LogFile}' is no valid file-path");
                            }
                            else
                            {
                                string directoryOfLogFile = Path.GetDirectoryName(this.Configuration.LogFile);
                                if (!(string.IsNullOrWhiteSpace(directoryOfLogFile) || Directory.Exists(directoryOfLogFile)))
                                {
                                    Directory.CreateDirectory(directoryOfLogFile);
                                }
                                Utilities.EnsureFileExists(this.Configuration.LogFile);
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException($"LogFile '{this.Configuration.LogFile}' is not available.");
                        }
                    }
                    try
                    {
                        File.AppendAllLines(this.Configuration.LogFile, new string[] { textForLogFileAndEventViewer }, this.Configuration.EncodingForLogfile);
                    }
                    catch
                    {
                        if (!this.Configuration.IgnoreErrorsWhileWritingLogItems)
                        {
                            throw;
                        }
                    }
                }
                if (this.Configuration.WriteLogEntriesToWindowsEventViewer && this.Configuration.LoggedMessageTypesInWindowsEventViewer.Contains(logLevel))
                {
                    try
                    {
                        _EventLog.WriteEntry(textForLogFileAndEventViewer, GetEventType(logLevel));
                    }
                    catch
                    {
                        if (!this.Configuration.IgnoreErrorsWhileWritingLogItems)
                        {
                            throw;
                        }
                    }
                }
                if (this.Configuration.PrintOutputInConsole && this.Configuration.LoggedMessageTypesInConsole.Contains(logLevel))
                {
                    try
                    {
                        if (this.Configuration.LogOverhead)
                        {
                            Console.Write(part1);
                            this.WriteWithColorToConsole(part2, logLevel);
                            Console.Write(part3 + Environment.NewLine);
                        }
                        else
                        {
                            Console.WriteLine(originalMessage);
                        }
                    }
                    catch
                    {
                        if (!this.Configuration.IgnoreErrorsWhileWritingLogItems)
                        {
                            throw;
                        }
                    }
                }
            }
            if (this.Configuration.DebugBreakMode && this.Configuration.DebugBreakLevel.Contains(logLevel) && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            if (this.Configuration.LogOverhead)
            {
                NewLogItem?.Invoke(originalMessage, part1 + part2 + part3, logLevel);
            }
            else
            {
                NewLogItem?.Invoke(originalMessage, originalMessage, logLevel);
            }
        }

        private EventLogEntryType GetEventType(GRYLogLogLevel logLevel)
        {
            if (logLevel == GRYLogLogLevel.Information)
            {
                return EventLogEntryType.Information;
            }
            if (logLevel == GRYLogLogLevel.Critical)
            {
                return EventLogEntryType.Error;
            }
            if (logLevel == GRYLogLogLevel.Exception)
            {
                return EventLogEntryType.Error;
            }
            if (logLevel == GRYLogLogLevel.Warning)
            {
                return EventLogEntryType.Warning;
            }
            if (logLevel == GRYLogLogLevel.Debug)
            {
                return EventLogEntryType.Information;
            }
            if (logLevel == GRYLogLogLevel.Verbose)
            {
                return EventLogEntryType.Information;
            }
            throw new KeyNotFoundException();
        }

        private string GetLogItemId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, this.Configuration.LogItemIdLength);
        }
        private string GetExceptionMessage(string message, Exception exception)
        {
            string result = string.Empty;
            if (!(message.EndsWith(".") | message.EndsWith("?") | message.EndsWith(":") | message.EndsWith("!")))
            {
                result = message + ".";
            }
            if (this.Configuration.WriteExceptionMessageOfExceptionInLogEntry)
            {
                result = result + " Exception-message: " + exception.Message;
            }
            if (this.Configuration.WriteExceptionStackTraceOfExceptionInLogEntry)
            {
                result = result + " (Exception-details: " + exception.ToString().Replace(Environment.NewLine, string.Empty) + ")";
            }
            return result;
        }
        private string GetPrefixInStringFormat(GRYLogLogLevel logLevel)
        {
            if (logLevel == GRYLogLogLevel.Critical)
            {
                return this.Configuration.CriticalPrefix;
            }
            if (logLevel == GRYLogLogLevel.Exception)
            {
                return this.Configuration.ErrorPrefix;
            }
            if (logLevel == GRYLogLogLevel.Information)
            {
                return this.Configuration.InformationPrefix;
            }
            if (logLevel == GRYLogLogLevel.Warning)
            {
                return this.Configuration.WarningPrefix;
            }
            if (logLevel == GRYLogLogLevel.Debug)
            {
                return this.Configuration.DebugPrefix;
            }
            if (logLevel == GRYLogLogLevel.Verbose)
            {
                return this.Configuration.VerbosePrefix;
            }
            throw new Exception("Invalid LogLevel");
        }

        private void WriteWithColorToConsole(string message, GRYLogLogLevel logLevel)
        {
            Console.ForegroundColor = this.GetColorByType(logLevel);
            Console.Write(message);
            Console.ForegroundColor = this._ConsoleDefaultColor;
        }

        private ConsoleColor GetColorByType(GRYLogLogLevel type)
        {
            if (type == GRYLogLogLevel.Critical)
            {
                return this.Configuration.ColorForCritical;
            }
            if (type == GRYLogLogLevel.Exception)
            {
                return this.Configuration.ColorForError;
            }
            if (type == GRYLogLogLevel.Information)
            {
                return this.Configuration.ColorForInfo;
            }
            if (type == GRYLogLogLevel.Warning)
            {
                return this.Configuration.ColorForWarning;
            }
            if (type == GRYLogLogLevel.Debug)
            {
                return this.Configuration.ColorForDebug;
            }
            if (type == GRYLogLogLevel.Verbose)
            {
                return this.Configuration.ColorForVerbose;
            }
            throw new Exception("Invalid LogLevel");
        }
        private bool CheckEnabled()
        {
            return this.Configuration.Enabled;
        }

        public void Dispose()
        {
            this._EventLog.Dispose();
            if (this._Watcher != null)
            {
                this._Watcher.Dispose();
            }
        }
    }
    public class GRYLogConfiguration
    {
        public GRYLogConfiguration()
        {
            this.Enabled = true;
            this.DebugBreakMode = false;
            this.LogFile = string.Empty;
            this.ConvertTimeForLogentriesToUTCFormat = false;
            this.WriteLogEntriesToWindowsEventViewer = false;
            this.EncodingForLogfile = new UTF8Encoding(false);
            this.DebugBreakLevel = new List<GRYLogLogLevel>() { GRYLogLogLevel.Exception };
            this.DateFormat = "yyyy-MM-dd HH:mm:ss";
            this.LoggedMessageTypesInConsole = new List<GRYLogLogLevel>();
            this.LoggedMessageTypesInLogFile = new List<GRYLogLogLevel>();
            this.LoggedMessageTypesInWindowsEventViewer = new List<GRYLogLogLevel>();
            this.CreateLogEntryWhenGRYLogWriteToLogFileWillBeEnabledOrDisabled = false;
            this.InformationPrefix = "Info";
            this.ErrorPrefix = "Error";
            this.DebugPrefix = "Debug";
            this.WarningPrefix = "Warning";
            this.VerbosePrefix = "Additional";
            this.CriticalPrefix = "Critical";
            this.LogOverhead = true;
            this.PrintEmptyLines = false;
            this.WriteExceptionMessageOfExceptionInLogEntry = true;
            this.WriteExceptionStackTraceOfExceptionInLogEntry = true;
            this.AddIdToEveryLogEntry = false;
            this.StoreErrorsInErrorQueueInsteadOfLoggingThem = false;
            this.PrintOutputInConsole = true;
            this.ColorForDebug = ConsoleColor.DarkBlue;
            this.ColorForError = ConsoleColor.Red;
            this.ColorForInfo = ConsoleColor.Green;
            this.ColorForWarning = ConsoleColor.DarkYellow;
            this.ColorForVerbose = ConsoleColor.Blue;
            this.ColorForCritical = ConsoleColor.DarkRed;
            this.LogItemIdLength = 8;
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Information);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Information);
            this.IgnoreErrorsWhileWritingLogItems = false;
            this.NameOfSourceForWindowsEventViewer = string.Empty;
            this.ApplicationNameForWindowsEventViewer = string.Empty;
            this.ReloadConfigurationWhenSourceFileWillBeChanged = true;
            this.CreateLogFileIfRequiredAndIfPossible = true;
            this.WriteToLogFileIfLogFileIsAvailable = true;
        }
        public string LogFile { get; set; }
        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenSourceFileWillBeChanged { get; set; }
        /// <summary>
        /// If true then <see cref="Debugger.Break"/> will be executed every time a logentry will be created whose loglevel is contained in <see cref="GRYLogConfiguration.DebugBreakLevel"/>. This feature is for debugging-purposes only.
        /// </summary>
        /// <remarks>
        /// Caution: Do not enable this function in productive-mode unless you know exactly what you are doing! Applications typically crash if <see cref="Debugger.Break"/> will be executed and no debugger is attached. For that reason this function is disabled by default.
        /// </remarks>
        public bool DebugBreakMode { get; set; }
        public bool Enabled { get; set; }
        public Encoding EncodingForLogfile { get; set; }
        public string InformationPrefix { get; set; }
        public string WarningPrefix { get; set; }
        public string ErrorPrefix { get; set; }
        public string CriticalPrefix { get; set; }
        public string DebugPrefix { get; set; }
        public string VerbosePrefix { get; set; }
        public bool IgnoreErrorsWhileWritingLogItems { get; set; }
        public int LogItemIdLength { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public bool StoreErrorsInErrorQueueInsteadOfLoggingThem { get; set; }
        public bool PrintOutputInConsole { get; set; }
        public bool WriteLogEntriesToWindowsEventViewer { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        /// <summary>
        /// If true then every log-entry gets a random id. This function is disabled by default.
        /// </summary>
        public bool AddIdToEveryLogEntry { get; set; }
        public bool CreateLogEntryWhenGRYLogWriteToLogFileWillBeEnabledOrDisabled { get; set; }
        public string DateFormat { get; set; }
        public IList<GRYLogLogLevel> DebugBreakLevel { get; set; }
        public IList<GRYLogLogLevel> LoggedMessageTypesInConsole { get; set; }
        public IList<GRYLogLogLevel> LoggedMessageTypesInLogFile { get; set; }
        public IList<GRYLogLogLevel> LoggedMessageTypesInWindowsEventViewer { get; set; }
        public ConsoleColor ColorForInfo { get; set; }
        public ConsoleColor ColorForWarning { get; set; }
        public ConsoleColor ColorForError { get; set; }
        public ConsoleColor ColorForDebug { get; set; }
        public ConsoleColor ColorForVerbose { get; set; }
        public ConsoleColor ColorForCritical { get; set; }
        /// <summary>
        /// If true then overhead like date/time will be added to every log-entry.
        /// </summary>
        public bool LogOverhead { get; set; }
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; }
        public string ApplicationNameForWindowsEventViewer { get; set; }
        public string NameOfSourceForWindowsEventViewer { get; set; }
        public bool WriteToLogFileIfLogFileIsAvailable { get; set; }
        public bool CreateLogFileIfRequiredAndIfPossible { get; set; }

        private static SimpleObjectPersistence<GRYLogConfiguration> _SimpleObjectPersistence = null;
        public static Encoding GRYLogConfigurationFileDefaultEncoding { get; set; } = new UTF8Encoding(false);
        public static GRYLogConfiguration LoadConfiguration(string configurationFile, Encoding encoding)
        {
            if (_SimpleObjectPersistence == null)
            {
                _SimpleObjectPersistence = new SimpleObjectPersistence<GRYLogConfiguration>(configurationFile, encoding);
            }
            return _SimpleObjectPersistence.Object;
        }
        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            return LoadConfiguration(configurationFile, GRYLogConfigurationFileDefaultEncoding);
        }
        public static void SavedConfiguration(string configurationFile, GRYLogConfiguration configuration, Encoding encoding)
        {
            if (_SimpleObjectPersistence == null)
            {
                _SimpleObjectPersistence = new SimpleObjectPersistence<GRYLogConfiguration>(configurationFile, encoding);
            }
            _SimpleObjectPersistence.Object = configuration;
            _SimpleObjectPersistence.SaveObject();
        }
        public static void SavedConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            SavedConfiguration(configurationFile, configuration, GRYLogConfigurationFileDefaultEncoding);
        }
    }
}