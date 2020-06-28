using GRYLibrary.Core.AdvancedObjectAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GRYLibrary.TestData.TestTypes.AdvancedObjectAnalysisUsageFull
{
    public class TestType : IXmlSerializable
    {
        public TestType()
        {
        }
        public TestAttributeType AttributeA { get; set; }
        public TestAttributeType AttributeB { get; set; }
        public static TestType GetRandom()
        {
            TestType result = new TestType();
            result.AttributeA = new TestAttributeType();
            result.AttributeB = new TestAttributeType();
            result.AttributeA.ComplexAttribute = result.AttributeB;
            result.AttributeA.IntAttribute = 2;
            result.AttributeB.ComplexAttribute = result.AttributeA;
            result.AttributeB.IntAttribute = 3;
            return result;
        }
        #region Overhead
        public override bool Equals(object obj)
        {
            return Generic.GenericEquals(this, obj);
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
