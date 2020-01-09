using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary
{
    public enum GRYLogLogFormat : int
    {
        OnlyMessage = 0,
        GRYLogFormat = 1,
        DateOnly = 2,
    }
    public class GRYLog : IDisposable, ILogger
    {
        public GRYLogConfiguration Configuration { get; set; }
        private readonly static object _LockObject = new object();
        private readonly bool _Initialized = false;
        private int _AmountOfErrors = 0;
        private int _AmountOfWarnings = 0;
        private readonly ConsoleColor _ConsoleDefaultColor;
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(string message, string fullMessage, LogLevel level);
        private FileSystemWatcher _Watcher;
        private GRYLog(GRYLogConfiguration configuration, string configurationFile)
        {
            lock (_LockObject)
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
                this._Initialized = true;
            }
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
            configurationFile = Utilities.ResolveToFullPath(configurationFile);
            this._Watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(configurationFile),
                Filter = Path.GetFileName(configurationFile),
                NotifyFilter = NotifyFilters.LastWrite,
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
                    this.Log($"Could not reload Configuration of {nameof(GRYLog)} stored in {configurationFile}", LogLevel.Error, exception);
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
            this.Log("Amount of occurred Errors and Criticals: " + this.GetAmountOfErrors().ToString(), LogLevel.Information);
            this.Log("Amount of occurred Warnings: " + this.GetAmountOfWarnings().ToString(), LogLevel.Information);
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
            this.Log(message, LogLevel.Information);
        }
        public void Log(string message, Exception exception)
        {
            this.Log(message, LogLevel.Error, exception);
        }
        public void Log(string message, LogLevel logLevel, Exception exception)
        {
            this.Log(this.GetExceptionMessage(message, exception), logLevel);
        }
        public void Log(string message, LogLevel logLevel)
        {
            this.Log(() => message, logLevel);
        }

        public void Log(Func<string> getMessage)
        {
            this.Log(getMessage, LogLevel.Information);
        }
        public void Log(Func<string> getMessage, Exception exception)
        {
            this.Log(getMessage, LogLevel.Error, exception);
        }
        public void Log(Exception exception)
        {
            this.Log(LogLevel.Error, exception);
        }
        public void Log(LogLevel logLevel, Exception exception)
        {
            this.Log(() => "An exception occurred", logLevel, exception);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel, Exception exception)
        {
            this.Log(() => this.GetExceptionMessage(getMessage(), exception), logLevel);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel)
        {
            if (LogLevel.None == logLevel)
            {
                return;
            }
            DateTime momentOfLogEntry = DateTime.Now;
            lock (_LockObject)
            {
                if (!this._Initialized)
                {
                    return;
                }
                if (!this.Configuration.Enabled)
                {
                    return;
                }
                if (!this.IsEnabled(logLevel))
                {
                    return;
                }
                string message = getMessage();
                if (!this.LineShouldBePrinted(message))
                {
                    return;
                }
                message = message.Trim();
                string originalMessage = message;
                if (message.Contains(Environment.NewLine) && this.Configuration.LogEveryLineOfLogEntryInNewLine)
                {
                    foreach (string line in message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        this.Log(line, logLevel);
                    }
                    return;
                }

                if (this.Configuration.PrintErrorsAsInformation && this.IsErrorLogLevel(logLevel))
                {
                    logLevel = LogLevel.Information;
                }
                if (this.IsErrorLogLevel(logLevel))
                {
                    this._AmountOfErrors += 1;
                }
                if (this.IsWarningLogLevel(logLevel))
                {
                    this._AmountOfWarnings += 1;
                }
                if (this.Configuration.ConvertTimeForLogentriesToUTCFormat)
                {
                    momentOfLogEntry = momentOfLogEntry.ToUniversalTime();
                }
                message = message.Trim();
                if (!string.IsNullOrEmpty(this.Configuration.Name))
                {
                    message = $"[{this.Configuration.Name.Trim()}] {message}";
                }

                this.FormatMessage(message, momentOfLogEntry, logLevel, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor color);
                message = formattedMessage;

                bool writeToLogFileIfLogFileIsAvailable = this.Configuration.WriteToLogFileIfLogFileIsAvailable;
                bool logLevelMatches = this.Configuration.LogLevelsForLogFile.Contains(logLevel);
                bool logFileExists = File.Exists(this.Configuration.LogFile);
                bool createLogFile = this.Configuration.CreateLogFileIfRequiredAndIfPossible;
                bool writeToLogFile = writeToLogFileIfLogFileIsAvailable && logLevelMatches && (logFileExists || createLogFile);
                if (writeToLogFile)
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
                    string messageForFile = message;
                    if (!Utilities.FileIsEmpty(this.Configuration.LogFile))
                    {
                        messageForFile = Environment.NewLine + messageForFile;
                    }
                    File.AppendAllText(this.Configuration.LogFile, messageForFile, Encoding.GetEncoding(this.Configuration.EncodingForLogfile));
                }
                if (this.Configuration.PrintOutputInConsole && this.Configuration.LogLevelsForConsoleOutput.Contains(logLevel))
                {
                    Console.Write(message.Substring(0, colorBegin));
                    this.WriteWithColorToConsole(message.Substring(colorBegin, colorEnd - colorBegin), logLevel);
                    Console.Write(message.Substring(colorEnd) + Environment.NewLine);
                }
                NewLogItem?.Invoke(originalMessage, message, logLevel);
            }
        }
        private void FormatMessage(string message, DateTime momentOfLogEntry, LogLevel loglevel, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor color)
        {
            color = this.Configuration.LoggedMessageTypesConfiguration[loglevel].ConsoleColor;
            switch (this.Configuration.Format)
            {
                case GRYLogLogFormat.OnlyMessage:
                    formattedMessage = message;
                    colorBegin = 0;
                    colorEnd = 0;
                    break;
                case GRYLogLogFormat.GRYLogFormat:
                    string part1 = "[" + momentOfLogEntry.ToString(this.Configuration.DateFormat) + "] [";
                    string part2 = this.Configuration.LoggedMessageTypesConfiguration[loglevel].CustomText;
                    string part3 = "] " + message;
                    formattedMessage = part1 + part2 + part3;
                    colorBegin = part1.Length;
                    colorEnd = part1.Length + part2.Length;
                    break;
                case GRYLogLogFormat.DateOnly:
                    formattedMessage = momentOfLogEntry.ToString(this.Configuration.DateFormat) + " " + message;
                    colorBegin = 0;
                    colorEnd = 0;
                    break;
                default:
                    throw new KeyNotFoundException($"Formatting {nameof(GRYLogLogFormat)} '{loglevel.ToString()}' is not implemented yet.");
            }
        }

        private bool IsWarningLogLevel(LogLevel logLevel)
        {
            return logLevel.Equals(LogLevel.Warning);
        }

        private bool IsErrorLogLevel(LogLevel logLevel)
        {
            return logLevel.Equals(LogLevel.Error) || logLevel.Equals(LogLevel.Critical);
        }

        private string GetExceptionMessage(string message, Exception exception)
        {
            string result = message;
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
                result = result + " (Exception-details: " + exception.StackTrace + ")";
            }
            return result;
        }
        private void WriteWithColorToConsole(string message, LogLevel logLevel)
        {
            try
            {
                Console.ForegroundColor = this.Configuration.LoggedMessageTypesConfiguration[logLevel].ConsoleColor;
                Console.Write(message);
            }
            finally
            {
                Console.ForegroundColor = this._ConsoleDefaultColor;
            }
        }

        public void ExecuteAndLog(Action action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            try
            {
                action();
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", LogLevel.Error, exception);
                if (!preventThrowingExceptions)
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished.", logLevelForOverhead);
            }
        }
        public void ExecuteAndLog<TParameter>(Action<TParameter> action, string nameOfAction, TParameter argument, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug)
        {
            string argumentAsString = argument.ToString();
            this.Log($"Action '{nameOfAction}({argumentAsString})' will be started now.", logLevelForOverhead);
            try
            {
                action(argument);
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}({argumentAsString})'.", LogLevel.Error, exception);
                if (!preventThrowingExceptions)
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}({argumentAsString})' finished.", logLevelForOverhead);
            }
        }
        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", LogLevel.Error, exception);
                if (preventThrowingExceptions)
                {
                    return defaultValue;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished.", logLevelForOverhead);
            }
        }
        public TResult ExecuteAndLog<TParameter, TResult>(Func<TParameter, TResult> action, string nameOfAction, TParameter argument, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default)
        {
            string argumentAsString = argument.ToString();
            this.Log($"Action '{nameOfAction}({argumentAsString})' will be started now.", logLevelForOverhead);
            try
            {
                return action(argument);
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}({argumentAsString})'.", LogLevel.Error, exception);
                if (preventThrowingExceptions)
                {
                    return defaultValue;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}({argumentAsString})' finished.", logLevelForOverhead);
            }
        }

        public void Dispose()
        {
            if (this._Watcher != null)
            {
                this._Watcher.Dispose();
            }
        }
        public IDisposable UseSubNamespace(string subnamespace)
        {
            return new GRYLogSubNamespaceProvider(this, subnamespace);
        }
        #region ILogger-Implementation
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.Log(() => $"{FormatEvent(eventId)} | { formatter(state, exception)}", logLevel);
        }

        private string FormatEvent(EventId eventId)
        {
            return $"EventId: {eventId.Id}, EventName: '{eventId.Name}'";
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (this.Configuration.LogLevelsForLogFile.Contains(logLevel) && this.Configuration.WriteToLogFileIfLogFileIsAvailable)
                || (this.Configuration.LogLevelsForConsoleOutput.Contains(logLevel) && this.Configuration.PrintOutputInConsole);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new GRYLogSubNamespaceProvider(this, state.ToString());
        }
        #endregion
    }
    public class GRYLogSubNamespaceProvider : IDisposable
    {
        private readonly GRYLog _LogObject;
        private readonly string _SubNamespace;
        public readonly string _OriginalNamespace;

        public GRYLogSubNamespaceProvider(GRYLog logObject, string subnamespace)
        {
            this._LogObject = logObject;
            this._SubNamespace = subnamespace;
            this._OriginalNamespace = this._LogObject.Configuration.Name;
            this._LogObject.Configuration.Name = $"{this._LogObject.Configuration.Name}.{this._SubNamespace}";
        }

        public void Dispose()
        {
            this._LogObject.Configuration.Name = this._OriginalNamespace;
        }
    }

    public class GRYLogConfiguration : IXmlSerializable
    {
        public GRYLogConfiguration()
        {
            this.Enabled = true;
            this.LogFile = string.Empty;
            this.ConvertTimeForLogentriesToUTCFormat = false;
            this.EncodingForLogfile = "utf-8";
            this.DateFormat = "yyyy-MM-dd HH:mm:ss";
            this.Format = GRYLogLogFormat.GRYLogFormat;
            this.PrintEmptyLines = false;
            this.WriteExceptionMessageOfExceptionInLogEntry = true;
            this.WriteExceptionStackTraceOfExceptionInLogEntry = true;
            this.StoreErrorsInErrorQueueInsteadOfLoggingThem = false;
            this.PrintOutputInConsole = true;
            this.LogItemIdLength = 8;
            this.Name = string.Empty;
            this.ReloadConfigurationWhenSourceFileWillBeChanged = true;
            this.CreateLogFileIfRequiredAndIfPossible = true;
            this.WriteToLogFileIfLogFileIsAvailable = true;
            this.PrintErrorsAsInformation = false;
            this.LogEveryLineOfLogEntryInNewLine = false;
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Trace, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Trace), ConsoleColor = ConsoleColor.Gray });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Debug, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Debug), ConsoleColor = ConsoleColor.Cyan });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Information, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Information), ConsoleColor = ConsoleColor.DarkGreen });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Warning, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Warning), ConsoleColor = ConsoleColor.DarkYellow });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Error, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Error), ConsoleColor = ConsoleColor.Red });
            this.LoggedMessageTypesConfiguration.Add(LogLevel.Critical, new LoggedMessageTypeConfiguration() { CustomText = nameof(LogLevel.Critical), ConsoleColor = ConsoleColor.DarkRed });
            this.LogLevelsForConsoleOutput.Add(LogLevel.Information);
            this.LogLevelsForLogFile.Add(LogLevel.Information);
            this.LogLevelsForConsoleOutput.Add(LogLevel.Warning);
            this.LogLevelsForLogFile.Add(LogLevel.Warning);
            this.LogLevelsForConsoleOutput.Add(LogLevel.Error);
            this.LogLevelsForLogFile.Add(LogLevel.Error);
            this.LogLevelsForConsoleOutput.Add(LogLevel.Critical);
            this.LogLevelsForLogFile.Add(LogLevel.Critical);
        }

        public string LogFile { get; set; }
        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenSourceFileWillBeChanged { get; set; }
        public bool Enabled { get; set; }
        public string EncodingForLogfile { get; set; }
        public int LogItemIdLength { get; set; }
        public bool PrintEmptyLines { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public string Name { get; set; }
        public bool StoreErrorsInErrorQueueInsteadOfLoggingThem { get; set; }
        public bool PrintOutputInConsole { get; set; }
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        public string DateFormat { get; set; }
        public IDictionary<LogLevel, LoggedMessageTypeConfiguration> LoggedMessageTypesConfiguration { get; set; } = new Dictionary<LogLevel, LoggedMessageTypeConfiguration>();
        public ISet<LogLevel> LogLevelsForConsoleOutput { get; set; } = new HashSet<LogLevel>();
        public ISet<LogLevel> LogLevelsForLogFile { get; set; } = new HashSet<LogLevel>();
        public GRYLogLogFormat Format { get; set; }
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; }
        public bool WriteToLogFileIfLogFileIsAvailable { get; set; }
        public bool CreateLogFileIfRequiredAndIfPossible { get; set; }
        public bool LogEveryLineOfLogEntryInNewLine { get; set; }

        public static GRYLogConfiguration LoadConfiguration(string configurationFile)
        {
            return Utilities.LoadFromDisk<GRYLogConfiguration>(configurationFile).Object;
        }
        public static void SaveConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            Utilities.PersistToDisk(configuration, configurationFile);
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
    public class LoggedMessageTypeConfiguration : IXmlSerializable
    {
        public ConsoleColor ConsoleColor { get; set; }
        public string CustomText { get; set; }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}