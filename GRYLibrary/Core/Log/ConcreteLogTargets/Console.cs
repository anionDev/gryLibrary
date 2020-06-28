using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Console : GRYLogTarget
    {
        public Console() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            TextWriter output;
            if (logItem.IsErrorEntry())
            {
                output = System.Console.Error;
            }
            else
            {
                output = System.Console.Out;
            }
            logItem.Format(logObject.Configuration, out string formattedMessage, out int cb, out int ce, out ConsoleColor _);
            output.Write(formattedMessage.Substring(0, cb));
            this.WriteWithColorToConsole(formattedMessage[cb..ce], output, logItem.LogLevel, logObject);
            output.Write(formattedMessage.Substring(ce) + Environment.NewLine);
        }
        private void WriteWithColorToConsole(string message, TextWriter output, LogLevel logLevel, GRYLog logObject)
        {
            try
            {
                System.Console.ForegroundColor = logObject.Configuration.GetLoggedMessageTypesConfigurationByLogLevel(logLevel).ConsoleColor;
                output.Write(message);
            }
            finally
            {
                System.Console.ForegroundColor = logObject._ConsoleDefaultColor;
            }
        }
    }
}
