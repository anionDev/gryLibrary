using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.LogObject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous
{
    public class ExternalProgramExecutor : IDisposable
    {
        public ExternalProgramExecutor(string programPathAndFile, string arguments) : this(programPathAndFile, arguments, null)
        {
        }
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string workingDirectory)
        {
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.WorkingDirectory = workingDirectory;
        }
        public ExecutionState CurrentExecutionState { get; private set; } = ExecutionState.NotStarted;
        public bool LogOverhead { get; set; } = false;
        public GRYLog LogObject { get; set; }
        public string Arguments { get; set; }
        public string ProgramPathAndFile { get; set; }
        public bool CreateWindow { get; set; } = true;
        public string Title { get; set; }
        public string LogNamespace { get; set; }
        public string WorkingDirectory { get; set; }
        public bool UpdateConsoleTitle { get; set; } = false;
        /// <remarks>
        /// This property will be ignored if <see cref="RunSynchronously"/>==false.
        /// </remarks>
        public bool ThrowErrorIfExitCodeIsNotZero { get; set; } = false;
        public int? TimeoutInMilliseconds { get; set; }
        internal string DefaultTitle { get; private set; }
        public bool PrintErrorsAsInformation { get; set; }
        public delegate void ExecutionFinishedHandler(ExternalProgramExecutor sender, int exitCode);
        public event ExecutionFinishedHandler ExecutionFinishedEvent;
        private bool _Running = false;
        private TimeSpan _ExecutionDuration = default;
        public TimeSpan ExecutionDuration
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._ExecutionDuration;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExecutionDuration), ExecutionState.Terminated, true));
                }
            }
            private set { this._ExecutionDuration = value; }
        }
        public bool Running()
        {
            return this._Running;
        }
        private readonly GRYLog _DefaultLog = GRYLog.Create();
        private readonly object _LockObject = new object();
        private readonly ConcurrentQueue<(LogLevel, string)> _NotLoggedOutputLines = new ConcurrentQueue<(LogLevel, string)>();
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// The exit-code of the executed program will be returned.
        /// </returns>
        /// <exception cref="UnexpectedExitCodeException">
        /// Will be thrown if <see cref="ThrowErrorIfExitCodeIsNotZero"/> and the exitcode of the executed program is not 0.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Will be thrown if <see cref="StartSynchronously"/> was already called.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Will be thrown if the given working-directory does not exist.
        /// </exception>
        /// <exception cref="ProcessStartException">
        /// Will be thrown if the process could not be started.
        /// </exception>
        public int StartSynchronously()
        {
            Prepare();
            string originalConsoleTitle = default;
            ConsoleColor originalConsoleForegroundColor = default;
            ConsoleColor originalConsoleBackgroundColor = default;

            try
            {
                originalConsoleForegroundColor = Console.ForegroundColor;
                originalConsoleBackgroundColor = Console.BackgroundColor;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    originalConsoleTitle = Console.Title;
                }
            }
            catch
            {
                Utilities.NoOperation();
            }
            try
            {
                if (UpdateConsoleTitle)
                {
                    try
                    {
                        Console.Title = this.Title;
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                }
                Task task = StartProgram();
                task.Wait();
                return this.ExitCode;
            }
            finally
            {
                try
                {
                    this._Running = false;
                    if (UpdateConsoleTitle)
                    {
                        Console.Title = originalConsoleTitle;
                    }
                    Console.ForegroundColor = originalConsoleForegroundColor;
                    Console.BackgroundColor = originalConsoleBackgroundColor;
                }
                catch
                {
                    Utilities.NoOperation();
                }
            }
        }
        private void Prepare()
        {
            this.CheckIfStartOperationWasAlreadyCalled();
            if (string.IsNullOrWhiteSpace(this.LogNamespace))
            {
                this.LogNamespace = string.Empty;
            }
            if (LogObject == default)
            {
                LogObject = _DefaultLog;
            }
            this.ResolvePaths();
            DefaultTitle = $"{WorkingDirectory}>{ProgramPathAndFile} {Arguments}";
            if (string.IsNullOrWhiteSpace(this.Title))
            {
                this.Title = DefaultTitle;
            }
        }

        public void StartAsynchronously()
        {
            Prepare();
            StartProgram();
        }
        private Process _Process;
        private IDisposable _SubNamespace;
        private Task StartProgram()
        {
            _SubNamespace = LogObject.UseSubNamespace(this.LogNamespace);
            _Process = new Process();
            Stopwatch stopWatch = new Stopwatch();
            try
            {
                this.ProcessWasAbortedDueToTimeout = false;
                if (!Directory.Exists(this.WorkingDirectory))
                {
                    throw new ArgumentException($"The specified working-directory '{this.WorkingDirectory}' does not exist.");
                }
                ProcessStartInfo StartInfo = new ProcessStartInfo(this.ProgramPathAndFile)
                {
                    UseShellExecute = false,
                    ErrorDialog = false,
                    Arguments = this.Arguments,
                    WorkingDirectory = this.WorkingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = !this.CreateWindow,
                };
                _Process.StartInfo = StartInfo;
                _Process.OutputDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
                {
                    this.EnqueueInformation(dataReceivedEventArgs.Data);
                };
                _Process.ErrorDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
                {
                    if (this.PrintErrorsAsInformation)
                    {
                        this.EnqueueInformation(dataReceivedEventArgs.Data);
                    }
                    else
                    {
                        this.EnqueueError(dataReceivedEventArgs.Data);
                    }
                };
                SupervisedThread readLogItemsThread;
                if (this.LogOverhead)
                {
                    this.LogObject.Log($"Start '{this.Title}'", LogLevel.Debug);
                }
                stopWatch.Start();
                _Process.Start();
                this.ProcessId = _Process.Id;
                _Process.BeginOutputReadLine();
                _Process.BeginErrorReadLine();
                this._Running = true;
                if (this.LogOutput)
                {
                    readLogItemsThread = SupervisedThread.Create(this.LogOutputImplementation);
                    readLogItemsThread.Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.Title}))";
                    readLogItemsThread.LogOverhead = this.LogOverhead;
                    readLogItemsThread.Start();
                }
            }
            catch (Exception exception)
            {
                Dispose();
                throw new ProcessStartException($"Exception occurred while start execution '{this.Title}'", exception);
            }
            Task task = new Task(() =>
            {
                try
                {
                    WaitForProcessEnd(_Process);
                    stopWatch.Stop();
                    this.ExecutionDuration = stopWatch.Elapsed;
                    this.ExitCode = _Process.ExitCode;
                    while (!_NotLoggedOutputLines.IsEmpty)
                    {
                        Thread.Sleep(60);
                    }
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    if (this.LogOverhead)
                    {
                        this.LogObject.Log($"Finished '{this.Title}'", LogLevel.Debug);
                        this.LogObject.Log(this.GetResult(), LogLevel.Debug);
                    }
                    try
                    {
                        ExecutionFinishedEvent?.Invoke(this, this.ExitCode);
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                    if (this.ThrowErrorIfExitCodeIsNotZero && this.ExitCode != 0)
                    {
                        throw new UnexpectedExitCodeException(this);
                    }
                }
                finally
                {
                    Dispose();
                }
            });
            task.Start();
            return task;
        }
        public void Dispose()
        {
            if (_SubNamespace != null)
            {
                _SubNamespace.Dispose();
            }
            if (_Process != null)
            {
                _Process.Dispose();
            }
            if (_DefaultLog != null)
            {
                _DefaultLog.Dispose();
            }
        }

        private void WaitForProcessEnd(Process process)
        {
            if (this.TimeoutInMilliseconds.HasValue)
            {
                if (!process.WaitForExit(this.TimeoutInMilliseconds.Value))
                {
                    process.Kill();
                    process.WaitForExit();
                    this.ProcessWasAbortedDueToTimeout = true;
                }
            }
            else
            {
                process.WaitForExit();
            }
            this.CurrentExecutionState = ExecutionState.Terminated;
        }
        public void WaitUntilTerminated()
        {
            Utilities.WaitUntilConditionIsTrue(() => this.CurrentExecutionState == ExecutionState.Terminated);
        }
        private void CheckIfStartOperationWasAlreadyCalled()
        {
            lock (this._LockObject)
            {
                if (this.CurrentExecutionState != ExecutionState.NotStarted)
                {
                    throw new InvalidOperationException("The process was already started.");
                }
                this.CurrentExecutionState = ExecutionState.Running;
            }
        }

        private void ResolvePaths()
        {
            string newProgram = this.ProgramPathAndFile;
            string newArgument = this.Arguments;
            Utilities.ResolvePathOfProgram(ref newProgram, ref newArgument);
            this.ProgramPathAndFile = newProgram;
            if (string.IsNullOrWhiteSpace(this.WorkingDirectory))
            {
                this.WorkingDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                this.WorkingDirectory = Utilities.ResolveToFullPath(this.WorkingDirectory);
            }
        }
        private readonly IList<string> _AllStdErrLines = new List<string>();
        private string[] _AllStdErrLinesAsArray;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string[] AllStdErrLines
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdErrLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdErrLines), ExecutionState.Terminated, true));
                }
            }
        }

        private string GetInvalidOperationDueToNotTerminatedMessageByMembername(string name, ExecutionState state, bool requiredIn)
        {
            string requiredInAsString = requiredIn ? "" : " not";
            return $"'{name}' is not avilable because the current {nameof(ExecutionState)}-value state is {Enum.GetName(typeof(ExecutionState), this.CurrentExecutionState)} but it must{requiredInAsString} be in the state {Enum.GetName(typeof(ExecutionState), state)} to be able to query it.";
        }

        private bool _processWasAbortedDueToTimeout;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public bool ProcessWasAbortedDueToTimeout
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._processWasAbortedDueToTimeout;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ProcessWasAbortedDueToTimeout), ExecutionState.Terminated, true));
                }
            }
            private set
            {
                this._processWasAbortedDueToTimeout = value;
            }
        }
        private int _ExitCode;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public int ExitCode
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._ExitCode;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExitCode), ExecutionState.Terminated, true));
                }
            }
            private set
            {
                this._ExitCode = value;
            }
        }
        private int _ProcessId;
        public int ProcessId
        {
            get
            {
                if (this.CurrentExecutionState != ExecutionState.NotStarted)
                {
                    return this._ProcessId;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ProcessId), ExecutionState.NotStarted, false));
                }
            }
            private set
            {
                this._ProcessId = value;
            }
        }

        private void EnqueueError(string data)
        {
            if (data != null)
            {
                this._AllStdErrLines.Add(data);
                if (this.LogOutput)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Error, data));
                }
            }
        }

        private readonly IList<string> _AllStdOutLines = new List<string>();
        public bool LogOutput { get; set; } = true;
        private string[] _AllStdOutLinesAsArray;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string[] AllStdOutLines
        {
            get
            {
                if (this.CurrentExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdOutLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdOutLines), ExecutionState.Terminated, true));
                }
            }
        }


        private void EnqueueInformation(string data)
        {
            if (data != null)
            {
                this._AllStdOutLines.Add(data);
                if (this.LogOutput)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Information, data));
                }
            }
        }
        private void LogOutputImplementation()
        {
            while (this.Running() || !this._NotLoggedOutputLines.IsEmpty)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out (LogLevel, string) logItem))
                {
                    this.LogObject.Log(logItem.Item2, logItem.Item1);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        /// <returns>
        /// Returns a summary of the executed program with its error-code, console-outputs, etc.
        /// </returns>
        /// <remarks>
        /// This summary is designed for readability and not for a further program-controlled processing of the data. For that purpose please read out the properties of this object.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public string GetResult()
        {
            if (this.CurrentExecutionState == ExecutionState.Terminated)
            {
                string result = $"{nameof(ExternalProgramExecutor)}-summary:";
                result = result + Environment.NewLine + $"Executed program: {DefaultTitle}";
                result = result + Environment.NewLine + $"Process-Id: {this.ProcessId}";
                result = result + Environment.NewLine + $"Title: {this.Title}";
                result = result + Environment.NewLine + $"Exit-code: {this.ExitCode}";
                result = result + Environment.NewLine + $"Execution-duration: {this.ExecutionDuration:d'd 'h'h 'm'm 's's'}";
                result = result + Environment.NewLine + $"StdOut:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdOutLines);
                result = result + Environment.NewLine + $"StdErr:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdErrLines);
                return result;
            }
            else
            {
                throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.GetResult), ExecutionState.Terminated, true));
            }
        }
    }
    public enum ExecutionState
    {
        NotStarted = 0,
        Running = 1,
        Terminated = 2
    }
}