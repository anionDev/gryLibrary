using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class DictionaryComparer : AbstractCustomComparer
    {
        internal DictionaryComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        internal override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToDictionary<object, object>(item1), Utilities.ObjectToDictionary<object, object>(item2));
            return result;
        }
        internal bool EqualsTyped<TKey, TValue>(IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count)
            {
                return false;
            }
            foreach (TKey key in dictionary1.Keys)
            {
                if (this.ContainsKey(dictionary2, key))
                {
                    KeyValuePair<TKey, TValue> kvp1 = new KeyValuePair<TKey, TValue>(key, dictionary1[key]);
                    KeyValuePair<TKey, TValue> kvp2 = new KeyValuePair<TKey, TValue>(key, dictionary2[key]);
                    if (!this._PropertyEqualsCalculator.Equals(kvp1, kvp2))
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

        private bool ContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                if (new PropertyEqualsCalculator(this.Configuration).Equals(kvp.Key, key))
                {
                    return true;
                }
            }
            return false;
        }
        internal override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }
        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return Utilities.TypeIsDictionary(typeOfObject1) && Utilities.TypeIsDictionary(typeOfObject2);
        }
    }
}
