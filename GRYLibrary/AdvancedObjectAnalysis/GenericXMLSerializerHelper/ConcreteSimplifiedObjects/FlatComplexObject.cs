using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    /// <summary>
    /// Represents a SimplifiedObject for <see cref="GRYSObject"/>
    /// </summary>
    [XmlRoot(ElementName = "FO")]
    public class FlatComplexObject : FlatObject
    {
        public List<FlatAttribute> Attributes { get; set; } = new List<FlatAttribute>();

        public override void Accept(IFlatObjectVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IFlatObjectVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }

}
