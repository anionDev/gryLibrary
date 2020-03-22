﻿using Microsoft.Extensions.Logging;
using System;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Console : GRYLogTarget
    {
        public Console() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logItem.Format(logObject.Configuration, out string formattedMessage, out int cb, out int ce, out ConsoleColor _);
            System.Console.Write(formattedMessage.Substring(0, cb));
            this.WriteWithColorToConsole(formattedMessage[cb..ce], logItem.LogLevel,logObject);
            System.Console.Write(formattedMessage.Substring(ce) + Environment.NewLine);
        }
        private void WriteWithColorToConsole(string message, LogLevel logLevel, GRYLog logObject)
        {
            try
            {
                System.Console.ForegroundColor = logObject.Configuration.GetLoggedMessageTypesConfigurationByLogLevel(logLevel).ConsoleColor;
                System.Console.Write(message);
            }
            finally
            {
                System.Console.ForegroundColor = logObject._ConsoleDefaultColor;
            }
        }
    }
}
