using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log
{
    public abstract class GRYLogTarget
    {
        public ISet<LogLevel> LogLevels { get; set; } = new HashSet<LogLevel>();
        public bool Enabled { get; set; } = true;
        public GRYLogTarget()
        {
            this.LogLevels.Add(LogLevel.Information);
            this.LogLevels.Add(LogLevel.Warning);
            this.LogLevels.Add(LogLevel.Error);
            this.LogLevels.Add(LogLevel.Critical);
        }
        internal void Execute(LogItem logItem, GRYLog logObject)
        {
            this.ExecuteImplementation(logItem, logObject);
        }
        protected abstract void ExecuteImplementation(LogItem logItem, GRYLog logObject);
    }
}
