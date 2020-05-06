using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.GenericXMLSerializerHelper.ConcreteSimplifiedObjects
{
    public class SimplifiedEnumerable : Simplified
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
