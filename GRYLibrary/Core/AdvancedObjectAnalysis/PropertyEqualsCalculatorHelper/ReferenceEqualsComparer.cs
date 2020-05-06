using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class ReferenceEqualsComparer : IEqualityComparer<object>
    {
        public static IEqualityComparer<object> Instance { get; } = new ReferenceEqualsComparer();
        private ReferenceEqualsComparer() { }
        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
