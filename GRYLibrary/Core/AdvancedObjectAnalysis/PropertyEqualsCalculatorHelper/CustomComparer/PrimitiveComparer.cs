using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class PrimitiveComparer : AbstractCustomComparer
    {
        private PrimitiveComparer() { }
        public static PrimitiveComparer DefaultInstance { get; } = new PrimitiveComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return type.IsPrimitive || typeof(string).Equals(type) || type.IsValueType;
        }
    }
}
