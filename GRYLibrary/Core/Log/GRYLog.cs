using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Log
{
    public class GRYLog : IDisposable, ILogger, IXmlSerializable
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
                foreach (GRYLogTarget target in this.Configuration._LogTargets)
                {
                    if (target.Enabled)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public GRYLog() { }
        private GRYLog(GRYLogConfiguration configuration)
        {
            lock (_LockObject)
            {
                this._ConsoleDefaultColor = System.Console.ForegroundColor;
                this.Configuration = configuration;
                this.Configuration.GetLogTarget<LogFile>().Enabled = !string.IsNullOrWhiteSpace(this.Configuration.LogFile);
                if (this.Configuration.ReloadConfigurationWhenConfigurationFileWillBeChanged && File.Exists(this.Configuration.ConfigurationFile))
                {
                    this.StartFileWatcherForConfigurationFile(this.Configuration.ConfigurationFile);
                }
                this._Initialized = true;
            }
        }
        public static GRYLog Create()
        {
            return new GRYLog(new GRYLogConfiguration());
        }
        public static GRYLog Create(string logFile)
        {
            GRYLogConfiguration configuration = new GRYLogConfiguration
            {
                LogFile = logFile
            };
            return new GRYLog(configuration);
        }
        public static GRYLog CreateByConfigurationFile(string configurationFile)
        {
            return new GRYLog(GRYLogConfiguration.LoadConfiguration(configurationFile));
        }
        public override int GetHashCode()
        {
            return this.Configuration.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            GRYLog typedObject = obj as GRYLog;
            if (typedObject == null)
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
        public void LogGeneralSystemInformation()
        {
            this.Log($"Executing assembly name: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}", LogLevel.Information);
            this.Log($"Executing assembly version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}", LogLevel.Information);
            this.Log($"OS description: {RuntimeInformation.OSDescription}", LogLevel.Information);
        }
        public void LogCurrentSystemStatistics()
        {
            string appDrive = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
            string cDrive = "C:";
            this.LogDriveStatistics(cDrive);
            if (!appDrive.Equals(cDrive))
            {
                this.LogDriveStatistics(appDrive);
            }
            this.Log($"Current working-directory: {Directory.GetCurrentDirectory()}", LogLevel.Information);
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
            this.Log($"Amount of occurred Errors and Criticals: { this.GetAmountOfErrors().ToString()}", LogLevel.Information);
            this.Log($"Amount of occurred Warnings: { this.GetAmountOfWarnings().ToString()}", LogLevel.Information);
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
            this.Log(new LogItem(getMessage, logLevel));
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
                    if (!this.AnyLogTargetEnabled)
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
                    foreach (GRYLogTarget logTarget in this.Configuration._LogTargets)
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
            try
            {
                using (this.UseSubNamespace(subNamespaceForLog))
                {
                    action();
                }
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
        public TResult ExecuteAndLog<TResult>(Func<TResult> action, string nameOfAction, bool preventThrowingExceptions = false, LogLevel logLevelForOverhead = LogLevel.Debug, TResult defaultValue = default, string subNamespaceForLog = "")
        {
            this.Log($"Action '{nameOfAction}' will be started now.", logLevelForOverhead);
            try
            {
                using (this.UseSubNamespace(subNamespaceForLog))
                {
                    return action();
                }
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
            this.Log(() => $"{this.FormatEvent(eventId)} | { formatter(state, exception)}", logLevel);
        }


        #endregion
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == this.GetType().ToString())
            {
                this.Configuration = new GRYLogConfiguration();
                this.Configuration.ReadXml(reader);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(this.GetType().ToString());
            this.Configuration.WriteXml(writer);
            writer.WriteEndElement();
        }
    }

}