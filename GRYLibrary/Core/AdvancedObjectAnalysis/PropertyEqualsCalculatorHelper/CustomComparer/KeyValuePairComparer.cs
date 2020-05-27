using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class KeyValuePairComparer : AbstractCustomComparer
    {
        internal KeyValuePairComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToKeyValuePair<object, object>(item1), Utilities.ObjectToKeyValuePair<object, object>(item2));
            return result;
        }

        internal bool EqualsTyped(KeyValuePair<object, object> keyValuePair1, KeyValuePair<object, object> keyValuePair2)
        {
            return new PropertyEqualsCalculator(this.Configuration).Equals(keyValuePair1.Key, keyValuePair2.Key) && new PropertyEqualsCalculator(this.Configuration).Equals(keyValuePair1.Value, keyValuePair2.Value);
        }

        public override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetRuntimeHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsKeyValuePair(type);
        }
    }
}
