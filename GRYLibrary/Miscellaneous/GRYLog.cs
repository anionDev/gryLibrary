using System;
using System.Collections.Generic;
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
    public enum GRYLogLogFormat : int
    {
        OnlyMessage = 0,
        GRYLogFormat = 1,
        DateOnly = 2
    }
    public class GRYLog : IDisposable
    {
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

        public void Log(Func<string> message)
        {
            this.Log(message, GRYLogLogLevel.Information);
        }
        public void LogByFunction(Func<string> message, Exception exception)
        {
            this.Log(message, GRYLogLogLevel.Exception, exception);
        }
        public void Log(Func<string> message, GRYLogLogLevel logLevel, Exception exception)
        {
            this.Log(() => this.GetExceptionMessage(message(), exception), logLevel);
        }
        public void Log(Func<string> message, GRYLogLogLevel logLevel)
        {
            this.Log(message(), logLevel);
        }

        public void Log(string message)
        {
            this.Log(message, GRYLogLogLevel.Information);
        }
        public void Log(string message, Exception exception)
        {
            this.Log(message, GRYLogLogLevel.Exception, exception);
        }
        public void Log(Exception exception)
        {
            this.Log(GRYLogLogLevel.Exception, exception);
        }
        public void Log(GRYLogLogLevel logLevel, Exception exception)
        {
            this.Log("An exception occurred", logLevel, exception);
        }
        public void Log(string message, GRYLogLogLevel logLevel, Exception exception)
        {
            this.Log(this.GetExceptionMessage(message, exception), logLevel);
        }
        public void Log(string message, GRYLogLogLevel logLevel)
        {
            DateTime momentOfLogEntry = DateTime.Now;
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
            message = message.Trim();
            string originalMessage = message;
            lock (_LockObject)
            {
                if (message.Contains(Environment.NewLine) && this.Configuration.LogEveryLineOfLogEntryInNewLine)
                {
                    foreach (string line in message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        this.Log(line, logLevel);
                    }
                    return;
                }

                if (!this.ShouldBeLogged(logLevel))
                {
                    return;
                }
                if (this.Configuration.PrintErrorsAsInformation && this.IsErrorLogLevel(logLevel))
                {
                    logLevel = GRYLogLogLevel.Information;
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
                    message = "[" + this.Configuration.Name.Trim() + "] " + message;
                }

                this.FormatMessage(message, momentOfLogEntry, logLevel, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor color);
                message = formattedMessage;

                lock (_LockObject)
                {
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
                        File.AppendAllLines(this.Configuration.LogFile, new string[] { message }, this.Configuration.EncodingForLogfile);
                    }
                    if (this.Configuration.PrintOutputInConsole && this.Configuration.LoggedMessageTypesInConsole.Contains(logLevel))
                    {
                        Console.Write(message.Substring(0, colorBegin));
                        this.WriteWithColorToConsole(message.Substring(colorBegin, colorEnd - colorBegin), logLevel);
                        Console.Write(message.Substring(colorEnd) + Environment.NewLine);
                    }
                }
                NewLogItem?.Invoke(originalMessage, message, logLevel);
            }
        }
        private void FormatMessage(string message, DateTime momentOfLogEntry, GRYLogLogLevel loglevel, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor color)
        {
            color = this.GetColorByType(loglevel);
            if (this.Configuration.Format == GRYLogLogFormat.OnlyMessage)
            {
                formattedMessage = message;
                colorBegin = 0;
                colorEnd = 0;
                return;
            }
            if (this.Configuration.Format == GRYLogLogFormat.GRYLogFormat)
            {
                string part1 = "[" + momentOfLogEntry.ToString(this.Configuration.DateFormat) + "] [";
                string part2 = this.GetPrefixInStringFormat(loglevel);
                string part3 = "] " + message;
                formattedMessage = part1 + part2 + part3;
                colorBegin = part1.Length;
                colorEnd = part1.Length + part2.Length;
                return;
            }
            if (this.Configuration.Format == GRYLogLogFormat.DateOnly)
            {
                formattedMessage = momentOfLogEntry.ToString(this.Configuration.DateFormat) + " " + message;
                colorBegin = 0;
                colorEnd = 0;
                return;
            }
            throw new KeyNotFoundException();
        }

        private bool IsWarningLogLevel(GRYLogLogLevel logLevel)
        {
            return logLevel.Equals(GRYLogLogLevel.Warning);
        }

        private bool IsErrorLogLevel(GRYLogLogLevel logLevel)
        {
            return logLevel.Equals(GRYLogLogLevel.Exception) || logLevel.Equals(GRYLogLogLevel.Critical);
        }

        private bool ShouldBeLogged(GRYLogLogLevel logLevel)
        {
            return this.Configuration.LoggedMessageTypesInLogFile.Contains(logLevel) || this.Configuration.LoggedMessageTypesInConsole.Contains(logLevel);
        }

        private string GetLogItemId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, this.Configuration.LogItemIdLength);
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
            this.LogFile = string.Empty;
            this.ConvertTimeForLogentriesToUTCFormat = false;
            this.EncodingForLogfile = new UTF8Encoding(false);
            this.DateFormat = "yyyy-MM-dd HH:mm:ss";
            this.InformationPrefix = "Information";
            this.ErrorPrefix = "Error";
            this.DebugPrefix = "Debug";
            this.WarningPrefix = "Warning";
            this.VerbosePrefix = "Verbose";
            this.CriticalPrefix = "Critical";
            this.MaximalLengthOfPrefixes = 30;
            this.Format = GRYLogLogFormat.GRYLogFormat;
            this.PrintEmptyLines = false;
            this.WriteExceptionMessageOfExceptionInLogEntry = true;
            this.WriteExceptionStackTraceOfExceptionInLogEntry = true;
            this.StoreErrorsInErrorQueueInsteadOfLoggingThem = false;
            this.PrintOutputInConsole = true;
            this.ColorForDebug = ConsoleColor.Cyan;
            this.ColorForVerbose = ConsoleColor.DarkGreen;
            this.ColorForInfo = ConsoleColor.Green;
            this.ColorForWarning = ConsoleColor.DarkYellow;
            this.ColorForError = ConsoleColor.DarkRed;
            this.ColorForCritical = ConsoleColor.Red;
            this.LogItemIdLength = 8;
            this.Name = string.Empty;
            this.LoggedMessageTypesInConsole = new List<GRYLogLogLevel>();
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Information);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInConsole.Add(GRYLogLogLevel.Critical);
            this.LoggedMessageTypesInLogFile = new List<GRYLogLogLevel>();
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Information);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Warning);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Exception);
            this.LoggedMessageTypesInLogFile.Add(GRYLogLogLevel.Critical);
            this.ReloadConfigurationWhenSourceFileWillBeChanged = true;
            this.CreateLogFileIfRequiredAndIfPossible = true;
            this.WriteToLogFileIfLogFileIsAvailable = true;
            this.PrintErrorsAsInformation = false;
            this.LogEveryLineOfLogEntryInNewLine = false;
        }
        public string LogFile { get; set; }
        /// <summary>
        /// If this value is false then changing this value in the configuration-file has no effect.
        /// </summary>
        public bool ReloadConfigurationWhenSourceFileWillBeChanged { get; set; }
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
        public bool WriteExceptionMessageOfExceptionInLogEntry { get; set; }
        public bool WriteExceptionStackTraceOfExceptionInLogEntry { get; set; }
        public int MaximalLengthOfPrefixes { get; set; }
        public string DateFormat { get; set; }
        public IList<GRYLogLogLevel> LoggedMessageTypesInConsole { get; set; }
        public IList<GRYLogLogLevel> LoggedMessageTypesInLogFile { get; set; }
        public ConsoleColor ColorForInfo { get; set; }
        public ConsoleColor ColorForWarning { get; set; }
        public ConsoleColor ColorForError { get; set; }
        public ConsoleColor ColorForDebug { get; set; }
        public ConsoleColor ColorForVerbose { get; set; }
        public ConsoleColor ColorForCritical { get; set; }
        public GRYLogLogFormat Format { get; set; }
        public bool ConvertTimeForLogentriesToUTCFormat { get; set; }
        public bool WriteToLogFileIfLogFileIsAvailable { get; set; }
        public bool CreateLogFileIfRequiredAndIfPossible { get; set; }

        public static Encoding GRYLogConfigurationFileDefaultEncoding { get; set; } = new UTF8Encoding(false);
        public bool LogEveryLineOfLogEntryInNewLine { get; set; }

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
            new SimpleObjectPersistence<GRYLogConfiguration>(configurationFile, encoding) { Object = configuration }.SaveObject();
        }
        public static void SavedConfiguration(string configurationFile, GRYLogConfiguration configuration)
        {
            SavedConfiguration(configurationFile, configuration, GRYLogConfigurationFileDefaultEncoding);
        }
    }
}