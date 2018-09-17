using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace GRYLibrary
{
    public class ExternalProgramExecutor
    {
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, bool printErrorsAsInformation = false, string logFile = null)
        {
            this.LogObject = new GRYLog();
            if (logFile == null)
            {
                this.LogObject.WriteToLogFile = false;
            }
            else
            {
                this.LogObject.LogFile = logFile;
                this.LogObject.WriteToLogFile = true;
            }
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.Title = title;
            this.WorkingDirectory = workingDirectory;
            this.PrintErrorsAsInformation = printErrorsAsInformation;
        }
        public GRYLog LogObject { get; set; }
        public string Arguments { get; set; }
        public string ProgramPathAndFile { get; set; }
        public string Title { get; set; }
        public string WorkingDirectory { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        private bool _Running = false;
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
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => EnqueueInformation(e.Data);
                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (this.PrintErrorsAsInformation)
                    {
                        EnqueueInformation(e.Data);
                    }
                    else
                    {
                        EnqueueError(e.Data);
                    }
                };
                this._Running = true;
                SupervisedThread thread = new SupervisedThread(this.LogOutput)
                {
                    Name = $"Logger-Thread for '{this.Title}' ({nameof(ExternalProgramExecutor)}({this.ProgramPathAndFile} {this.Arguments}))"
                };
                thread.Start();
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return process.ExitCode;
            }
            finally
            {
                try
                {
                    this._Running = false;
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
            while (this._Running)
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