using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class TupleComparer : IEqualityComparer<Tuple<object, object>>
    {
        public static IEqualityComparer<Tuple<object, object>> Instance { get; } = new TupleComparer();
        private TupleComparer() { }
        public bool Equals(Tuple<object, object> x, Tuple<object, object> y)
        {
            return new PropertyEqualsCalculator().Equals(x, y);
        }

        public int GetHashCode(Tuple<object, object> obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
