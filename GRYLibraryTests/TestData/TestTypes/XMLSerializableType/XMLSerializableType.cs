using GRYLibrary.Core.AdvancedObjectAnalysis;
using GRYLibrary.TestData.TestTypes.CyclicDataStructure;
using GRYLibrary.TestData.TestTypes.SimpleDataStructure;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.TestData.TestTypes.XMLSerializableType
{
    public class XMLSerializableType : IXmlSerializable
    {
        public XMLSerializableType() { }
        public CycleA Cyle { get; set; }
        public SimpleDataStructure1 SimpleDataStructure1 { get; set; }
        internal static XMLSerializableType GetRandom()
        {
            XMLSerializableType result = new XMLSerializableType
            {
                Cyle = CycleA.GetRandom(),
                SimpleDataStructure1 = SimpleDataStructure1.GetRandom()
            };
            return result;
        }

        public override bool Equals(object @object)
        {
            return Generic.GenericEquals(this, @object);
        }

        public override int GetHashCode()
        {
            return Generic.GenericGetHashCode(this);
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

    }
}
