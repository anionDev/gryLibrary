using GRYLibrary.Core;
using GRYLibrary.Core.XMLSerializer;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure1
{
    public class SimpleDataStructure3 : IXmlSerializable
    {
        public string Property4 { get; set; }
        public IList<SimpleDataStructure2> Property5 { get; set; }

        internal static SimpleDataStructure3 GetTestObject()
        {
            var result = new SimpleDataStructure3();

            result.Property4 = "Property4_" + Guid.NewGuid().ToString();
            result.Property5 = new List<SimpleDataStructure2>();
            result.Property5.Add(SimpleDataStructure2.GetTestObject());
            result.Property5.Add(SimpleDataStructure2.GetTestObject());
            return result;
        }
        public XmlSchema GetSchema()
        {
            return new CustomizableXMLSerializer().GenericGetXMLSchema(this.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            new CustomizableXMLSerializer().GenericXMLDeserializer(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            new CustomizableXMLSerializer().GenericXMLSerializer(this, writer);
        }
    }
}
