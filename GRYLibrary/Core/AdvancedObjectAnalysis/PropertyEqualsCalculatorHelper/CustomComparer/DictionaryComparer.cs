using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class DictionaryComparer : AbstractCustomComparer
    {
        private DictionaryComparer() { }
        public static DictionaryComparer DefaultInstance { get; } = new DictionaryComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return this.EqualsTyped(Utilities.ObjectToDictionary<object, object>(x), Utilities.ObjectToDictionary<object, object>(y), visitedObjects);
        }
        public bool EqualsTyped<TKey, TValue>(IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            if (dictionary1.Count != dictionary2.Count)
            {
                return false;
            }
            foreach (TKey key in dictionary1.Keys)
            {
                if (this.ContainsKey(dictionary2, key, visitedObjects))
                {
                    KeyValuePair<TKey, TValue> kvp1 = new KeyValuePair<TKey, TValue>(key, dictionary1[key]);
                    KeyValuePair<TKey, TValue> kvp2 = new KeyValuePair<TKey, TValue>(key, dictionary2[key]);
                    if (!PropertyEqualsCalculator.DefaultInstance.Equals(kvp1, kvp2, visitedObjects))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                if (PropertyEqualsCalculator.DefaultInstance.Equals(kvp.Key, key, visitedObjects))
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsDictionary(type);
        }
    }
}
