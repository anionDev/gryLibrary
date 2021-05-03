using GRYLibrary.Core.LogObject.ConcreteLogTargets;
using GRYLibrary.Core.Miscellaneous;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GRYLibrary.Core.LogObject
{
    public sealed class GRYLog : IDisposable, ILogger
    {
        public GRYLogConfiguration Configuration { get; set; }
        private readonly static object _LockObject = new();
        private readonly bool _Initialized = false;
        private int _AmountOfErrors = 0;
        private int _AmountOfWarnings = 0;
        internal readonly ConsoleColor _ConsoleDefaultColor;
        public event NewLogItemEventHandler NewLogItem;
        public delegate void NewLogItemEventHandler(LogItem logItem);
        public event ErrorOccurredEventHandler ErrorOccurred;
        public delegate void ErrorOccurredEventHandler(Exception exception, LogItem logItem);
        private FileSystemWatcher _Watcher;
        private bool AnyLogTargetEnabled
        {
            get
            {
                foreach (GRYLogTarget target in this.Configuration.LogTargets)
                {
                    if (target.Enabled)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private GRYLog()
        {
            this.Configuration = new GRYLogConfiguration();
        }
        private GRYLog(GRYLogConfiguration configuration)
        {
            lock (_LockObject)
            {
                this._ConsoleDefaultColor = System.Console.ForegroundColor;
                this.Configuration = configuration;
                this.Configuration.GetLogTarget<LogFile>().Enabled = !string.IsNullOrWhiteSpace(this.Configuration.GetLogFile());
                if (this.Configuration.ReloadConfigurationWhenConfigurationFileWillBeChanged && File.Exists(this.Configuration.ConfigurationFile))
                {
                    this.StartFileWatcherForConfigurationFile(this.Configuration.ConfigurationFile);
                }
                this._Initialized = true;
            }
        }
        public static GRYLog Create()
        {
            return Create(null);
        }
        public static GRYLog Create(string logFile)
        {
            GRYLogConfiguration configuration = new();
            configuration.ResetToDefaultValues(logFile);
            return new GRYLog(configuration);
        }
        public static GRYLog CreateByConfigurationFile(string configurationFile)
        {
            return new GRYLog(GRYLogConfiguration.LoadConfiguration(configurationFile));
        }
        public static GRYLog CreateByConfiguration(GRYLogConfiguration configuration)
        {
            return new GRYLog(configuration);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Configuration);
        }

        public override bool Equals(object obj)
        {
            if (obj is not GRYLog typedObject)
            {
                return false;
            }
            if (!this.Configuration.Equals(typedObject.Configuration))
            {
                return false;
            }
            return true;
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
                    System.Threading.Thread.Sleep(200);
                    this.Configuration = GRYLogConfiguration.LoadConfiguration(configurationFile);
                }
                catch (Exception exception)
                {
                    this.Log($"Could not reload Configuration of {nameof(GRYLog)} stored in {configurationFile}", LogLevel.Error, exception, 0x78200001.ToString());
                }
                finally
                {
                    this._Watcher.EnableRaisingEvents = true;
                }
            });
            this._Watcher.EnableRaisingEvents = true;
        }
        /// <remarks>
        /// Will only be used if <see cref="GRYLogConfiguration.StoreProcessedLogItemsInternally"/> is set to true.ls -la
        /// </remarks>
        public IList<LogItem> ProcessedLogItems { get; set; } = new List<LogItem>();
        public int GetAmountOfErrors()
        {
            return this._AmountOfErrors;
        }
        public int GetAmountOfWarnings()
        {
            return this._AmountOfWarnings;
        }
        public void LogGeneralProgramInformation()
        {
            ProcessModule module = Process.GetProcessById(Environment.ProcessId).MainModule;
            this.Log($"Executing assembly-name: {AppDomain.CurrentDomain.FriendlyName} ({module.FileName})", LogLevel.Information);
            this.Log($"Executing file-version: {module.FileVersionInfo.FileVersion}", LogLevel.Information);
            this.Log($"Current working-directory: {Directory.GetCurrentDirectory()}", LogLevel.Information);
        }
        public void LogCurrentSystemStatistics()
        {
            this.Log($"OS description: {RuntimeInformation.OSDescription}", LogLevel.Information);
            string appDrive = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
            string cDrive = "C:\\";
            this.LogDriveStatistics(cDrive);
            if (!appDrive.Equals(cDrive))
            {
                this.LogDriveStatistics(appDrive);
            }
        }

        private void LogDriveStatistics(string drive)
        {
            this.Log($"Total free bytes on drive '{drive}': {Utilities.GetTotalFreeSpace(drive)}", LogLevel.Information);
            //todo print ram usage
            //todo print cpu usage
            //todo print if internetconnection exists
        }

        public void LogSummary()
        {
            this.Log($"Amount of occurred Errors and Criticals: { this.GetAmountOfErrors()}", LogLevel.Information);
            this.Log($"Amount of occurred Warnings: { this.GetAmountOfWarnings()}", LogLevel.Information);
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

        public void Log(string message, string messagId = null)
        {
            this.Log(message, LogLevel.Information, messagId);
        }
        public void Log(string message, Exception exception, string messageId = null)
        {
            this.Log(message, LogLevel.Error, exception, messageId);
        }
        public void Log(string message, LogLevel logLevel, Exception exception, string messageId)
        {
            this.Log(this.GetExceptionMessage(exception, message), logLevel, messageId);
        }
        public void Log(string message, LogLevel logLevel, string messageId = null)
        {
            this.Log(() => message, logLevel, messageId);
        }

        public void Log(Func<string> getMessage, string messageId = null)
        {
            this.Log(getMessage, LogLevel.Information, messageId);
        }
        public void Log(Func<string> getMessage, Exception exception, string messageId = null)
        {
            this.Log(getMessage, LogLevel.Error, exception, messageId);
        }
        public void Log(Exception exception, string messageId = null)
        {
            this.Log(LogLevel.Error, exception, messageId);
        }
        public void Log(LogLevel logLevel, Exception exception, string messageId = null)
        {
            this.Log(() => "An exception occurred", logLevel, exception, messageId);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel, Exception exception, string messageId = null)
        {
            this.Log(() => this.GetExceptionMessage(exception, getMessage()), logLevel, messageId);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel, string messageId = null)
        {
            this.Log(new LogItem(getMessage, logLevel, messageId));
        }
        public void Log(LogItem logitem)
        {
            if (this.Configuration.WriteLogEntriesAsynchronous)
            {
                new Task(() => this.LogImplementation(logitem)).Start();
            }
            else
            {
                this.LogImplementation(logitem);
            }
        }
        private void LogImplementation(LogItem logItem)
        {
            try
            {
                if (LogLevel.None == logItem.LogLevel)
                {
                    return;
                }
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
                    if (!this.IsEnabled(logItem.LogLevel))
                    {
                        return;
                    }
                    if (!this.LineShouldBePrinted(logItem.PlainMessage))
                    {
                        return;
                    }
                    if (this.Configuration.StoreProcessedLogItemsInternally)
                    {
                        this.ProcessedLogItems.Add(logItem);
                    }
                    if (logItem.PlainMessage.Contains(Environment.NewLine) && this.Configuration.LogEveryLineOfLogEntryInNewLine)
                    {
                        foreach (string line in logItem.PlainMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            this.Log(new LogItem(line, logItem.LogLevel, logItem.Exception));
                        }
                        return;
                    }
                    if (this.Configuration.PrintErrorsAsInformation && logItem.LogLevel.Equals(LogLevel.Error))
                    {
                        logItem.LogLevel = LogLevel.Information;
                    }
                    if (this.IsErrorLogLevel(logItem.LogLevel))
                    {
                        this._AmountOfErrors += 1;
                    }
                    if (this.IsWarningLogLevel(logItem.LogLevel))
                    {
                        this._AmountOfWarnings += 1;
                    }
                    foreach (GRYLogTarget logTarget in this.Configuration.LogTargets)
                    {
                        if (logTarget.Enabled)
                        {
                            if (logTarget.LogLevels.Contains(logItem.LogLevel))
                            {
                                logTarget.Execute(logItem, this);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorOccurred?.Invoke(exception, logItem);
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

        public const string _Indentation = "    ";
        private string GetExceptionMessage(Exception exception, string message = null, uint indentationLevel = 1, string exceptionTitle = "Exception-information")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "An exception occurred.";
            }
            if (!(message.EndsWith(".") | message.EndsWith("?") | message.EndsWith(":") | message.EndsWith("!")))
            {
                message += ".";
            }
            string result = $"{exceptionTitle}: ";
            if (exception == null)
            {
                result += "null";
            }
            else
            {
                result += $"'{message}', Exception-type: {exception.GetType().FullName}, Exception-message: '{exception.Message}'";
                if (this.Configuration.WriteDetailsOfLoggedExceptionsToLogEntry)
                {
                    result += @$"
(Exception-details:
{this.Indent(this.FormatStackTrace(exception), indentationLevel)},
{this.Indent(this.FormatStackInnerException(exception, indentationLevel), indentationLevel)}
)";
                }
            }
            return result;
        }

        private string Indent(IList<string> lines, uint indentationLevel)
        {
            string fullIndentation = string.Concat(Enumerable.Repeat(_Indentation, (int)indentationLevel));
            return string.Join(Environment.NewLine, lines.Select(line => fullIndentation + line));
        }

        private IList<string> FormatStackTrace(Exception exception)
        {
            List<string> result = new()
            {
                "Stack-trace: "
            };
            result.AddRange(Utilities.SplitOnNewLineCharacter(exception.StackTrace));
            return result;
        }

        private IList<string> FormatStackInnerException(Exception exception, uint indentationLevel)
        {
            return Utilities.SplitOnNewLineCharacter(this.GetExceptionMessage(exception.InnerException, null, indentationLevel + 1, "Inner exception")).ToList();
        }

        public void ExecuteAndLogForEach<T>(IEnumerable<T> items, Action<T> itemAction, string nameOfEntireLoopAction, string subNamespaceOfEntireLoopAction, Func<T, string> nameOfSingleItemFunc, Func<T, string> subNamespaceOfSingleItemFunc, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug)
        {
            this.ExecuteAndLog(() =>
            {
                List<T> itemsAsList = items.ToList();
                uint amountOfItems = (uint)itemsAsList.Count;
                for (uint currentIndex = 0; currentIndex < itemsAsList.Count; currentIndex++)
                {
                    try
                    {
                        T currentItem = itemsAsList[(int)currentIndex];
                        string nameOfSingleItem = nameOfSingleItemFunc(currentItem);
                        this.ExecuteAndLog(() => itemAction(currentItem), nameOfSingleItem, preventThrowingExceptions, logLevelForOverhead, subNamespaceOfSingleItemFunc(currentItem));
                    }
                    finally
                    {
                        this.LogProgress(currentIndex + 1, amountOfItems);
                    }

                }
            }, nameOfEntireLoopAction, preventThrowingExceptions, logLevelForOverhead, subNamespaceOfEntireLoopAction);
        }

        public void LogProgress(uint enumerator, uint denominator)
        {
            if (enumerator < 0 || denominator < 0 || denominator < enumerator)
            {
                throw new ArgumentException($"Can not log progress for {nameof(enumerator)}={enumerator} and {nameof(denominator)}={denominator}. Both values must be nonnegative and {nameof(denominator)} must be greater than {nameof(enumerator)}.");
            }
            else
            {
                string percentValue = Math.Round(enumerator / (double)denominator * 100, 2).ToString();
                int denominatorLength = (int)Math.Floor(Math.Log10(denominator) + 1);
                string message = $"Processed { enumerator.ToString().PadLeft(denominatorLength, '0')}/{denominator} items ({percentValue}%)";
                this.Log(message);
            }
        }

        public void ExecuteAndLog(Action action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, string subNamespaceForLog = Utilities.EmptyString)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            Stopwatch stopWatch = new();
            try
            {
                using (this.UseSubNamespace(subNamespaceForLog))
                {
                    stopWatch.Start();
                    action();
                    stopWatch.Stop();
                }
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", LogLevel.Error, exception, 0x78200002.ToString());
                if (!preventThrowingExceptions)
                {
                    throw;
                }
            }
            finally
            {
                this.Log($"Action '{nameOfAction}' finished. Duration: {Utilities.DurationToUserFriendlyString(stopWatch.Elapsed)}", logLevelForOverhead);
            }
        }
        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default, string subNamespaceForLog = Utilities.EmptyString)
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            Stopwatch stopWatch = new();
            try
            {
                using (this.UseSubNamespace(subNamespaceForLog))
                {
                    stopWatch.Start();
                    TResult result = action();
                    stopWatch.Stop();
                    return result;
                }
            }
            catch (Exception exception)
            {
                this.Log($"An exception occurred while executing action '{nameOfAction}'.", LogLevel.Error, exception, 0x78200003.ToString());
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
                this.Log($"Action '{nameOfAction}' finished. Duration: {Utilities.DurationToUserFriendlyString(stopWatch.Elapsed)}", logLevelForOverhead);
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

        internal void InvokeObserver(LogItem message)
        {
            try
            {
                NewLogItem?.Invoke(message);
            }
            catch
            {
                Utilities.NoOperation();
            }
        }

        private string FormatEvent(EventId eventId)
        {
            return $"EventId: {eventId.Id}, EventName: '{eventId.Name}'";
        }
        #region ILogger-Implementation
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.AnyLogTargetEnabled;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new GRYLogSubNamespaceProvider(this, state.ToString());
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.Log(() => $"{this.FormatEvent(eventId)} | { formatter(state, exception)}", logLevel, 0x78200004.ToString());
        }


        #endregion

    }

}