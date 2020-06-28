using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class KeyValuePairComparer : AbstractCustomComparer
    {
        internal KeyValuePairComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        internal override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToKeyValuePair<object, object>(item1), Utilities.ObjectToKeyValuePair<object, object>(item2));
            return result;
        }

        internal bool EqualsTyped(KeyValuePair<object, object> keyValuePair1, KeyValuePair<object, object> keyValuePair2)
        {
            return this._PropertyEqualsCalculator.Equals(keyValuePair1.Key, keyValuePair2.Key) && this._PropertyEqualsCalculator.Equals(keyValuePair1.Value, keyValuePair2.Value);
        }

        internal override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return Utilities.TypeIsKeyValuePair(typeOfObject1) && Utilities.TypeIsKeyValuePair(typeOfObject2);
        }
    }
}
