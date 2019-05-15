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
        private readonly EventLog _EventLog;
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
            if (string.IsNullOrWhiteSpace(configuration.LogFile) && configuration.CreateLogFileIfRequiredAndIfPossible)
            {
                configuration.WriteToLogFileIfLogFileIsAvailable = false;
            }
            this._ConsoleDefaultColor = Console.ForegroundColor;
            this.Configuration = configuration;
            if (this.Configuration.ReloadConfigurationWhenSourceFileWillBeChanged && File.Exists(configurationFile))
            {
                this.StartFileWatcherForConfigurationFile(configurationFile);
            }
            this._EventLog = new EventLog(this.Configuration.ApplicationNameForWindowsEventViewer)
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
            GRYLogConfiguration configuration = new GRYLogConfiguration
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
            this._Watcher = new FileSystemWatcher
            {
                Path = configurationFile,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.*"
            };
            this._Watcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs eventArgs) =>
            {
                try
                {
                    this._Watcher.EnableRaisingEvents = false;
                    this.Configuration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                }
                catch (Exception exception)
                {
                    this.Log("Could not reload Configuration", GRYLogLogLevel.Exception, exception);
                }
                finally
                {
                    this._Watcher.EnableRaisingEvents = true;
                }
            });
            this._Watcher.EnableRaisingEvents = true;
        }

        public int GetAmountOfErrors()
        {
            return this._AmountOfErrors;
        }
        public int GetAmountOfWarnings()
        {
            return this._AmountOfWarnings;
        }
        public void SummarizeLog()
        {
            this.Log("Amount of occurred Errors and Criticals: " + this.GetAmountOfErrors().ToString(), GRYLogLogLevel.Information);
            this.Log("Amount of occurred Warnings: " + this.GetAmountOfWarnings().ToString(), GRYLogLogLevel.Information);
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

        public void Log(string message)
        {
            Log(message, GRYLogLogLevel.Information);
        }
        public void Log(string message, Exception exception)
        {
            this.Log(message, GRYLogLogLevel.Exception, exception);
        }
        public void Log(Exception exception)
        {
            Log(GRYLogLogLevel.Exception, exception);
        }
        public void Log(GRYLogLogLevel logLevel, Exception exception)
        {
            Log("An exception occurred", logLevel, exception);
        }
        public void Log(string message, GRYLogLogLevel logLevel, Exception exception)
        {
            Log(GetExceptionMessage(message, exception), logLevel);
        }
        public void Log(string message, GRYLogLogLevel logLevel)
        {
            if (!this._Initialized)
            {
                return;
            }
            if (!this.CheckEnabled())
            {
                return;
            }

            if (!this.LineShouldBePrinted(message))
            {
                return;
            }

            if (!( this.Configuration.LoggedMessageTypesInLogFile.Contains(logLevel)
                || this.Configuration.LoggedMessageTypesInWindowsEventViewer.Contains(logLevel)
                || this.Configuration.LoggedMessageTypesInConsole.Contains(logLevel) 
                || this.Configuration.DebugBreakLevel.Contains(logLevel)))
            {
                return;
            }
            if (this.Configuration.PrintErrorsAsInformation)
            {
                if (logLevel.Equals(GRYLogLogLevel.Exception))
                {
                    this._AmountOfErrors += 1;
                }
            }
            if (logLevel.Equals(GRYLogLogLevel.Exception))
            {
                logLevel = GRYLogLogLevel.Information;
            }
            DateTime momentOfLogEntry = DateTime.Now;
            if (this.Configuration.ConvertTimeForLogentriesToUTCFormat)
            {
                momentOfLogEntry = momentOfLogEntry.ToUniversalTime();
            }
            message = message.Trim();
            string originalMessage = message;
            if (!string.IsNullOrEmpty(this.Configuration.Name))
            {
                message = "[" + this.Configuration.Name.Trim() + "] " + message;
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
                    if (this.Configuration.LogFile == null)
                    {
                        throw new NullReferenceException($"LogFile is null");
                    }
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
                    File.AppendAllLines(this.Configuration.LogFile, new string[] { textForLogFileAndEventViewer }, this.Configuration.EncodingForLogfile);
                }
                if (this.Configuration.WriteLogEntriesToWindowsEventViewer && this.Configuration.LoggedMessageTypesInWindowsEventViewer.Contains(logLevel))
                {
                    this._EventLog.WriteEntry(textForLogFileAndEventViewer, this.GetEventType(logLevel));
                }
                if (this.Configuration.PrintOutputInConsole && this.Configuration.LoggedMessageTypesInConsole.Contains(logLevel))
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
                result = result + " (Exception-details: " + exception.StackTrace.Replace(Environment.NewLine, " ") + ")";
            }
            return result;
        }
        private string GetPrefixInStringFormat(GRYLogLogLevel logLevel)
        {
            string result = string.Empty;
            if (logLevel == GRYLogLogLevel.Critical)
            {
                result = this.Configuration.CriticalPrefix;
            }
            if (logLevel == GRYLogLogLevel.Exception)
            {
                result = this.Configuration.ErrorPrefix;
            }
            if (logLevel == GRYLogLogLevel.Information)
            {
                result = this.Configuration.InformationPrefix;
            }
            if (logLevel == GRYLogLogLevel.Warning)
            {
                result = this.Configuration.WarningPrefix;
            }
            if (logLevel == GRYLogLogLevel.Debug)
            {
                result = this.Configuration.DebugPrefix;
            }
            if (logLevel == GRYLogLogLevel.Verbose)
            {
                result = this.Configuration.VerbosePrefix;
            }
            if (result.Length > this.Configuration.MaximalLengthOfPrefixes)
            {
                return result.Substring(0, this.Configuration.MaximalLengthOfPrefixes);
            }
            else
            {
                return result;
            }
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
        public void ExecuteAndLog(Action action, string nameOfAction)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", GRYLogLogLevel.Information);
            try
            {
                action();
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", GRYLogLogLevel.Exception, exception);
                throw;
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished.", GRYLogLogLevel.Information);
            }
        }
        public void ExecuteAndLog<TParameter>(Action<TParameter> action, string nameOfAction, TParameter argument)
        {
            string argumentAsString = argument.ToString();
            this.Log($"Action '{nameOfAction}({argumentAsString})' will be started now.");
            try
            {
                action(argument);
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}({argumentAsString})'.", GRYLogLogLevel.Exception, exception);
                throw;
            }
            finally
            {
                this.Log($"Action '{nameOfAction}({argumentAsString})' finished.");
            }
        }
        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction)
        {
            this.Log($"Action '{nameOfAction}' will be started now.");
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", GRYLogLogLevel.Exception, exception);
                throw;
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished.");
            }
        }
        public TOut ExecuteAndLog<TParameter, TOut>(Func<TParameter, TOut> action, string nameOfAction, TParameter argument)
        {
            string argumentAsString = argument.ToString();
            this.Log($"Action '{nameOfAction}({argumentAsString})' will be started now.");
            try
            {
                return action(argument);
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}({argumentAsString})'.", GRYLogLogLevel.Exception, exception);
                throw;
            }
            finally
            {
                this.Log($"Action '{nameOfAction}({argumentAsString})' finished.");
            }
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
            this.InformationPrefix = "Information";
            this.ErrorPrefix = "Error";
            this.DebugPrefix = "Debug";
            this.WarningPrefix = "Warning";
            this.VerbosePrefix = "Verbose";
            this.CriticalPrefix = "Critical";
            this.MaximalLengthOfPrefixes = 30;
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
            this.Name = string.Empty;
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Information);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Critical);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Information);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Critical);
            this.NameOfSourceForWindowsEventViewer = string.Empty;
            this.ApplicationNameForWindowsEventViewer = string.Empty;
            this.ReloadConfigurationWhenSourceFileWillBeChanged = true;
            this.CreateLogFileIfRequiredAndIfPossible = true;
            this.WriteToLogFileIfLogFileIsAvailable = true;
            this.PrintErrorsAsInformation = false;
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
        public int LogItemIdLength { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public string Name { get; set; }
        public bool StoreErrorsInErrorQueueInsteadOfLoggingThem { get; set; }
        public bool PrintOutputInConsole { get; set; }
        public bool WriteLogEntriesToWindowsEventViewer { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        public int MaximalLengthOfPrefixes { get; set; }
        /// <summary>
        /// If true then every log-entry gets a random id. This function is disabled by default.
        /// </summary>
        public bool AddIdToEveryLogEntry { get; set; }
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

        public static Encoding GRYLogConfigurationFileDefaultEncoding { get; set; } = new UTF8Encoding(false);
        public static GRYLogConfiguration LoadConfiguration(string configurationFile, Encoding encoding)
        {
            return new SimpleObjectPersistence<GRYLogConfiguration>(configurationFile, encoding).Object;
        }
        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            return LoadConfiguration(configurationFile, GRYLogConfigurationFileDefaultEncoding);
        }
        public static void SavedConfiguration(string configurationFile, GRYLogConfiguration configuration, Encoding encoding)
        {
            new SimpleObjectPersistence<GRYLogConfiguration>(configurationFile, encoding)
            {
                Object = configuration
            }.SaveObject();
        }
        public static void SavedConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            SavedConfiguration(configurationFile, configuration, GRYLogConfigurationFileDefaultEncoding);
        }
    }
}