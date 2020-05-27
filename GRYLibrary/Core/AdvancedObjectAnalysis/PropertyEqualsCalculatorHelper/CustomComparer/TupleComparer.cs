using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class TupleComparer : AbstractCustomComparer
    {
        internal TupleComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToTuple<object, object>(item1), Utilities.ObjectToTuple<object, object>(item2));
            return result;
        }

        internal bool EqualsTyped(Tuple<object, object> Tuple1, Tuple<object, object> Tuple2)
        {
            return new PropertyEqualsCalculator(this.Configuration).Equals(Tuple1.Item1, Tuple2.Item1) && new PropertyEqualsCalculator(this.Configuration).Equals(Tuple1.Item2, Tuple2.Item2);
        }

        public override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetRuntimeHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsTuple(type);
        }
    }
}
