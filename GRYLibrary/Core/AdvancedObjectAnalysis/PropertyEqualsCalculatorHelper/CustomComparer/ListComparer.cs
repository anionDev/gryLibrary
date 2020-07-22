using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class ListComparer : AbstractCustomComparer
    {
        internal ListComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration) : base(cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        public override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToList<object>(item1), Utilities.ObjectToList<object>(item2));
            return result;
        }

        internal bool EqualsTyped<T>(IList<T> list1, IList<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }
            for (int i = 0; i < list1.Count; i++)
            {
                if (!this._PropertyEqualsCalculator.Equals(list1[i], list2[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }

        public override bool IsApplicable(Type typeOfObject1, Type typeOfObject2)
        {
            return Utilities.TypeIsListGeneric(typeOfObject1) && Utilities.TypeIsListGeneric(typeOfObject2);
        }
    }
}
