using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class ReferenceEqualsComparer : GRYEqualityComparer<object>
    {
        public static GRYEqualityComparer<object> Instance { get; } = new ReferenceEqualsComparer();
        private ReferenceEqualsComparer() { }
        public override  bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return ReferenceEquals(x, y);
        }

        public override  int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
