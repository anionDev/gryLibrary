﻿using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Log
{
    public class GRYLog : IDisposable, ILogger
    {
        public GRYLogConfiguration Configuration { get; set; }
        private readonly static object _LockObject = new object();
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
        public GRYLog()
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
            GRYLogConfiguration configuration = new GRYLogConfiguration();
            configuration.ResetToDefaultValues(logFile);
            return new GRYLog(configuration);
        }
        public static GRYLog CreateByConfigurationFile(string configurationFile)
        {
            return new GRYLog(GRYLogConfiguration.LoadConfiguration(configurationFile));
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
            this.Log(this.GetExceptionMessage(message, exception), logLevel, messageId);
        }
        public void Log(string message, LogLevel logLevel, string messageId = null)
        {
            this.Log(() => message, logLevel, messageId);
        }

        public void Log(Func<string> getMessage, string messageId)
        {
            this.Log(getMessage, LogLevel.Information, messageId);
        }
        public void Log(Func<string> getMessage, Exception exception, string messageId)
        {
            this.Log(getMessage, LogLevel.Error, exception, messageId);
        }
        public void Log(Exception exception, string messageId)
        {
            this.Log(LogLevel.Error, exception, messageId);
        }
        public void Log(LogLevel logLevel, Exception exception, string messageId)
        {
            this.Log(() => "An exception occurred", logLevel, exception, messageId);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel, Exception exception, string messageId)
        {
            this.Log(() => this.GetExceptionMessage(getMessage(), exception), logLevel, messageId);
        }
        public void Log(Func<string> getMessage, LogLevel logLevel, string messageId)
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
        private void LogImplementation(LogItem logitem)
        {
            try
            {
                if (LogLevel.None == logitem.LogLevel)
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
                    if (!this.IsEnabled(logitem.LogLevel))
                    {
                        return;
                    }
                    if (!this.LineShouldBePrinted(logitem.PlainMessage))
                    {
                        return;
                    }
                    if (logitem.PlainMessage.Contains(Environment.NewLine) && this.Configuration.LogEveryLineOfLogEntryInNewLine)
                    {
                        foreach (string line in logitem.PlainMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            this.Log(new LogItem(line, logitem.LogLevel, logitem.Exception));
                        }
                        return;
                    }
                    if (this.Configuration.PrintErrorsAsInformation && logitem.LogLevel.Equals(LogLevel.Error))
                    {
                        logitem.LogLevel = LogLevel.Information;
                    }
                    if (this.IsErrorLogLevel(logitem.LogLevel))
                    {
                        this._AmountOfErrors += 1;
                    }
                    if (this.IsWarningLogLevel(logitem.LogLevel))
                    {
                        this._AmountOfWarnings += 1;
                    }
                    foreach (GRYLogTarget logTarget in this.Configuration.LogTargets)
                    {
                        if (logTarget.Enabled)
                        {
                            if (logTarget.LogLevels.Contains(logitem.LogLevel))
                            {
                                logTarget.Execute(logitem, this);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.ErrorOccurred?.Invoke(exception, logitem);
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

        public void ExecuteAndLog(Action action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, string subNamespaceForLog = "")
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            Stopwatch stopWatch = new Stopwatch();
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
        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default, string subNamespaceForLog = "")
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            Stopwatch stopWatch = new Stopwatch();
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
                this.NewLogItem?.Invoke(message);
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