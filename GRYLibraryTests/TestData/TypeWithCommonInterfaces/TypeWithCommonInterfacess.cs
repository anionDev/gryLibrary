using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.Tests.TestData.TypeWithCommonInterfaces
{
    public class TypeWithCommonInterfacess : IXmlSerializable
    {
        public IList<int> List { get; set; }
        public IEnumerable<int> Enumerable { get; set; }
        public ISet<int> Set { get; set; }
        public IDictionary<int, int> Dictionary { get; set; }

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
