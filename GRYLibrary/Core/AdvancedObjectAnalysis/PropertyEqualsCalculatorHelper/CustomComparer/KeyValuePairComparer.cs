using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class KeyValuePairComparer : AbstractCustomComparer
    {
        private KeyValuePairComparer() { }
        public static KeyValuePairComparer DefaultInstance { get; } = new KeyValuePairComparer();

        public override bool Equals(object keyValuePair1, object keyValuePair2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return this.EqualsTyped(Utilities.ObjectToKeyValuePair<object, object>(keyValuePair1), Utilities.ObjectToKeyValuePair<object, object>(keyValuePair2), visitedObjects);
        }

        public bool EqualsTyped(KeyValuePair<object, object> keyValuePair1, KeyValuePair<object, object> keyValuePair2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return PropertyEqualsCalculator.DefaultInstance.Equals(keyValuePair1.Key, keyValuePair2.Key, visitedObjects) && PropertyEqualsCalculator.DefaultInstance.Equals(keyValuePair1.Value, keyValuePair2.Value, visitedObjects);
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsKeyValuePair(type);
        }
    }
}
