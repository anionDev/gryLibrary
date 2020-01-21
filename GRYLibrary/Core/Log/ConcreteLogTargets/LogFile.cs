using System;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class LogFile : GRYLogTarget
    {
        private LogFile() { }
        public static LogFile Instance { get; } = new LogFile();
        public bool CreateLogFileIfRequiredAndIfPossible { get; set; }
        public string File { get; set; }
        public string Encoding { get; set; } = "utf-8";

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            if (string.IsNullOrWhiteSpace(this.File))
            {
                throw new NullReferenceException($"LogFile is not defined");
            }
            Utilities.EnsureFileExists(this.File);
            logItem.Format(logObject.Configuration, out string formattedMessage, out int _, out int _, out ConsoleColor _);
            if (!Utilities.FileIsEmpty(this.File))
            {
                formattedMessage = Environment.NewLine + formattedMessage;
            }
            System.IO.File.AppendAllText(this.File, formattedMessage, System.Text.Encoding.GetEncoding(this.Encoding));
        }
    }
}
