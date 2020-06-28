using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    /// <summary>
    /// Represents a SimplifiedEnumerable for <see cref="GRYSObject"/>
    /// </summary>
    [XmlRoot(ElementName = "SE")]
    public class SEnumerable : Simplified
    {
        public List<Guid> Items { get; set; }

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
