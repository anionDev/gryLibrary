using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace GRYLibrary
{
    public class ExternalProgramExecutor
    {
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, string logfile = null, bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
            : this(programPathAndFile, arguments, title, workingDirectory, GRYLog.Create(logfile), printErrorsAsInformation, timeoutInMilliseconds)
        {
        }
        public ExternalProgramExecutor(string programPathAndFile, string arguments, string title, string workingDirectory, GRYLog logObject = null, bool printErrorsAsInformation = false, int? timeoutInMilliseconds = null)
        {
            this.LogObject = logObject;
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
        private readonly ConcurrentQueue<Tuple<GRYLogLogLevel, string>> _NotLoggedOutputLines = new ConcurrentQueue<Tuple<GRYLogLogLevel, string>>();
        /// <summary>
        /// Starts the program which was set in the properties.
        /// </summary>
        /// <returns>
        /// Returns the exit-code of the executed program.
        /// </returns>
        public int StartConsoleApplicationInCurrentConsoleWindow()
        {
            string originalConsoleTitle = Console.Title;
            Process process = null;
            try
            {
                try
                {
                    Console.Title = this.Title;
                }
                finally
                {
                }
                process = new Process
                {
                    StartInfo = new ProcessStartInfo(this.Resolve(this.ProgramPathAndFile))
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
                    process?.Dispose();
                    Console.Title = originalConsoleTitle;
                }
                finally
                {
                    Utilities.NoOperation();
                }
            }
        }
        private static readonly string WherePath = @"C:\Windows\System32\where.exe";
        private string Resolve(string program)
        {
            if (File.Exists(program))
            {
                return program;
            }
            if (!(program.Contains("/") || program.Contains("\\") || program.Contains(":")))
            {
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
                    Console.WriteLine(output);
                    process.WaitForExit();
                }
                output = output.Replace("\r\n", string.Empty);
                if (File.Exists(output))
                {
                    return output;
                }
            }
            throw new FileNotFoundException($"Program '{program}' can not be found");
        }

        private void EnqueueError(string data)
        {
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Exception, data));
        }

        private void EnqueueInformation(string data)
        {
            this._NotLoggedOutputLines.Enqueue(new Tuple<GRYLogLogLevel, string>(GRYLogLogLevel.Information, data));
        }
        private void LogOutput()
        {
            while (!this._StopLogOutputThread)
            {
                if (this._NotLoggedOutputLines.TryDequeue(out Tuple<GRYLogLogLevel, string> logItem))
                {
                    if (logItem.Item1.Equals(GRYLogLogLevel.Exception))
                    {
                        this.LogObject?.LogError(logItem.Item2);
                    }
                    if (logItem.Item1.Equals(GRYLogLogLevel.Information))
                    {
                        this.LogObject?.LogInformation(logItem.Item2);
                    }
                }
            }
        }
    }
}