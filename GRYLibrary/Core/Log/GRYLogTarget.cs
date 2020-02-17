using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Log
{
    [XmlInclude(typeof(Console))]
    [XmlInclude(typeof(LogFile))]
    [XmlInclude(typeof(Observer))]
    [XmlInclude(typeof(WindowsEventLog))]
    public abstract class GRYLogTarget
    {
        public HashSet<LogLevel> LogLevels { get; set; } = new HashSet<LogLevel>();
        public bool Enabled { get; set; } 
        public GRYLogTarget()
        {
            this.Enabled = true;
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
