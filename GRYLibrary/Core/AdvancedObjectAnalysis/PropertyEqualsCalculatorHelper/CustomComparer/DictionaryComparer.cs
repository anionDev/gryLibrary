﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class DictionaryComparer : AbstractCustomComparer
    {
        internal DictionaryComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2)
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
                    if (!new PropertyEqualsCalculator(Configuration).Equals(kvp1, kvp2))
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
                if (new PropertyEqualsCalculator(Configuration).Equals(kvp.Key, key))
                {
                    return true;
                }
            }
            return false;
        }
        public override int DefaultGetHashCode(object obj)
        {
            return Configuration.GetRuntimeHashCode(obj);
        }
        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsDictionary(type);
        }
    }
}
