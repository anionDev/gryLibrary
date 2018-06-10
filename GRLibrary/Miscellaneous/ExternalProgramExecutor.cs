using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace GRLibrary
{
    public class ExternalProgrammExecutor
    {
        public ExternalProgrammExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, bool printErrorsAsInformation = false, string LogFile = null)
        {
            this.LogObject = new GLog();
            if (LogFile == null)
            {
                this.LogObject.WriteToLogFile = false;
            }
            else
            {
                this.LogObject.LogFile = LogFile;
                this.LogObject.WriteToLogFile = true;
            }
            this.ProgramPathAndFile = programPathAndFile;
            this.Arguments = arguments;
            this.Title = title;
            this.WorkingDirectory = workingDirectory;
            this.PrintErrorsAsInformation = printErrorsAsInformation;
        }
        public GLog LogObject { get; set; }
        public string Arguments { get; set; }
        public string ProgramPathAndFile { get; set; }
        public string Title { get; set; }
        public string WorkingDirectory { get; set; }
        public bool PrintErrorsAsInformation { get; set; }
        private bool _Running = false;
        private readonly ConcurrentQueue<Tuple<GLog.LogLevel, string>> _NotLoggedOutputLines = new ConcurrentQueue<Tuple<GLog.LogLevel, string>>();
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
                _Running = true;
                new Thread(LogOutput).Start();
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
                    _Running = false;
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
            _NotLoggedOutputLines.Enqueue(new Tuple<GLog.LogLevel, string>(GLog.LogLevel.Exception, data));
        }

        private void EnqueueInformation(string data)
        {
            _NotLoggedOutputLines.Enqueue(new Tuple<GLog.LogLevel, string>(GLog.LogLevel.Information, data));
        }
        private void LogOutput()
        {
            while (_Running)
            {
                if (_NotLoggedOutputLines.TryDequeue(out Tuple<GLog.LogLevel, string> logItem))
                {
                    if (logItem.Item1.Equals(GLog.LogLevel.Exception))
                    {
                        this.LogObject.LogError(logItem.Item2);
                    }
                    if (logItem.Item1.Equals(GLog.LogLevel.Information))
                    {
                        this.LogObject.LogInformation(logItem.Item2);
                    }
                }
            }
        }
    }
}