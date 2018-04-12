using System;
using System.Diagnostics;
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
                process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => this.LogObject.LogInformation(e.Data);
                process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (this.PrintErrorsAsInformation)
                    {
                        this.LogObject.LogInformation(e.Data);
                    }
                    else
                    {
                        this.LogObject.LogError(e.Data);
                    }
                };
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
                    Console.Title = originalConsoleTitle;
                }
                finally
                {
                }
            }
        }
    }
}