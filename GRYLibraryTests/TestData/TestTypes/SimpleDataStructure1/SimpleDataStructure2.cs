using GRYLibrary.Core;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure1
{
    public class SimpleDataStructure2 : IXmlSerializable
    {
        public Guid Guid { get; set; }

        internal static SimpleDataStructure2 GetTestObject()
        {
            SimpleDataStructure2 result = new SimpleDataStructure2();
            result.Guid = Guid.NewGuid();
            return result;
        }
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
    }
}
