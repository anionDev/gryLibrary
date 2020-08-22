using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Log
{
    [XmlInclude(typeof(ConcreteLogTargets.Console))]
    [XmlInclude(typeof(LogFile))]
    [XmlInclude(typeof(Observer))]
    [XmlInclude(typeof(WindowsEventLog))]
    public abstract class GRYLogTarget : IGRYSerializable
    {
        public GRYLogLogFormat Format { get; set; } = GRYLogLogFormat.GRYLogFormat;

        public HashSet<LogLevel> LogLevels { get; set; }
        public bool Enabled { get; set; }
        public abstract ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization();
        public GRYLogTarget()
        {
        }

        public void Initialize()
        {
            Enabled = true;
            this.LogLevels = new HashSet<LogLevel>
            {
                 LogLevel.Information,
                 LogLevel.Warning,
                 LogLevel.Error,
                 LogLevel.Critical
            };
        }
        internal void Execute(LogItem logItem, GRYLog logObject)
        {
            this.ExecuteImplementation(logItem, logObject);
        }
        protected abstract void ExecuteImplementation(LogItem logItem, GRYLog logObject);
        #region Overhead
        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
        }

        public override string ToString()
        {
            return Generic.GenericToString(this);
        }

        public XmlSchema GetSchema()
        {
            return Generic.GenericGetSchema(this);
        }

        public void ReadXml(XmlReader reader)
        {
            Generic.GenericReadXml(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Generic.GenericWriteXml(this, writer);
        }

        public ISet<Type> GetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>() { typeof(LogLevel) ,typeof(GRYLogLogFormat)};
        }
        #endregion
    }
}
