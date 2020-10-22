using GRYLibrary.Core.Exceptions;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.OperatingSystem;
using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GRYLibrary.Core
{
    public class ExternalProgramExecutor
    {
        public static ExternalProgramExecutor CreateByLogFile(string programPathAndFile, string arguments, string logFile, string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return CreateByGRYLog(programPathAndFile, arguments, GRYLog.Create(logFile), workingDirectory, title, printErrorsAsInformation, timeoutInMilliseconds);
        }
        public static ExternalProgramExecutor CreateByGRYLog(string programPathAndFile, string arguments, GRYLog log, string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return new ExternalProgramExecutor(programPathAndFile, arguments, title, workingDirectory, log, printErrorsAsInformation, timeoutInMilliseconds);
        }
        public static ExternalProgramExecutor Create(string programPathAndFile, string arguments = "", string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return new ExternalProgramExecutor(programPathAndFile, arguments, title, workingDirectory, GRYLog.Create(), printErrorsAsInformation, timeoutInMilliseconds);
        }
        private ExternalProgramExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, GRYLog logObject, bool printErrorsAsInformation, int? timeoutInMilliseconds)
        {
            this.LogObject = logObject;
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.Title = title;
            this.WorkingDirectory = workingDirectory;
            this.PrintErrorsAsInformation = printErrorsAsInformation;
            this.TimeoutInMilliseconds = timeoutInMilliseconds;
        }
        public ExecutionState ExecutionState { get; private set; } = ExecutionState.NotStarted;
        public bool LogOverhead { get; set; } = false;
        public GRYLog LogObject { get; set; }
        public string Arguments { get; set; }
        public string ProgramPathAndFile { get; set; }
        public bool CreateWindow { get; set; } = true;
        public string Title { get; set; }
        public string WorkingDirectory { get; set; }
        /// <remarks>
        /// This property will be ignored if <see cref="RunSynchronously"/>==false.
        /// </remarks>
        public bool ThrowErrorIfExitCodeIsNotZero { get; set; } = false;
        public int? TimeoutInMilliseconds { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        public bool RunSynchronously { get; set; } = true;
        public delegate void ExecutionFinishedHandler(ExternalProgramExecutor sender, int exitCode);
        public event ExecutionFinishedHandler ExecutionFinishedEvent;
        private bool _Running = false;
        private TimeSpan _ExecutionDuration = default;
        public TimeSpan ExecutionDuration
        {
            get
            {
                if (this.ExecutionState == ExecutionState.Terminated)
                {
                    return this._ExecutionDuration;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExecutionDuration)));
                }
            }
            private set { this._ExecutionDuration = value; }
        }
        public bool Running()
        {
            return this._Running;
        }
        private readonly object _LockObject = new object();
        private readonly ConcurrentQueue<(LogLevel, string)> _NotLoggedOutputLines = new ConcurrentQueue<(LogLevel, string)>();
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// If <see cref="RunSynchronously"/>==true then the exit-code of the executed program will be returned.
        /// If <see cref="RunSynchronously"/>==false then the process-id of the executed program will be returned.
        /// </returns>
        /// <exception cref="UnexpectedExitCodeException">
        /// Will be thrown if <see cref="ThrowErrorIfExitCodeIsNotZero"/> and the exitcode of the executed program is not 0.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Will be thrown if <see cref="Start"/> was already called.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Will be thrown if the given working-directory does not exist.
        /// </exception>
        /// <exception cref="ProcessStartException">
        /// Will be thrown if the process could not be started.
        /// </exception>
        public int Start()
        {
            this.CheckIfStartOperationWasAlreadyCalled();
            string originalConsoleTitle = null;
            try
            {
                originalConsoleTitle = Console.Title;
            }
            catch
            {
                Utilities.NoOperation();
            }
            this.ProcessWasAbortedDueToTimeout = false;
            Process process = new Process();
            try
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(this.Title))
                    {
                        Console.Title = this.Title;
                    }
                }
                catch
                {
                    Utilities.NoOperation();
                }
                this.ResolvePaths();
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
                process.StartInfo = StartInfo;
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    this.EnqueueInformation(e.Data);
                };
                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (this.PrintErrorsAsInformation)
                    {
                        this.EnqueueInformation(e.Data);
                    }
                    else
                    {
                        this.EnqueueError(e.Data);
                    }
                };
                SupervisedThread readLogItemsThread;
                string executionInfoAsString = $"{this.WorkingDirectory}>{this.ProgramPathAndFile} {this.Arguments}";
                if (this.LogOverhead)
                {
                    this.LogObject.Log($"Start '{executionInfoAsString}'", LogLevel.Debug);
                }
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                try
                {
                    process.Start();
                }
                catch (Exception exception)
                {
                    throw new ProcessStartException($"Exception occurred while start execution '{executionInfoAsString}'", exception);
                }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                this._Running = true;
                if (this.LogOutput)
                {
                    readLogItemsThread = SupervisedThread.Create(this.LogOutputImplementation);
                    readLogItemsThread.Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({executionInfoAsString}))";
                    readLogItemsThread.LogOverhead = this.LogOverhead;
                    readLogItemsThread.Start();
                }
                Task<int> task = new Task<int>(() =>
                {
                    WaitForProcessEnd(process);
                    stopWatch.Stop();
                    this.ExecutionDuration = stopWatch.Elapsed;
                    this.ExitCode = process.ExitCode;
                    while (0 < this._NotLoggedOutputLines.Count)
                    {
                        Thread.Sleep(60);
                    }
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    if (this.LogOverhead)
                    {
                        this.LogObject.Log($"Finished '{this.ProgramPathAndFile} {this.Arguments}'", LogLevel.Debug);
                        this.LogObject.Log(this.GetResult(), LogLevel.Debug);
                    }
                    ExecutionFinishedEvent?.Invoke(this, this.ExitCode);
                    process.Dispose();
                    if (this.RunSynchronously && this.ThrowErrorIfExitCodeIsNotZero && this.ExitCode != 0)
                    {
                        throw new UnexpectedExitCodeException($"'{executionInfoAsString}' had exitcode {this.ExitCode}. Duration: {Utilities.DurationToUserFriendlyString(this.ExecutionDuration)}", this);
                    }
                    else
                    {
                        return this.ExitCode;
                    }
                });
                task.Start();
                int result;
                if (RunSynchronously)
                {
                    task.Wait();
                    result = task.Result;
                }
                else
                {
                    result = process.Id;
                }
                return result;
            }
            finally
            {
                try
                {
                    this._Running = false;
                    if (originalConsoleTitle != null)
                    {
                        Console.Title = originalConsoleTitle;
                    }
                }
                catch
                {
                    Utilities.NoOperation();
                }
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
            this.ExecutionState = ExecutionState.Terminated;
        }
        private void CheckIfStartOperationWasAlreadyCalled()
        {
            lock (this._LockObject)
            {
                if (this.ExecutionState != ExecutionState.NotStarted)
                {
                    throw new InvalidOperationException("The process was already started.");
                }
                this.ExecutionState = ExecutionState.Running;
            }
        }

        private void ResolvePaths()
        {
            string newProgram = this.ProgramPathAndFile;
            string newArgument = this.Arguments;
            Utilities.ResolvePathOfProgram(ref newProgram, ref newArgument);
            this.ProgramPathAndFile = newProgram;
            ///TODO fix quotes (for exmaple like this: this.Arguments = newArgument.Replace("\"", "\"\"\"");// '"' must be escaped by '"""', see https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.arguments(v=vs.110).aspx )
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
                if (this.ExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdErrLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdErrLines)));
                }
            }
        }

        private string GetInvalidOperationDueToNotTerminatedMessageByMembername(string name)
        {
            return $"'{name}' is not available if the execution state is not '{nameof(ExecutionState.Terminated)}'.";
        }

        private bool _processWasAbortedDueToTimeout;
        /// <exception cref="InvalidOperationException">
        /// If the process is not terminated yet.
        /// </exception>
        public bool ProcessWasAbortedDueToTimeout
        {
            get
            {
                if (this.ExecutionState == ExecutionState.Terminated)
                {
                    return this._processWasAbortedDueToTimeout;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ProcessWasAbortedDueToTimeout)));
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
                if (this.ExecutionState == ExecutionState.Terminated)
                {
                    return this._ExitCode;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.ExitCode)));
                }
            }
            private set
            {
                this._ExitCode = value;
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
                if (this.ExecutionState == ExecutionState.Terminated)
                {
                    return this._AllStdOutLinesAsArray;
                }
                else
                {
                    throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.AllStdOutLines)));
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
            while (this.Running() || this._NotLoggedOutputLines.Count > 0)
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
            if (this.ExecutionState == ExecutionState.Terminated)
            {
                string result = $"{nameof(ExternalProgramExecutor)}-summary:";
                result = result + Environment.NewLine + $"Executed program: {this.WorkingDirectory}>{this.ProgramPathAndFile} {this.Arguments}";
                result = result + Environment.NewLine + $"Exit-code: {this.ExitCode}";
                result = result + Environment.NewLine + $"Execution-duration: {this.ExecutionDuration:d'd 'h'h 'm'm 's's'} ({this.ExecutionDuration.TotalSeconds} seconds total)";
                result = result + Environment.NewLine + $"StdOut:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdOutLines);
                result = result + Environment.NewLine + $"StdErr:" + Environment.NewLine + string.Join(Environment.NewLine + "    ", this.AllStdErrLines);
                return result;
            }
            else
            {
                throw new InvalidOperationException(this.GetInvalidOperationDueToNotTerminatedMessageByMembername(nameof(this.GetResult)));
            }
        }
    }
    public enum ExecutionState
    {
        NotStarted = 0,
        Running = 1,
        Terminated = 2
    }
    public class UnexpectedExitCodeException : Exception
    {
        public ExternalProgramExecutor ExecutedProgram { get; }
        public UnexpectedExitCodeException(string message, ExternalProgramExecutor externalProgramExecutor) : base(message)
        {
            this.ExecutedProgram = externalProgramExecutor;
        }
    }
}