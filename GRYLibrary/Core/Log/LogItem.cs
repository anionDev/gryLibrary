using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log
{
    public struct LogItem
    {
        private string _FormattedMessage;
        private int _ColorBegin;
        private int _ColorEnd;
        private bool _MessageLoaded;
        private bool _FormatingLoaded;
        private ConsoleColor _ConsoleColor;
        private string _PlainMessage;
        private readonly Func<string> _GetMessageFunction;
        public int EventId { get; set; }
        public short Category { get; set; }

        public LogLevel LogLevel { get; internal set; }
        public Exception Exception { get; }
        public DateTime MomentOfLogEntry { get; }

        public string PlainMessage
        {
            get
            {
                if (!this._MessageLoaded)
                {
                    this._PlainMessage = this._GetMessageFunction();
                    this._MessageLoaded = true;
                }
                return this._PlainMessage;
            }
        }
        #region Constructors
        public LogItem(string getMessageFunction) : this(() => getMessageFunction, DateTime.Now)
        {
        }
        public LogItem(string getMessageFunction, LogLevel logLevel) : this(() => getMessageFunction, DateTime.Now, logLevel, null)
        {
        }

        public LogItem(string getMessageFunction, Exception exception) : this(() => getMessageFunction, DateTime.Now, LogLevel.Error, exception)
        {
        }
        public LogItem(string getMessageFunction, LogLevel logLevel, Exception exception) : this(() => getMessageFunction, DateTime.Now, logLevel, exception)
        {
        }
        public LogItem(Func<string> getMessageFunction) : this(getMessageFunction, DateTime.Now, LogLevel.Information)
        {
        }
        public LogItem(Func<string> getMessageFunction, LogLevel logLevel) : this(getMessageFunction, DateTime.Now, logLevel, null)
        {
        }

        public LogItem(Func<string> getMessageFunction, Exception exception) : this(getMessageFunction, DateTime.Now, LogLevel.Error, exception)
        {
        }
        public LogItem(Func<string> getMessageFunction, LogLevel logLevel, Exception exception) : this(getMessageFunction, DateTime.Now, logLevel, exception)
        {
        }
        public LogItem(string getMessageFunction, DateTime dateTime) : this(() => getMessageFunction, dateTime)
        {
        }
        public LogItem(string getMessageFunction, DateTime dateTime, LogLevel logLevel) : this(() => getMessageFunction, dateTime, logLevel, null)
        {
        }

        public LogItem(string getMessageFunction, DateTime dateTime, Exception exception) : this(() => getMessageFunction, dateTime, LogLevel.Error, exception)
        {
        }
        public LogItem(string getMessageFunction, DateTime dateTime, LogLevel logLevel, Exception exception) : this(() => getMessageFunction, dateTime, logLevel, exception)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime) : this(getMessageFunction, dateTime, LogLevel.Information)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime, LogLevel logLevel) : this(getMessageFunction, dateTime, logLevel, null)
        {
        }

        public LogItem(Func<string> getMessageFunction, DateTime dateTime, Exception exception) : this(getMessageFunction, dateTime, LogLevel.Error, exception)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime, LogLevel logLevel, Exception exception) : this()
        {
            this._GetMessageFunction = getMessageFunction;
            this._MessageLoaded = false;
            this._FormatingLoaded = false;
            this.LogLevel = logLevel;
            this.Exception = exception;
            this.MomentOfLogEntry = dateTime;
            this.EventId = 101;
            this.Category = 1;
        }
        #endregion 
        internal void Format(GRYLogConfiguration configuration, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor consoleColor)
        {
            if (!this._FormatingLoaded)
            {
                this.FormatMessage(configuration, this.PlainMessage, this.MomentOfLogEntry, this.LogLevel, out string fm, out int cb, out int ce, out ConsoleColor cc);
                this._FormattedMessage = fm;
                this._ColorBegin = cb;
                this._ColorEnd = ce;
                this._ConsoleColor = cc;
                this._FormatingLoaded = true;
            }
            formattedMessage = this._FormattedMessage;
            colorBegin = this._ColorBegin;
            colorEnd = this._ColorEnd;
            consoleColor = this._ConsoleColor;
        }
        private void FormatMessage(GRYLogConfiguration configuration, string message, DateTime momentOfLogEntry, LogLevel loglevel, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor consoleColor)
        {
            consoleColor = configuration.GetLoggedMessageTypesConfigurationByLogLevel(loglevel).ConsoleColor;
            if (!string.IsNullOrEmpty(configuration.Name))
            {
                message = $"[{configuration.Name.Trim()}] {message}";
            }
            if (configuration.ConvertTimeForLogentriesToUTCFormat)
            {
                momentOfLogEntry = momentOfLogEntry.ToUniversalTime();
            }
            switch (configuration.Format)
            {
                case GRYLogLogFormat.OnlyMessage:
                    formattedMessage = message;
                    colorBegin = 0;
                    colorEnd = 0;
                    break;
                case GRYLogLogFormat.GRYLogFormat:
                    string part1 = "[" + momentOfLogEntry.ToString(configuration.DateFormat) + "] [";
                    string part2 = configuration.GetLoggedMessageTypesConfigurationByLogLevel(loglevel).CustomText;
                    string part3 = "] " + message;
                    formattedMessage = part1 + part2 + part3;
                    colorBegin = part1.Length;
                    colorEnd = part1.Length + part2.Length;
                    break;
                case GRYLogLogFormat.DateOnly:
                    formattedMessage = momentOfLogEntry.ToString(configuration.DateFormat) + " " + message;
                    colorBegin = 0;
                    colorEnd = 0;
                    break;
                default:
                    throw new KeyNotFoundException($"Formatting {nameof(GRYLogLogFormat)} '{loglevel.ToString()}' is not implemented yet.");
            }
        }
    }
}

