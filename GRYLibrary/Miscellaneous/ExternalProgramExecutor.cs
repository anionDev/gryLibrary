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
        public string Title { get; set; }
        public string WorkingDirectory { get; set; }
        public int? TimeoutInMilliseconds { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        private bool _LogOutputThreadStopped = false;
        private bool _Running = false;
        public bool Running()
        {
            return _Running;
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
                string originalConsoleTitle = Console.Title;
                Process process = null;
                try
                {
                    try
                    {
                        Console.Title = this.Title;
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo(this.ResolvePathOfProgram(this.ProgramPathAndFile))
                        {
                            UseShellExecute = false,
                            ErrorDialog = false,
                            Arguments = this.Arguments,
                            WorkingDirectory = this.WorkingDirectory,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    };
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
                    this._LogOutputThreadStopped = false;
                    SupervisedThread readLogItemsThread = SupervisedThread.Create(this.LogOutput);
                    readLogItemsThread.Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.ProgramPathAndFile} {this.Arguments}))";
                    readLogItemsThread.LogOverhead = this.LogOverhead;
                    if (this.LogOverhead)
                    {
                        this.EnqueueInformation($"Start '{this.ProgramPathAndFile} {this.Arguments}' in '{this.WorkingDirectory}'");
                    }
                    process.Start();
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
                    if (this.LogOverhead)
                    {
                        this.EnqueueInformation($"Finished '{this.ProgramPathAndFile} {this.Arguments}'");
                    }
                    this.ExitCode = process.ExitCode;
                    this._AllStdErrLinesAsArray = this._AllStdErrLines.ToArray();
                    this._AllStdOutLinesAsArray = this._AllStdOutLines.ToArray();
                    this.ExecutionState = ExecutionState.Terminated;
                    return this.ExitCode;
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
                        Console.Title = originalConsoleTitle;
                    }
                    catch
                    {
                        Utilities.NoOperation();
                    }
                }
            }
        }
        private static readonly string WherePath = @"C:\Windows\System32\where.exe";
        private string ResolvePathOfProgram(string program)
        {
            if (File.Exists(program))
            {
                return program;
            }
            if (!(program.Contains("/") || program.Contains("\\") || program.Contains(":")))
            {
                int exitCode = 1;
                string output = string.Empty;
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = WherePath;
                    process.StartInfo.Arguments = program;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    StreamReader reader = process.StandardOutput;
                    output = reader.ReadToEnd();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }
                if (exitCode == 0)
                {
                    output = output.Replace("\r\n", string.Empty);
                    if (File.Exists(output))
                    {
                        return output;
                    }
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
            return $"{name} is not available if the execution state is not terminated.";
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
            this._AllStdErrLines.Add(data);
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Exception, data));
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
            this._AllStdOutLines.Add(data);
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Information, data));
        }
        private void LogOutput()
        {
            while (Running() || this._NotLoggedOutputLines.Count > 0)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out Tuple<GRYLogLogLevel, string> logItem))
                {
                    if (logItem.Item1.Equals(GRYLogLogLevel.Exception))
                    {
                        this.LogObject?.Log(logItem.Item2);
                    }
                    if (logItem.Item1.Equals(GRYLogLogLevel.Information))
                    {
                        this.LogObject?.Log(logItem.Item2);
                    }
                }
            }
            _LogOutputThreadStopped = true;
        }
    }
    public enum ExecutionState
    {
        NotStarted = 0,
        Running = 1,
        Terminated = 2
    }
}