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
    public sealed class ExternalProgramExecutor : IDisposable
    {
        public ExternalProgramExecutor(string programPathAndFile) : this(programPathAndFile, Utilities.EmptyString, null)
        {
        }
        public ExternalProgramExecutor(string programPathAndFile, string arguments) : this(programPathAndFile, arguments, null)
        {
        }
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string workingDirectory)
        {
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.WorkingDirectory = workingDirectory;
        }
        public Verbosity Verbosity { get; set; } = Verbosity.Normal;
        public ExecutionState CurrentExecutionState { get; private set; } = ExecutionState.NotStarted;
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
        internal string CMD { get; private set; }
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
        public bool IsRunning
        {
            get
            {
                return this._Running;
            }
        }
        private static readonly GRYLog _DefaultLog = GRYLog.Create();
        private readonly object _LockObject = new();
        private readonly ConcurrentQueue<(LogLevel, string)> _NotLoggedOutputLines = new();
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
            this.Prepare();
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
                if (this.UpdateConsoleTitle)
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
                Task task = this.StartProgram();
                task.Wait();
                return this.ExitCode;
            }
            finally
            {
                try
                {
                    if (this.UpdateConsoleTitle)
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
            if (this.LogObject == default)
            {
                this.LogObject = _DefaultLog;
                if (this.Verbosity == Verbosity.Verbose)
                {
                    foreach (var logtarget in this.LogObject.Configuration.LogTargets)
                    {
                        logtarget.LogLevels.Add(LogLevel.Debug);
                    }
                }
            }
            this.ResolvePaths();
            this.CMD = $"{this.WorkingDirectory}>{this.ProgramPathAndFile} {this.Arguments}";
            if (this.Title == null)
            {
                this.Title = string.Empty;
            }
            this.LogStart();
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
        public string GetSummaryOfExecutedProgram(bool includeStdOutAndStdErr = false)
        {
            if (this.CurrentExecutionState == ExecutionState.Terminated)
            {
                string result = $"{nameof(ExternalProgramExecutor)}-summary:";
                result = result + Environment.NewLine + $"Title: {this.Title}";
                result = result + Environment.NewLine + $"Executed program: {this.CMD}";
                result = result + Environment.NewLine + $"Process-Id: {this.ProcessId}";
                result = result + Environment.NewLine + $"Exit-code: {this.ExitCode}";
                result = result + Environment.NewLine + $"Execution-duration: {this.ExecutionDuration:d'd 'h'h 'm'm 's's'}";
                if (includeStdOutAndStdErr)
                {
                    result = result + Environment.NewLine + $"StdOut:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdOutLines);
                    result = result + Environment.NewLine + $"StdErr:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdErrLines);
                }
                return result;
            }
            else
            {
                throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.GetSummaryOfExecutedProgram), ExecutionState.Terminated, true));
            }
        }

        private void LogStart()
        {
            if (string.IsNullOrWhiteSpace(this.Title))
            {
                this.LogObject.Log($"Start executing program", LogLevel.Debug);
            }
            else
            {
                this.LogObject.Log($"Start executing '{this.Title}'", LogLevel.Debug);
            }
            this.LogObject.Log($"Program which will be executed: {this.CMD}", LogLevel.Debug);
        }
        private void LogImmediatelyAfterStart(int processId)
        {
            this.LogObject.Log($"Process-Id of started program: " + processId, LogLevel.Debug);
        }
        private void LogException(Exception exception)
        {
            this.LogObject.Log(exception);
        }
        private void LogEnd()
        {
            this.LogObject.Log($"Finished executing program", LogLevel.Debug);
            foreach (string line in Utilities.SplitOnNewLineCharacter(this.GetSummaryOfExecutedProgram()))
            {
                this.LogObject.Log(line, LogLevel.Debug);
            }
        }

        public void StartAsynchronously()
        {
            this.Prepare();
            this.StartProgram();
        }
        private Process _Process;
        private IDisposable _SubNamespace;
        private Task StartProgram()
        {
            this._SubNamespace = this.LogObject.UseSubNamespace(this.LogNamespace);
            this._Process = new Process();
            Stopwatch stopWatch = new();
            try
            {
                this.ProcessWasAbortedDueToTimeout = false;
                if (!Directory.Exists(this.WorkingDirectory))
                {
                    throw new ArgumentException($"The specified working-directory '{this.WorkingDirectory}' does not exist.");
                }
                ProcessStartInfo StartInfo = new(this.ProgramPathAndFile)
                {
                    UseShellExecute = false,
                    ErrorDialog = false,
                    Arguments = Arguments,
                    WorkingDirectory = WorkingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = !this.CreateWindow,
                };
                this._Process.StartInfo = StartInfo;
                this._Process.OutputDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
                {
                    this.EnqueueInformation(dataReceivedEventArgs.Data);
                };
                this._Process.ErrorDataReceived += (object sender, DataReceivedEventArgs dataReceivedEventArgs) =>
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
                stopWatch.Start();
                this._Process.Start();
                this.ProcessId = this._Process.Id;
                this.LogImmediatelyAfterStart(this._ProcessId);
                this._Process.BeginOutputReadLine();
                this._Process.BeginErrorReadLine();
                this._Running = true;
                readLogItemsThread = SupervisedThread.Create(this.LogOutputImplementation);
                readLogItemsThread.Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.Title}))";
                readLogItemsThread.LogOverhead = false;
                readLogItemsThread.Start();
            }
            catch (Exception exception)
            {
                this.Dispose();
                Exception processStartException = new ProcessStartException($"Exception occurred while start execution '{this.Title}'", exception);
                this.LogException(processStartException);
                throw processStartException;
            }
            Task task = new(() =>
            {
                try
                {
                    this.WaitForProcessEnd(this._Process, stopWatch);
                    this.ExecutionDuration = stopWatch.Elapsed;
                    this.ExitCode = this._Process.ExitCode;
                    while (!this._NotLoggedOutputLines.IsEmpty)
                    {
                        Thread.Sleep(60);
                    }
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    this.LogEnd();
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
                catch (Exception exception)
                {
                    this.LogObject.Log("Error while finishing program-execution", exception);
                }
                finally
                {
                    this.Dispose();
                }
            });
            task.Start();
            return task;
        }


        public void Dispose()
        {
            if (this._SubNamespace != null)
            {
                this._SubNamespace.Dispose();
            }
            if (this._Process != null)
            {
                this._Process.Dispose();
            }
            if (_DefaultLog != null)
            {
                _DefaultLog.Dispose();
            }
        }

        private void WaitForProcessEnd(Process process, Stopwatch stopwatch)
        {
            if (this.TimeoutInMilliseconds.HasValue)
            {
                if (!process.WaitForExit(this.TimeoutInMilliseconds.Value))
                {
                    process.Kill();
                    process.WaitForExit();
                    stopwatch.Stop();
                    this.LogObject.Log($"Execution was aborted due to a timeout. (The timeout was set to {Utilities.DurationToUserFriendlyString(TimeSpan.FromMilliseconds(this.TimeoutInMilliseconds.Value))}).", LogLevel.Debug);
                    this.ProcessWasAbortedDueToTimeout = true;
                }
            }
            else
            {
                process.WaitForExit();
                stopwatch.Stop();
            }
            if (process.ExitCode != 0 && this.Verbosity == Verbosity.Normal)
            {
                foreach (string stdOutLine in this._AllStdOutLines)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Information, stdOutLine));
                }
                foreach (string stdErrLine in this._AllStdErrLines)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Error, stdErrLine));
                }
            }
            this._Running = false;
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
            Tuple<string, string, string> temp = Utilities.ResolvePathOfProgram(ProgramPathAndFile, Arguments, WorkingDirectory);
            this.ProgramPathAndFile = temp.Item1;
            this.Arguments = temp.Item2;
            this.WorkingDirectory = temp.Item3;
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

        private readonly IList<string> _AllStdOutLines = new List<string>();
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

        private void EnqueueInformation(string rawLine)
        {
            if (this.NormalizeLine(rawLine, out string line))
            {
                this._AllStdOutLines.Add(line);
                if (this.Verbosity == Verbosity.Full || this.Verbosity == Verbosity.Verbose)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Information, line));
                }
            }
        }

        private void EnqueueError(string rawLine)
        {
            if (this.NormalizeLine(rawLine, out string line))
            {
                this._AllStdErrLines.Add(line);
                if (this.Verbosity == Verbosity.Full || this.Verbosity == Verbosity.Verbose)
                {
                    this._NotLoggedOutputLines.Enqueue((LogLevel.Error, line));
                }
            }
        }

        private bool NormalizeLine(string line, out string data)
        {
            if (line == null)
            {
                data = null;
                return false;
            }
            else
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    data = null;
                    return false;
                }
                else
                {
                    data = line;
                    return true;
                }
            }

        }
        private void LogOutputImplementation()
        {
            while (this.IsRunning || !this._NotLoggedOutputLines.IsEmpty)
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
    }
    /// <summary>
    /// This enum contains the verbosity-level for <see cref="ExternalProgramExecutor"/>.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// No output will be logged.
        /// </summary>
        Quiet = 0,
        /// <summary>
        /// If the exitcode of the executed program is not 0 then the StdErr will be logged.
        /// This is the default-value for <see cref="ExternalProgramExecutor.Verbosity"/>.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Logs StdOut and StdErr of the executed program in realtime.
        /// </summary>
        Full = 2,
        /// <summary>
        /// Same as <see cref="Verbosity.Full"/> but with some more information added by <see cref="ExternalProgramExecutor"/>.
        /// </summary>
        Verbose = 3,
    }
    public enum ExecutionState
    {
        NotStarted = 0,
        Running = 1,
        Terminated = 2
    }
}