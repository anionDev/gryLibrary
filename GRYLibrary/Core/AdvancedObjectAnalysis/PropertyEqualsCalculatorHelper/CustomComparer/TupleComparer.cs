﻿using System;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class TupleComparer : AbstractCustomComparer
    {
        internal TupleComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
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
            return this._PropertyEqualsCalculator.Equals(Tuple1.Item1, Tuple2.Item1) && this._PropertyEqualsCalculator.Equals(Tuple1.Item2, Tuple2.Item2);
        }

        public override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return Utilities.TypeIsTuple(typeOfObject1) && Utilities.TypeIsTuple(typeOfObject2);
        }
    }
}
