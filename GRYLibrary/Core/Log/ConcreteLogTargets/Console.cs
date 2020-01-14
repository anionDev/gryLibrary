using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Console : GRYLogTarget
    {
        private Console() { }
        public static Console Instance { get; } = new Console();
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logItem.Format(logObject.Configuration, out string formattedMessage, out int cb, out int ce, out ConsoleColor cc);
            System.Console.Write(formattedMessage.Substring(0, cb));
            this.WriteWithColorToConsole(formattedMessage.Substring(cb, ce - cb), logItem.LogLevel,logObject);
            System.Console.Write(formattedMessage.Substring(ce) + Environment.NewLine);
        }
        private void WriteWithColorToConsole(string message, LogLevel logLevel, GRYLog logObject)
        {
            try
            {
                System.Console.ForegroundColor = logObject.Configuration.LoggedMessageTypesConfiguration[logLevel].ConsoleColor;
                System.Console.Write(message);
            }
            finally
            {
                System.Console.ForegroundColor = logObject._ConsoleDefaultColor;
            }
        }
    }
}
