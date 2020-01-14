using GRYLibrary.Core;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure1
{
    public class SimpleDataStructure1 : IXmlSerializable
    {
        public IList<SimpleDataStructure3> Property1 { get; set; }
        public SimpleDataStructure2 Property2 { get; set; }
        public int Property3 { get; set; }

        public static SimpleDataStructure1 GetTestObject()
        {
            SimpleDataStructure1 result = new SimpleDataStructure1();

            result.Property1 = new List<SimpleDataStructure3>();
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property1.Add(SimpleDataStructure3.GetTestObject());
            result.Property2 = SimpleDataStructure2.GetTestObject();
            result.Property3 = 21;
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
