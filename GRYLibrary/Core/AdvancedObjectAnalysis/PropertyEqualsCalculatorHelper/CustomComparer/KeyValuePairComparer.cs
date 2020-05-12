using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class KeyValuePairComparer : AbstractCustomComparer
    {
        private KeyValuePairComparer() { }
        public static AbstractCustomComparer DefaultInstance { get; } = new KeyValuePairComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            throw new NotImplementedException();
        }

        public  bool EqualsTyped(KeyValuePair<object, object> x, KeyValuePair<object, object> y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(x.Key, y.Key, visitedObjects) && PropertyEqualsCalculator.DefaultInstance.Equals(x.Value, y.Value, visitedObjects);
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
