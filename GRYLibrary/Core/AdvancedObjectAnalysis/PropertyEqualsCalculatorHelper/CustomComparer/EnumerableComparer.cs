using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class EnumerableComparer : AbstractCustomComparer
    {
        private EnumerableComparer() { }
        public static AbstractCustomComparer DefaultInstance { get; } = new EnumerableComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }
        public  bool EqualsTyped(IEnumerable<object> x, IEnumerable<object> y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicable(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
