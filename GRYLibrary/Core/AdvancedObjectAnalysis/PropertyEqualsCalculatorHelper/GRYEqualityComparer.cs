using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public abstract class GRYEqualityComparer<T> : IEqualityComparer<T>
    {
        internal PropertyEqualsCalculatorConfiguration Configuration { get;  set; }
        internal abstract bool DefaultEquals(T item1, T item2);
        internal abstract int DefaultGetHashCode(T @object);
        public int GetHashCode(T @object)
        {
            return this.DefaultGetHashCode(@object);
        }
        public bool Equals(T item1, T item2)
        {
            return this.DefaultEquals(item1, item2);
        }
    }
}
