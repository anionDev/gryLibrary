using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Log
{
    public class LoggedMessageTypeConfiguration : IXmlSerializable
    {
        public ConsoleColor ConsoleColor { get; set; }
        public string CustomText { get; set; }
        public LoggedMessageTypeConfiguration() { }
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
        #endregion
    }

}
