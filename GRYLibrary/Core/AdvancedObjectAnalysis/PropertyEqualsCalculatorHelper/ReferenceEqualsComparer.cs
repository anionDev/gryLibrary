using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public class ReferenceEqualsComparer : IEqualityComparer<object>
    {
        public new bool Equals(object item1, object item2)
        {            
            return Utilities.ImprovedReferenceEquals(item1,item2);
        }

        public int GetHashCode(object obj)
        {
            int result = RuntimeHelpers.GetHashCode(obj);
            return result;
        }
    }
}
