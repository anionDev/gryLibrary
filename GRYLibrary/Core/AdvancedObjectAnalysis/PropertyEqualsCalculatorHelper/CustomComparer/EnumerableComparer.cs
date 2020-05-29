using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    internal class EnumerableComparer : AbstractCustomComparer
    {
        internal EnumerableComparer(PropertyEqualsCalculatorConfiguration cacheAndConfiguration)
        {
            this.Configuration = cacheAndConfiguration;
        }

        internal override bool DefaultEquals(object item1, object item2)
        {
            bool result = this.EqualsTyped(Utilities.ObjectToEnumerable(item1), Utilities.ObjectToEnumerable(item2));
            return result;
        }
        internal bool EqualsTyped(IEnumerable enumerable1, IEnumerable enumerable2)
        {
            if (enumerable1.Count() != enumerable2.Count())
            {
                return false;
            }
            foreach (object item in enumerable1)
            {
                if (this.GetCountOfItemInEnumerable(enumerable1, item) != this.GetCountOfItemInEnumerable(enumerable2, item))
                {
                    return false;
                }
            }
            return true;
        }

        private int GetCountOfItemInEnumerable(IEnumerable enumerable, object item)
        {
            int result = 0;
            foreach (object enumerableEntry in enumerable)
            {
                if (new PropertyEqualsCalculator(this.Configuration).Equals(item))
                {
                    result += 1;
                }
            }

            return result;
        }
        internal override int DefaultGetHashCode(object obj)
        {
            return this.Configuration.GetHashCode(obj);
        }
        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsEnumerable(type);
        }
    }
}
