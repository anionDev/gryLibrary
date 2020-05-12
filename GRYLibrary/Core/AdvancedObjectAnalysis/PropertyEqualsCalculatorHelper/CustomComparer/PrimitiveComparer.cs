using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class PrimitiveComparer : AbstractCustomComparer
    {
        private PrimitiveComparer() { }
        public static AbstractCustomComparer DefaultInstance { get; } = new PrimitiveComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicable(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
