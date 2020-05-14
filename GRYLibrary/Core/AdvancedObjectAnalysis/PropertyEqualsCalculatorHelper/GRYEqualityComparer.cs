using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public abstract class GRYEqualityComparer<T> : IEqualityComparer<T>
    {
        internal PropertyEqualsCalculatorConfiguration Configuration { get;  set; }
        public abstract bool DefaultEquals(T x, T y);
        public abstract int DefaultGetHashCode(T obj);
        public int GetHashCode(T obj)
        {
            return this.DefaultGetHashCode(obj);
        }
        public bool Equals(T x, T y)
        {
            return this.DefaultEquals(x, y);
        }
    }
}
