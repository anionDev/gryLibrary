using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class PrimitiveComparer : AbstractCustomComparer
    {
        internal PrimitiveComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2 )
        {
            bool result = this.Configuration.AreInSameEquivalenceClass(item1, item2);
            return result;
        }

        public override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetRuntimeHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return type.IsPrimitive || typeof(string).Equals(type) || type.IsValueType;
        }
    }
}
