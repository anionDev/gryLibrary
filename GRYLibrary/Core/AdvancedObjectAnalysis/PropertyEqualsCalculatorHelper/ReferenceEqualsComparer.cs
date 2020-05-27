using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class ReferenceEqualsComparer : IEqualityComparer<object>
    {
        public new bool Equals(object item1, object item2)
        {
            return object.ReferenceEquals(item1, item2);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
