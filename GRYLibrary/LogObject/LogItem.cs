using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.LogObject
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
        /**
         * Only relevant for  <see cref="ConcreteLogTargets.WindowsEventLog"/>.
         */
        public int EventId { get; set; }
        /**
         * Only relevant for  <see cref="ConcreteLogTargets.WindowsEventLog"/>.
         */
        public short Category { get; set; }

        public LogLevel LogLevel { get; internal set; }
        public Exception Exception { get; }
        public DateTime MomentOfLogEntry { get; }
        public string MessageId { get; }
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
        public LogItem(string message, string messageId = null) : this(() => message, DateTime.Now, messageId)
        {
        }
        public LogItem(string message, LogLevel logLevel) : this(() => message, DateTime.Now, logLevel, null)
        {
        }

        public LogItem(string message, Exception exception, string messageId = null) : this(() => message, DateTime.Now, LogLevel.Error, exception, messageId)
        {
        }
        public LogItem(string message, LogLevel logLevel, Exception exception, string messageId = null) : this(() => message, DateTime.Now, logLevel, exception, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, string messageId = null) : this(getMessageFunction, DateTime.Now, LogLevel.Information, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, LogLevel logLevel, string messageId = null) : this(getMessageFunction, DateTime.Now, logLevel, null, messageId)
        {
        }

        public LogItem(Func<string> getMessageFunction, Exception exception, string messageId = null) : this(getMessageFunction, DateTime.Now, LogLevel.Error, exception, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, LogLevel logLevel, Exception exception, string messageId) : this(getMessageFunction, DateTime.Now, logLevel, exception, messageId)
        {
        }
        public LogItem(string message, DateTime dateTime, string messageId = null) : this(() => message, dateTime, messageId)
        {
        }
        public LogItem(string message, DateTime dateTime, LogLevel logLevel) : this(() => message, dateTime, logLevel, null)
        {
        }

        public LogItem(string message, DateTime dateTime, Exception exception, string messageId = null) : this(() => message, dateTime, LogLevel.Error, exception, messageId)
        {
        }
        public LogItem(string message, DateTime dateTime, LogLevel logLevel, Exception exception, string messageId = null) : this(() => message, dateTime, logLevel, exception, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime, string messageId = null) : this(getMessageFunction, dateTime, LogLevel.Information, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime, LogLevel logLevel, string messageId = null) : this(getMessageFunction, dateTime, logLevel, null, messageId)
        {
        }

        public LogItem(Func<string> getMessageFunction, DateTime dateTime, Exception exception, string messageId = null) : this(getMessageFunction, dateTime, LogLevel.Error, exception, messageId)
        {
        }
        public LogItem(Func<string> getMessageFunction, DateTime dateTime, LogLevel logLevel, Exception exception, string messageId) : this()
        {
            this._GetMessageFunction = getMessageFunction;
            this._MessageLoaded = false;
            this._FormatingLoaded = false;
            this.MessageId = messageId;
            this.LogLevel = logLevel;
            this.Exception = exception;
            this.MomentOfLogEntry = dateTime;
            this.EventId = 101;
            this.Category = 1;
        }
        #endregion 
        internal void Format(GRYLogConfiguration configuration, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor consoleColor, GRYLogLogFormat format, string messageIdValue)
        {
            if (!this._FormatingLoaded)
            {
                this.FormatMessage(configuration, this.PlainMessage, this.MomentOfLogEntry, this.LogLevel, format, out string fm, out int cb, out int ce, out ConsoleColor cc, messageIdValue);
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
        public bool IsErrorEntry()
        {
            return this.LogLevel == LogLevel.Critical || this.LogLevel == LogLevel.Error;
        }
        private void FormatMessage(GRYLogConfiguration configuration, string message, DateTime momentOfLogEntry, LogLevel loglevel, GRYLogLogFormat format, out string formattedMessage, out int colorBegin, out int colorEnd, out ConsoleColor consoleColor, string messageIdValue)
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
            switch (format)
            {
                case GRYLogLogFormat.OnlyMessage:
                    formattedMessage = message;
                    colorBegin = 0;
                    colorEnd = 0;
                    break;
                case GRYLogLogFormat.GRYLogFormat:
                    string messageId;
                    if (string.IsNullOrWhiteSpace(messageIdValue))
                    {
                        messageId = string.Empty;
                    }
                    else
                    {
                        messageId = $"[{messageIdValue}] ";
                    }
                    string part1 = $"[{momentOfLogEntry.ToString(configuration.DateFormat)}] {messageId}[";
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
                    throw new KeyNotFoundException($"Formatting {nameof(GRYLogLogFormat)} '{loglevel}' is not implemented yet.");
            }
        }

        public override bool Equals(object obj)
        {
            return obj is LogItem item &&
                   _PlainMessage == item._PlainMessage &&
                   EventId == item.EventId &&
                   Category == item.Category &&
                   LogLevel == item.LogLevel &&
                   MomentOfLogEntry == item.MomentOfLogEntry &&
                   MessageId == item.MessageId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_PlainMessage, EventId, Category, LogLevel, MomentOfLogEntry, MessageId);
        }

        public static bool operator ==(LogItem left, LogItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LogItem left, LogItem right)
        {
            return !(left == right);
        }
    }
}

