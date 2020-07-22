using GRYLibrary.Core.AdvancedObjectAnalysis;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.TestData.TestTypes.SimpleDataStructure
{
    public class SimpleDataStructure3
    {
        public string Property4 { get; set; }
        public List<SimpleDataStructure2> Property5 { get; set; }

        internal static SimpleDataStructure3 GetRandom()
        {
            SimpleDataStructure3 result = new SimpleDataStructure3
            {
                Property4 = "Property4_e7df34db-bb6f-4a11-8c6d-66bccafbd041",
                Property5 = new List<SimpleDataStructure2>
                {
                    new SimpleDataStructure2() { Guid = Guid.Parse("a54f4945-e928-4296-bf9b-e9ae16b35744") },
                    new SimpleDataStructure2() { Guid = Guid.Parse("1735ece2-942f-4380-aec4-27aaa4021ed5") }
                }
            };
            return result;
        }

        #region Overhead
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
        #endregion
    }
}
