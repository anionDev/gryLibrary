using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace GRYLibrary
{
    public class ExternalProgramExecutor
    {
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, bool printErrorsAsInformation = false, string logFile = null, int? timeoutInMilliseconds = null)
        {
            this.LogObject = new GRYLog();
            this.LogObject.PrintOutputInConsole = true;
            if (logFile == null)
            {
                this.LogObject.WriteToLogFile = false;
            }
            else
            {
                this.LogObject.WriteToLogFile = true;
                this.LogObject.LogFile = logFile;
            }
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.Title = title;
            this.WorkingDirectory = workingDirectory;
            this.PrintErrorsAsInformation = printErrorsAsInformation;
            this.TimeoutInMilliseconds = timeoutInMilliseconds;
        }
        public bool LogOverhead { get; set; } = false;
        public GRYLog LogObject { get; set; }
        public string Arguments { get; set; }
        public string ProgramPathAndFile { get; set; }
        public string Title { get; set; }
        public string WorkingDirectory { get; set; }
        public int? TimeoutInMilliseconds { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        private bool _StopLogOutputThread = false;
        private readonly ConcurrentQueue<Tuple<GRYLog.LogLevel, string>> _NotLoggedOutputLines = new ConcurrentQueue<Tuple<GRYLog.LogLevel, string>>();
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// Returns the exit-code of the executed program.
        /// </returns>
        public int StartConsoleApplicationInCurrentConsoleWindow()
        {
            string originalConsoleTitle = Console.Title;
            try
            {
                try
                {
                    Console.Title = this.Title;
                }
                finally
                {
                }
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo(this.ProgramPathAndFile)
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
                this._StopLogOutputThread = false;
                SupervisedThread readLogItemsThread = new SupervisedThread(this.LogOutput)
                {
                    Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.ProgramPathAndFile} {this.Arguments}))",
                    LogOverhead = false
                };
                readLogItemsThread.Start();
                if (this.LogOverhead)
                {
                    this.EnqueueInformation($"-----------------------------------------------------");
                    this.EnqueueInformation($"Start '{this.ProgramPathAndFile} {this.Arguments}' in '{this.WorkingDirectory}'");
                }
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
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
                    this.EnqueueInformation($"-----------------------------------------------------");
                }
                return process.ExitCode;
            }
            finally
            {
                try
                {
                    this._StopLogOutputThread = true;
                    Console.Title = originalConsoleTitle;
                }
                finally
                {
                    Utilities.NoOperation();
                }
            }
        }
        private void EnqueueError(string data)
        {
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLog.LogLevel, string>(GRYLog.LogLevel.Exception, data));
        }

        private void EnqueueInformation(string data)
        {
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLog.LogLevel, string>(GRYLog.LogLevel.Information, data));
        }
        private void LogOutput()
        {
            while (!this._StopLogOutputThread)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out Tuple<GRYLog.LogLevel, string> logItem))
                {
                    if (logItem.Item1.Equals(GRYLog.LogLevel.Exception))
                    {
                        this.LogObject.LogError(logItem.Item2);
                    }
                    if (logItem.Item1.Equals(GRYLog.LogLevel.Information))
                    {
                        this.LogObject.LogInformation(logItem.Item2);
                    }
                }
            }
        }
    }
}