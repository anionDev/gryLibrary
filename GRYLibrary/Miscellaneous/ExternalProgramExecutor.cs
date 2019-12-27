using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GRYLibrary
{
    public class ExternalProgramExecutor
    {
        public static ExternalProgramExecutor CreateByLogFile(string programPathAndFile, string arguments, string logFile, string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return CreateWithGRYLog(programPathAndFile, arguments, GRYLog.Create(logFile), workingDirectory, title, printErrorsAsInformation, timeoutInMilliseconds);
        }
        public static ExternalProgramExecutor CreateWithGRYLog(string programPathAndFile, string arguments, GRYLog log, string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return new ExternalProgramExecutor(programPathAndFile, arguments, title, workingDirectory, log, printErrorsAsInformation, timeoutInMilliseconds);
        }
        public static ExternalProgramExecutor Create(string programPathAndFile, string arguments, string workingDirectory = "", string title = "", bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            return CreateByLogFile(programPathAndFile, arguments, string.Empty, workingDirectory, title, printErrorsAsInformation, timeoutInMilliseconds);
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
        public bool ThrowErrorIfExitCodeIsNotZero { get; set; } = false;
        public int? TimeoutInMilliseconds { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
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
        private readonly ConcurrentQueue<Tuple<GRYLogLogLevel, string>> _NotLoggedOutputLines = new ConcurrentQueue<Tuple<GRYLogLogLevel, string>>();
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// Returns the exit-code of the executed program.
        /// </returns>
        public int StartConsoleApplicationInCurrentConsoleWindow()
        {
            lock (this._NotLoggedOutputLines)
            {
                if (this.ExecutionState != ExecutionState.NotStarted)
                {
                    throw new InvalidOperationException("This process was already started");
                }
                this.ExecutionState = ExecutionState.Running;
                string originalConsoleTitle = null;
                try
                {
                    originalConsoleTitle = Console.Title;
                }
                catch
                {
                }
                Process process = null;
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
                    ProcessStartInfo StartInfo = new ProcessStartInfo(this.ResolvePathOfProgram(this.ProgramPathAndFile))
                    {
                        UseShellExecute = false,
                        ErrorDialog = false,
                        Arguments = this.Arguments,
                        WorkingDirectory = this.WorkingDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    StartInfo.CreateNoWindow = !this.CreateWindow;
                    process = new Process();
                    process.StartInfo = StartInfo;
                    process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => this.EnqueueInformation(e.Data);
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
                    SupervisedThread readLogItemsThread = SupervisedThread.Create(this.LogOutput);
                    readLogItemsThread.Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.ProgramPathAndFile} {this.Arguments}))";
                    readLogItemsThread.LogOverhead = this.LogOverhead;
                    if (this.LogOverhead)
                    {
                        this.EnqueueInformation($"Start '{this.ProgramPathAndFile} {this.Arguments}' in '{this.WorkingDirectory}'");
                    }
                    Stopwatch stopWatch = new Stopwatch();
                    string executionInfoAsString = $"{ this.WorkingDirectory }>{ this.ProgramPathAndFile } { this.Arguments }";
                    stopWatch.Start();
                    try
                    {
                        process.Start();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Exception occurred while start execution '{executionInfoAsString}'", exception);
                    }
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    this._Running = true;
                    readLogItemsThread.Start();
                    if (this.TimeoutInMilliseconds.HasValue)
                    {
                        if (!process.WaitForExit(this.TimeoutInMilliseconds.Value))
                        {
                            process.Kill();
                        }
                    }
                    else
                    {
                        process.WaitForExit();
                    }
                    stopWatch.Stop();
                    this.ExecutionDuration = stopWatch.Elapsed;
                    if (this.LogOverhead)
                    {
                        this.EnqueueInformation($"Finished '{this.ProgramPathAndFile} {this.Arguments}'");
                    }
                    this.ExitCode = process.ExitCode;
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    this.ExecutionState = ExecutionState.Terminated;
                    if (this.ThrowErrorIfExitCodeIsNotZero && this.ExitCode != 0)
                    {
                        throw new UnexpectedExitCodeException($"'{executionInfoAsString}' had exitcode {this.ExitCode.ToString()}.", this);
                    }
                    else
                    {
                        return this.ExitCode;
                    }
                }
                finally
                {
                    try
                    {
                        this._Running = false;
                        while (this._NotLoggedOutputLines.Count > 0)
                        {
                            System.Threading.Thread.Sleep(30);
                        }
                        process?.Dispose();
                        if (originalConsoleTitle != null)
                        {
                            try
                            {
                                Console.Title = originalConsoleTitle;
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                }
            }
        }
        private string ResolvePathOfProgram(string program)
        {
            if (File.Exists(program))
            {
                return program;
            }
            if (!(program.Contains("/") || program.Contains("\\") || program.Contains(":")))
            {
                if (Utilities.TryResolvePathByPathVariable(program, out string programWithFullPath))
                {
                    return programWithFullPath;
                }
            }
            throw new FileNotFoundException($"Program '{program}' can not be found");
        }

        private readonly IList<string> _AllStdErrLines = new List<string>();
        private string[] _AllStdErrLinesAsArray;
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

        private int _ExitCode;
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
                this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Exception, data));
            }
        }
        private readonly IList<string> _AllStdOutLines = new List<string>();
        private string[] _AllStdOutLinesAsArray;
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
                this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Information, data));
            }
        }
        private void LogOutput()
        {
            while (this.Running() || this._NotLoggedOutputLines.Count > 0)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out Tuple<GRYLogLogLevel, string> logItem))
                {
                    this.LogObject?.Log(logItem.Item2, logItem.Item1);
                }
            }
        }

        /// <returns>Returns a summary of the executed program with its error-code, console-outputs, etc.</returns>
        /// <remarks>This summary is designed for readability and not for a further program-controlled processing of the data. For that purpose please read out the properties of this object.</remarks>
        public string GetResult()
        {
            if (this.ExecutionState == ExecutionState.Terminated)
            {
                string result = $"{nameof(ExternalProgramExecutor)}-summary:";
                result = result + Environment.NewLine + $"Executed program: {this.WorkingDirectory}>{this.ProgramPathAndFile} {this.Arguments}";
                result = result + Environment.NewLine + $"Exit-code: {this.ExitCode}";
                result = result + Environment.NewLine + $"Execution-duration: {this.ExecutionDuration.ToString("d'd 'h'h 'm'm 's's'")} ({this.ExecutionDuration.TotalSeconds.ToString()} seconds total)";
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