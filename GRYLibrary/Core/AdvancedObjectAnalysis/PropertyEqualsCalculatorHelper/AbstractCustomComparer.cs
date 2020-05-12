using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper
{
    public abstract class AbstractCustomComparer : GRYEqualityComparer<object>
    {
        public abstract override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects);

        public abstract override int GetHashCode(object obj);
        public abstract bool IsApplicable(Type type);
    }

}
