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

        public XmlSchema GetSchema()
        {
            return Utilities.GenericGetXMLSchema(this.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            Utilities.GenericXMLDeserializer(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            Utilities.GenericXMLSerializer(this, writer);
        }
        public override int GetHashCode()
        {
            return Utilities.GenericGetHashCode(this);
        }
        public override bool Equals(object obj)
        {
            return Utilities.GenericEquals(this, obj);
        }
    }

}
