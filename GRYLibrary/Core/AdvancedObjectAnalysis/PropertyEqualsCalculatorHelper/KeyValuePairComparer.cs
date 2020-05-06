using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class KeyValuePairComparer : IEqualityComparer<KeyValuePair<object, object>>
    {
        public static IEqualityComparer<KeyValuePair<object, object>> DefaultInstance { get; } = new KeyValuePairComparer();
        private KeyValuePairComparer() { }
        public bool Equals(KeyValuePair<object, object> x, KeyValuePair<object, object> y)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(x.Key, y.Key) && PropertyEqualsCalculator.DefaultInstance.Equals(x.Value, y.Value);
        }

        public int GetHashCode(KeyValuePair<object, object> obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
