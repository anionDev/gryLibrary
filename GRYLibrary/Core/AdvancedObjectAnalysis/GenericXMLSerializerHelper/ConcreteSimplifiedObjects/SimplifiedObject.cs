using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    public class SimplifiedObject : Simplified
    {
        public List<SimplifiedAttribute> Attributes { get; set; } = new List<SimplifiedAttribute>();

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
