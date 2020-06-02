using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    /// <summary>
    /// Represents a SimplifiedObject for <see cref="GRYSObject"/>
    /// </summary>
    [XmlRoot(ElementName = "SO")]
    public class SObject : Simplified
    {
        public List<SAttribute> Attributes { get; set; } = new List<SAttribute>();

        public override void Accept(ISimplifiedVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ISimplifiedVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }

}
