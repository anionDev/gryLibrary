using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public abstract class GRYEqualityComparer<T> : IEqualityComparer<T>
    {
        public abstract bool Equals(T x, T y, ISet<PropertyEqualsCalculatorTuple> visitedObjects);
        public abstract int GetHashCode(T obj);
        public bool Equals(T x, T y)
        {
            return this.Equals(x, y, new HashSet<PropertyEqualsCalculatorTuple>());
        }
    }
}
