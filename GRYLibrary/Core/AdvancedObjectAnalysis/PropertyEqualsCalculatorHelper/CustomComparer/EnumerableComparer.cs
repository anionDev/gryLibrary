using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;

namespace GRYLibrary.Core.AdvancedObjectAnalysis.PropertyEqualsCalculatorHelper.CustomComparer
{
    public class EnumerableComparer : AbstractCustomComparer
    {
        private EnumerableComparer() { }
        public static EnumerableComparer DefaultInstance { get; } = new EnumerableComparer();

        public override bool Equals(object x, object y, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            return this.EqualsTyped(Utilities.ObjectToEnumerable(x), Utilities.ObjectToEnumerable(y), visitedObjects);
        }
        public bool EqualsTyped(IEnumerable enumerable1, IEnumerable enumerable2, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            if (enumerable1.Count() != enumerable2.Count())
            {
                return false;
            }
            foreach (object item in enumerable1)
            {
                if (this.GetCountOfItemInEnumerable(enumerable1, item, visitedObjects) != this.GetCountOfItemInEnumerable(enumerable2, item, visitedObjects))
                {
                    return false;
                }
            }
            return true;
        }

        private int GetCountOfItemInEnumerable(IEnumerable enumerable, object item, ISet<PropertyEqualsCalculatorTuple> visitedObjects)
        {
            int result = 0;
            foreach (object enumerableEntry in enumerable)
            {
                if (PropertyEqualsCalculator.DefaultInstance.Equals(item, visitedObjects))
                {
                    result += 1;
                }
            }

            return result;
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public override bool IsApplicable(Type type)
        {
            return Utilities.TypeIsEnumerable(type);
        }
    }
}
